using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Core.Interfaces
{
    public interface IRelationshipAnalyzer
    {
        Task<List<RelationshipDiagram>> AnalyzeRelationshipsAsync(List<Abstraction> abstractions, List<CodeFile> codeFiles);
        Task<RelationshipDiagram> CreateClassDiagramAsync(List<Abstraction> abstractions);
        Task<RelationshipDiagram> CreateComponentDiagramAsync(List<Abstraction> abstractions);
    }
}
