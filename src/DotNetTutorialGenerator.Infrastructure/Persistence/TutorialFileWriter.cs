using System;
using System.IO;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Infrastructure.Persistence
{
    public class TutorialFileWriter : ITutorialGenerator
    {
        public async Task<List<TutorialChapter>> GenerateTutorialAsync(List<Abstraction> abstractions, List<RelationshipDiagram> diagrams)
        {
            var chapters = new List<TutorialChapter>();

            // Create introduction chapter
            chapters.Add(new TutorialChapter
            {
                Id = 1,
                Title = "Introduction",
                Content = "This tutorial explains how the .NET application is structured and how its components work together.",
                Order = 1
            });

            // Create chapters for each abstraction
            for (int i = 0; i < abstractions.Count; i++)
            {
                var abstraction = abstractions[i];
                chapters.Add(new TutorialChapter
                {
                    Id = i + 2,
                    Title = $"{abstraction.Type}: {abstraction.Name}",
                    Content = abstraction.Description,
                    Order = i + 2
                });
            }

            // Create relationships chapter
            if (diagrams.Count > 0)
            {
                chapters.Add(new TutorialChapter
                {
                    Id = abstractions.Count + 2,
                    Title = "Architecture Diagrams",
                    Content = "The following diagrams show how the components relate to each other.",
                    Order = abstractions.Count + 2,
                    Diagram = diagrams.Count > 0 ? diagrams[0].MermaidSyntax : string.Empty
                });
            }

            return chapters;
        }

        public async Task<string> GenerateMarkdownTutorialAsync(List<TutorialChapter> chapters)
        {
            var markdown = "# .NET Application Tutorial\n\n";

            foreach (var chapter in chapters.OrderBy(c => c.Order))
            {
                markdown += $"## {chapter.Title}\n\n";
                markdown += $"{chapter.Content}\n\n";

                if (!string.IsNullOrEmpty(chapter.Diagram))
                {
                    markdown += "```mermaid\n";
                    markdown += $"{chapter.Diagram}\n";
                    markdown += "```\n\n";
                }

                if (chapter.CodeExamples.Count > 0)
                {
                    foreach (var codeExample in chapter.CodeExamples)
                    {
                        markdown += "```csharp\n";
                        markdown += $"{codeExample}\n";
                        markdown += "```\n\n";
                    }
                }
            }

            return markdown;
        }

        public async Task<string> GenerateHtmlTutorialAsync(List<TutorialChapter> chapters)
        {
            var html = "<!DOCTYPE html>\n<html>\n<head>\n";
            html += "<title>.NET Application Tutorial</title>\n";
            html += "<script src=\"https://cdn.jsdelivr.net/npm/mermaid@10.6.1/dist/mermaid.min.js\"></script>\n";
            html += "<script>mermaid.initialize({startOnLoad:true});</script>\n";
            html += "</head>\n<body>\n";
            html += "<h1>.NET Application Tutorial</h1>\n";

            foreach (var chapter in chapters.OrderBy(c => c.Order))
            {
                html += $"<h2>{chapter.Title}</h2>\n";
                html += $"<p>{chapter.Content}</p>\n";

                if (!string.IsNullOrEmpty(chapter.Diagram))
                {
                    html += "<div class=\"mermaid\">\n";
                    html += $"{chapter.Diagram}\n";
                    html += "</div>\n";
                }

                if (chapter.CodeExamples.Count > 0)
                {
                    foreach (var codeExample in chapter.CodeExamples)
                    {
                        html += "<pre><code class=\"language-csharp\">\n";
                        html += $"{codeExample}\n";
                        html += "</code></pre>\n";
                    }
                }
            }

            html += "</body>\n</html>";
            return html;
        }

        public async Task SaveTutorialAsync(string content, string outputPath, string format = "md")
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(outputPath, content);
        }
    }
}
