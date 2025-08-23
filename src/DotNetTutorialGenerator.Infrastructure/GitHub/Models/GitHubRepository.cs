using System;

namespace DotNetTutorialGenerator.Infrastructure.GitHub.Models
{
    public class GitHubRepository
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string SshUrl { get; set; } = string.Empty;
        public string CloneUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int StargazersCount { get; set; }
        public int WatchersCount { get; set; }
        public int ForksCount { get; set; }
        public bool IsPrivate { get; set; }
        public string DefaultBranch { get; set; } = string.Empty;
    }
}
