namespace DotNetTutorialGenerator.Core.Models
{
    public enum GenerationPhase
    {
        Initializing,
        AnalyzingCodebase,
        IdentifyingAbstractions,
        AnalyzingRelationships,
        GeneratingContent,
        FormattingOutput,
        Completed,
        Failed
    }

    public class GenerationProgress
    {
        public string TaskId { get; set; } = string.Empty;
        public GenerationPhase CurrentPhase { get; set; }
        public int PercentComplete { get; set; }
        public List<string> Logs { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string CurrentOperation { get; set; } = string.Empty;

        public void AddLog(string message)
        {
            Logs.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}");
        }

        public void UpdateProgress(GenerationPhase phase, int percent, string operation)
        {
            CurrentPhase = phase;
            PercentComplete = percent;
            CurrentOperation = operation;
            AddLog($"Progress updated: {phase} ({percent}%) - {operation}");
        }
    }
}
