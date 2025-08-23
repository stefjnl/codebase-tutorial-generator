using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Models;
using Microsoft.CodeAnalysis;

namespace DotNetTutorialGenerator.Core.Interfaces
{
    public interface IRoslynAnalyzer
    {
        Task<List<Abstraction>> AnalyzeProjectAsync(string projectPath);
        Task<Abstraction> AnalyzeClassAsync(string filePath);
        Task<List<string>> GetDependenciesAsync(string filePath);
        Task<ProjectMetadata> ExtractProjectMetadataAsync(string projectPath);
    }
}
