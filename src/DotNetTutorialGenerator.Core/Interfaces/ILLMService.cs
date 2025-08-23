using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Core.Interfaces
{
    public interface ILLMService
    {
        Task<string> GenerateTextAsync(string prompt, int maxTokens = 1000);
        Task<List<Abstraction>> IdentifyAbstractionsAsync(List<CodeFile> codeFiles);
        Task<List<RelationshipDiagram>> AnalyzeRelationshipsAsync(List<Abstraction> abstractions, List<CodeFile> codeFiles);
        Task<string> GenerateTutorialContentAsync(List<Abstraction> abstractions, List<RelationshipDiagram> diagrams);
        Task<T> GenerateStructuredResponseAsync<T>(string prompt, int maxTokens = 1000);
    }
}
