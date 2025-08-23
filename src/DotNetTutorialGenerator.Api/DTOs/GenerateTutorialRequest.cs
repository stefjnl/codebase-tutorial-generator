namespace DotNetTutorialGenerator.Api.DTOs
{
    public class GenerateTutorialRequest
    {
        public string RepositoryUrl { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public string LlmProvider { get; set; } = "openai";
        public string OutputFormat { get; set; } = "markdown";
        public bool GenerateDiagrams { get; set; } = true;
        public int MaxAbstractions { get; set; } = 10;
    }
}
