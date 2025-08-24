using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Core.Interfaces
{
    public interface ITutorialOrchestrator
    {
        Task<string> GenerateTutorialAsync(string source, TutorialGenerationOptions options, IProgress<GenerationProgress> progress, CancellationToken cancellationToken);
        Task<ValidationResult> ValidateSourceAsync(string source, CancellationToken cancellationToken);
    }

    public class TutorialGenerationOptions
    {
        public string Language { get; set; } = "English";
        public int MaxAbstractions { get; set; } = 15;
        public bool GenerateDiagrams { get; set; } = true;
        public string[] IncludePatterns { get; set; } = Array.Empty<string>();
        public string[] ExcludePatterns { get; set; } = Array.Empty<string>();
    }

    public class GenerationProgress
    {
        public int Percentage { get; set; }
        public string CurrentPhase { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = "";
        public List<string> Issues { get; set; } = new List<string>();
    }
}
