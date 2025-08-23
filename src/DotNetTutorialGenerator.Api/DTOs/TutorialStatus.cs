namespace DotNetTutorialGenerator.Api.DTOs
{
    public class TutorialStatus
    {
        public string TutorialId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed
        public int Progress { get; set; } // 0-100
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
