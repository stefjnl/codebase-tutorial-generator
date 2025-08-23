using System.Collections.Generic;

namespace DotNetTutorialGenerator.Core.Models
{
    public class TutorialChapter
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<string> CodeExamples { get; set; } = new List<string>();
        public string Diagram { get; set; } = string.Empty; // Mermaid diagram syntax
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
