using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetTutorialGenerator.Infrastructure.Roslyn
{
    public class RoslynAnalyzer : IRoslynAnalyzer
    {
        public async Task<List<Abstraction>> AnalyzeProjectAsync(string projectPath)
        {
            var abstractions = new List<Abstraction>();

            if (!Directory.Exists(projectPath))
                throw new DirectoryNotFoundException($"Project path not found: {projectPath}");

            // Find all C# files in the project
            var csFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                var abstraction = await AnalyzeClassAsync(file);
                if (abstraction != null)
                {
                    abstractions.Add(abstraction);
                }
            }

            return abstractions;
        }

        public async Task<Abstraction?> AnalyzeClassAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                var code = await File.ReadAllTextAsync(filePath);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = await tree.GetRootAsync();

                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();
                var controllerClasses = classDeclarations.Where(c => c.Identifier.Text.EndsWith("Controller"));

                var abstraction = new Abstraction
                {
                    Id = filePath.GetHashCode(),
                    Name = Path.GetFileNameWithoutExtension(filePath),
                    FilePath = filePath
                };

                // Determine the type of abstraction
                if (controllerClasses.Any())
                {
                    abstraction.Type = "Controller";
                    abstraction.Description = $"MVC Controller class with {controllerClasses.Count()} controller(s)";
                }
                else if (classDeclarations.Any())
                {
                    abstraction.Type = "Class";
                    abstraction.Description = $"Class with {classDeclarations.Count()} class(es)";
                }
                else if (interfaceDeclarations.Any())
                {
                    abstraction.Type = "Interface";
                    abstraction.Description = $"Interface with {interfaceDeclarations.Count()} interface(s)";
                }
                else
                {
                    abstraction.Type = "Unknown";
                    abstraction.Description = "File with no recognized C# constructs";
                }

                return abstraction;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<string>> GetDependenciesAsync(string filePath)
        {
            var dependencies = new List<string>();

            if (!File.Exists(filePath))
                return dependencies;

            try
            {
                var code = await File.ReadAllTextAsync(filePath);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = await tree.GetRootAsync();

                // Get using statements
                var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
                foreach (var usingDirective in usingDirectives)
                {
                    dependencies.Add(usingDirective.Name.ToString());
                }

                return dependencies;
            }
            catch (Exception)
            {
                return dependencies;
            }
        }

        public async Task<ProjectMetadata> ExtractProjectMetadataAsync(string projectPath)
        {
            var metadata = new ProjectMetadata
            {
                CreatedDate = DateTime.UtcNow
            };

            if (!Directory.Exists(projectPath))
                throw new DirectoryNotFoundException($"Project path not found: {projectPath}");

            metadata.Name = new DirectoryInfo(projectPath).Name;
            metadata.Version = "1.0.0"; // Default version

            // Try to find project files to extract more metadata
            var projectFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.AllDirectories);
            if (projectFiles.Length > 0)
            {
                metadata.ProjectType = FileSystem.FileTypeDetector.GetProjectTypeFromProjectFile(projectFiles[0]);
                // In a real implementation, you would parse the project file to extract more metadata
            }

            return metadata;
        }
    }
}
