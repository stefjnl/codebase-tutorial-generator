using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Infrastructure.FileSystem
{
    public class LocalRepositoryCrawler : IRepositoryCrawler
    {
        public async Task<List<CodeFile>> CrawlRepositoryAsync(string repositoryPath, CrawlOptions options)
        {
            var codeFiles = new List<CodeFile>();

            if (!Directory.Exists(repositoryPath))
                throw new DirectoryNotFoundException($"Repository path not found: {repositoryPath}");

            await CrawlDirectoryAsync(repositoryPath, repositoryPath, codeFiles, options);

            return codeFiles;
        }

        private async Task CrawlDirectoryAsync(string rootPath, string currentPath, List<CodeFile> codeFiles, CrawlOptions options, int currentDepth = 0)
        {
            if (currentDepth > options.MaxDepth)
                return;

            // Get all files in the current directory
            var files = Directory.GetFiles(currentPath);
            foreach (var file in files)
            {
                // Check if file should be excluded
                var relativePath = GetRelativePath(rootPath, file);
                if (options.ExcludePatterns.Any(pattern => MatchesPattern(relativePath, pattern)))
                    continue;

                // Check if file matches include patterns
                if (options.IncludePatterns.Any(pattern => MatchesPattern(relativePath, pattern)))
                {
                    var fileInfo = new FileInfo(file);
                    // Check file size
                    if (fileInfo.Length <= options.MaxFileSize)
                    {
                        var content = await File.ReadAllTextAsync(file);
                        codeFiles.Add(new CodeFile
                        {
                            FilePath = relativePath,
                            Content = content,
                            Language = GetLanguageFromExtension(fileInfo.Name),
                            SizeInBytes = fileInfo.Length,
                            LastModified = fileInfo.LastWriteTime
                        });
                    }
                }
            }

            // Recursively crawl subdirectories if recursive is enabled
            if (options.Recursive)
            {
                var directories = Directory.GetDirectories(currentPath);
                foreach (var directory in directories)
                {
                    var dirInfo = new DirectoryInfo(directory);
                    var relativeDirPath = GetRelativePath(rootPath, directory);

                    // Check if directory should be excluded
                    if (!options.ExcludePatterns.Any(pattern => MatchesPattern(relativeDirPath, pattern)))
                    {
                        await CrawlDirectoryAsync(rootPath, directory, codeFiles, options, currentDepth + 1);
                    }
                }
            }
        }

        private string GetRelativePath(string rootPath, string fullPath)
        {
            var rootUri = new Uri(rootPath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? rootPath : rootPath + Path.DirectorySeparatorChar);
            var fullUri = new Uri(fullPath);
            var relativeUri = rootUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

        private bool MatchesPattern(string path, string pattern)
        {
            // Simple pattern matching (could be enhanced with more sophisticated glob matching)
            if (pattern.StartsWith("*") && pattern.EndsWith("*"))
            {
                return path.Contains(pattern.Trim('*'));
            }
            else if (pattern.StartsWith("*"))
            {
                return path.EndsWith(pattern.Trim('*'));
            }
            else if (pattern.EndsWith("*"))
            {
                return path.StartsWith(pattern.Trim('*'));
            }
            else
            {
                return path.Equals(pattern, StringComparison.OrdinalIgnoreCase);
            }
        }

        private string GetLanguageFromExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".cs" => "C#",
                ".csproj" => "C# Project",
                ".sln" => "Solution",
                ".json" => "JSON",
                ".xml" => "XML",
                ".config" => "Config",
                _ => "Unknown"
            };
        }

        public async Task<ProjectMetadata> ExtractProjectMetadataAsync(string repositoryPath)
        {
            var metadata = new ProjectMetadata
            {
                CreatedDate = DateTime.UtcNow
            };

            if (!Directory.Exists(repositoryPath))
                throw new DirectoryNotFoundException($"Repository path not found: {repositoryPath}");

            metadata.Name = new DirectoryInfo(repositoryPath).Name;
            metadata.Version = "1.0.0"; // Default version
            metadata.ProjectType = "Local Repository";

            // Try to find project files to extract more metadata
            var projectFiles = Directory.GetFiles(repositoryPath, "*.csproj", SearchOption.AllDirectories);
            if (projectFiles.Length > 0)
            {
                metadata.ProjectType = "C# Project";
                // In a real implementation, you would parse the project file to extract more metadata
            }

            return metadata;
        }
    }
}
