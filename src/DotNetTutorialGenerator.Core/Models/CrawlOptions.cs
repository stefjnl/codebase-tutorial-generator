using System.Collections.Generic;

namespace DotNetTutorialGenerator.Core.Models
{
    public class CrawlOptions
    {
        public long MaxFileSize { get; set; } = 102400; // 100KB default
        public int MaxAbstractions { get; set; } = 10;
        public List<string> IncludePatterns { get; set; } = new List<string> { "*.cs", "*.csproj", "*.sln" };
        public List<string> ExcludePatterns { get; set; } = new List<string> { "*bin/*", "*obj/*", "*test*" };
        public bool Recursive { get; set; } = true;
        public int MaxDepth { get; set; } = 10;
    }
}
