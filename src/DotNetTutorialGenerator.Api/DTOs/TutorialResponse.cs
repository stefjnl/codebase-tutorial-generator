namespace DotNetTutorialGenerator.Api.DTOs
{
    public class TutorialResponse
    {
        public string TutorialId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
    }
}
