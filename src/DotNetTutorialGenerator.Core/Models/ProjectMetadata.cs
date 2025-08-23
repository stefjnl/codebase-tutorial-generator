using System;
using System.Collections.Generic;

namespace DotNetTutorialGenerator.Core.Models
{
    public class ProjectMetadata
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetFramework { get; set; } = string.Empty;
        public List<string> Authors { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; }
        public List<string> Dependencies { get; set; } = new List<string>();
        public string ProjectType { get; set; } = string.Empty; // Web API, MVC, Console, etc.
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
