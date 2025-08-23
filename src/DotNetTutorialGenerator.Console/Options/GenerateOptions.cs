using System.CommandLine;
using System.CommandLine.Binding;

namespace DotNetTutorialGenerator.Cli.Options
{
    public class GenerateOptions
    {
        public string? RepositoryUrl { get; set; }
        public string? LocalPath { get; set; }
        public string? LlmProvider { get; set; }
        public string? OutputFormat { get; set; }
        public bool GenerateDiagrams { get; set; }
        public int MaxAbstractions { get; set; }
        public string? OutputPath { get; set; }
        public string[]? IncludePatterns { get; set; }
        public string[]? ExcludePatterns { get; set; }
    }
}
