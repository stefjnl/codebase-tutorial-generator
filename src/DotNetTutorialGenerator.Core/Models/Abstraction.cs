using System.Collections.Generic;

namespace DotNetTutorialGenerator.Core.Models
{
    public class Abstraction
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Controller, Service, Model, etc.
        public List<int> RelatedFileIndices { get; set; } = new List<int>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
