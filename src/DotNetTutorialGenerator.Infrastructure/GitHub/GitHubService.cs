using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Models;
using DotNetTutorialGenerator.Infrastructure.GitHub.Models;

namespace DotNetTutorialGenerator.Infrastructure.GitHub
{
    public class GitHubService : IRepositoryCrawler
    {
        private readonly GitHubApiClient _gitHubClient;

        public GitHubService(GitHubApiClient gitHubClient)
        {
            _gitHubClient = gitHubClient;
        }

        public async Task<List<CodeFile>> CrawlRepositoryAsync(string repositoryUrl, CrawlOptions options)
        {
            var codeFiles = new List<CodeFile>();

            // Parse repository URL to extract owner and repo name
            var uri = new Uri(repositoryUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
                throw new ArgumentException("Invalid GitHub repository URL");

            var owner = segments[1].TrimEnd('/');
            var repo = segments[2].TrimEnd('/');

            // Get repository information
            var repository = await _gitHubClient.GetRepositoryAsync(owner, repo);
            if (repository == null)
                throw new InvalidOperationException("Failed to retrieve repository information");

            // Crawl repository contents
            await CrawlRepositoryContentsAsync(owner, repo, "", codeFiles, options);

            return codeFiles;
        }

        private async Task CrawlRepositoryContentsAsync(string owner, string repo, string path, List<CodeFile> codeFiles, CrawlOptions options, int currentDepth = 0)
        {
            if (currentDepth > options.MaxDepth)
                return;

            var contents = await _gitHubClient.GetRepositoryContentsAsync(owner, repo, path);
            if (contents == null)
                return;

            foreach (var item in contents)
            {
                // Check if item should be excluded
                if (options.ExcludePatterns.Any(pattern => MatchesPattern(item.Path, pattern)))
                    continue;

                if (item.Type == "file")
                {
                    // Check if file matches include patterns
                    if (options.IncludePatterns.Any(pattern => MatchesPattern(item.Path, pattern)))
                    {
                        // Check file size
                        if (item.Size <= options.MaxFileSize)
                        {
                            var content = await _gitHubClient.GetFileContentAsync(item.DownloadUrl);
                            if (content != null)
                            {
                                codeFiles.Add(new CodeFile
                                {
                                    FilePath = item.Path,
                                    Content = content,
                                    Language = GetLanguageFromExtension(item.Name),
                                    SizeInBytes = item.Size,
                                    LastModified = item.LastModified
                                });
                            }
                        }
                    }
                }
                else if (item.Type == "dir" && options.Recursive)
                {
                    // Recursively crawl subdirectories
                    await CrawlRepositoryContentsAsync(owner, repo, item.Path, codeFiles, options, currentDepth + 1);
                }
            }
        }

        private bool MatchesPattern(string path, string pattern)
        {
            // Simple pattern matching (could be enhanced with more sophisticated glob matching)
            if (pattern.StartsWith("*") && pattern.EndsWith("*"))
            {
                return path.Contains(pattern.Trim('*'));
            }
            else if (pattern.StartsWith("*"))
            {
                return path.EndsWith(pattern.Trim('*'));
            }
            else if (pattern.EndsWith("*"))
            {
                return path.StartsWith(pattern.Trim('*'));
            }
            else
            {
                return path.Equals(pattern, StringComparison.OrdinalIgnoreCase);
            }
        }

        private string GetLanguageFromExtension(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".cs" => "C#",
                ".csproj" => "C# Project",
                ".sln" => "Solution",
                ".json" => "JSON",
                ".xml" => "XML",
                ".config" => "Config",
                _ => "Unknown"
            };
        }

        public async Task<ProjectMetadata> ExtractProjectMetadataAsync(string repositoryUrl)
        {
            var metadata = new ProjectMetadata
            {
                CreatedDate = DateTime.UtcNow
            };

            // Parse repository URL to extract owner and repo name
            var uri = new Uri(repositoryUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
                throw new ArgumentException("Invalid GitHub repository URL");

            var owner = segments[1].TrimEnd('/');
            var repo = segments[2].TrimEnd('/');

            // Get repository information
            var repository = await _gitHubClient.GetRepositoryAsync(owner, repo);
            if (repository != null)
            {
                metadata.Name = repository.Name;
                metadata.Description = repository.Description;
                metadata.Version = "1.0.0"; // Default version
                metadata.Authors.Add(owner);
                metadata.ProjectType = "GitHub Repository";
            }

            return metadata;
        }
    }
}
