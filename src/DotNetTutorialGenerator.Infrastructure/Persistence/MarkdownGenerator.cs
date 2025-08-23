using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Infrastructure.Persistence
{
    public static class MarkdownGenerator
    {
        public static string GenerateTutorialMarkdown(List<TutorialChapter> chapters)
        {
            var markdown = new StringBuilder();

            // Add title
            if (chapters.Count > 0)
            {
                markdown.AppendLine($"# {chapters.First().Title}");
                markdown.AppendLine();
            }

            // Add table of contents
            markdown.AppendLine("## Table of Contents");
            foreach (var chapter in chapters.OrderBy(c => c.Order))
            {
                markdown.AppendLine($"- [{chapter.Title}](#chapter-{chapter.Id})");
            }
            markdown.AppendLine();

            // Add chapters
            foreach (var chapter in chapters.OrderBy(c => c.Order))
            {
                markdown.AppendLine($"<a name=\"chapter-{chapter.Id}\"></a>");
                markdown.AppendLine($"## {chapter.Title}");
                markdown.AppendLine();
                markdown.AppendLine(chapter.Content);
                markdown.AppendLine();

                // Add code examples
                if (chapter.CodeExamples.Any())
                {
                    markdown.AppendLine("### Code Examples");
                    foreach (var example in chapter.CodeExamples)
                    {
                        markdown.AppendLine("```csharp");
                        markdown.AppendLine(example);
                        markdown.AppendLine("```");
                        markdown.AppendLine();
                    }
                }

                // Add diagram
                if (!string.IsNullOrEmpty(chapter.Diagram))
                {
                    markdown.AppendLine("### Diagram");
                    markdown.AppendLine("```mermaid");
                    markdown.AppendLine(chapter.Diagram);
                    markdown.AppendLine("```");
                    markdown.AppendLine();
                }
            }

            return markdown.ToString();
        }

        public static string GenerateAbstractionsMarkdown(List<Abstraction> abstractions)
        {
            var markdown = new StringBuilder();
            markdown.AppendLine("# Abstractions");
            markdown.AppendLine();

            foreach (var abstraction in abstractions)
            {
                markdown.AppendLine($"## {abstraction.Name} ({abstraction.Type})");
                markdown.AppendLine();
                markdown.AppendLine(abstraction.Description);
                markdown.AppendLine();

                if (abstraction.Metadata.Any())
                {
                    markdown.AppendLine("### Metadata");
                    foreach (var metadata in abstraction.Metadata)
                    {
                        markdown.AppendLine($"- {metadata.Key}: {metadata.Value}");
                    }
                    markdown.AppendLine();
                }
            }

            return markdown.ToString();
        }

        public static string GenerateRelationshipsMarkdown(List<RelationshipDiagram> diagrams)
        {
            var markdown = new StringBuilder();
            markdown.AppendLine("# Relationships");
            markdown.AppendLine();

            foreach (var diagram in diagrams)
            {
                markdown.AppendLine($"## {diagram.Title}");
                markdown.AppendLine();
                markdown.AppendLine("```mermaid");
                markdown.AppendLine(diagram.MermaidSyntax);
                markdown.AppendLine("```");
                markdown.AppendLine();
            }

            return markdown.ToString();
        }
    }
}
