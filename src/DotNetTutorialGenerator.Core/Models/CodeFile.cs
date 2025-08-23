using System;
using System.Collections.Generic;

namespace DotNetTutorialGenerator.Core.Models
{
    public class CodeFile
    {
        public string FilePath { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime LastModified { get; set; }
        public List<string> Dependencies { get; set; } = new List<string>();
    }
}
