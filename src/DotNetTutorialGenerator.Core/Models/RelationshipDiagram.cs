using System.Collections.Generic;

namespace DotNetTutorialGenerator.Core.Models
{
    public class RelationshipDiagram
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string DiagramType { get; set; } = string.Empty; // Class, Component, Sequence, etc.
        public string MermaidSyntax { get; set; } = string.Empty;
        public List<int> AbstractionIds { get; set; } = new List<int>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
