using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Core.Interfaces
{
    public interface ITutorialGenerator
    {
        Task<List<TutorialChapter>> GenerateTutorialAsync(List<Abstraction> abstractions, List<RelationshipDiagram> diagrams);
        Task<string> GenerateMarkdownTutorialAsync(List<TutorialChapter> chapters);
        Task<string> GenerateHtmlTutorialAsync(List<TutorialChapter> chapters);
        Task SaveTutorialAsync(string content, string outputPath, string format = "md");
    }
}
