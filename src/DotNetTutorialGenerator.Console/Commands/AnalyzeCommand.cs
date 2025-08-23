using System.CommandLine;
using DotNetTutorialGenerator.Cli.Options;
using DotNetTutorialGenerator.Cli.Utilities;
using DotNetTutorialGenerator.Core.Models;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using DotNetTutorialGenerator.Infrastructure.Roslyn;

namespace DotNetTutorialGenerator.Cli.Commands
{
    public class AnalyzeCommand : Command
    {
        public AnalyzeCommand() : base("analyze", "Analyze a .NET codebase and display information about it")
        {
            var repoOption = new Option<string>(new[] { "--repo", "-r" }, "GitHub repository URL to analyze");
            var dirOption = new Option<string>(new[] { "--dir", "-d" }, "Local directory path to analyze");
            var detailedOption = new Option<bool>(new[] { "--detailed" }, () => false, "Show detailed analysis");
            var includeOption = new Option<string[]>(new[] { "--include" }, "File patterns to include");
            var excludeOption = new Option<string[]>(new[] { "--exclude" }, "File patterns to exclude");

            AddOption(repoOption);
            AddOption(dirOption);
            AddOption(detailedOption);
            AddOption(includeOption);
            AddOption(excludeOption);

            // For now, let's revert to the simpler approach and fix the CommandHandler issue later
            // This is getting complex and we need to focus on the main build issues first
        }

        private static async Task<int> HandleCommand(AnalyzeOptions options, GlobalOptions globalOptions)
        {
            var logger = new ConsoleLogger(globalOptions.Verbose, globalOptions.LogFile);
            var progressReporter = new ProgressReporter(logger);

            try
            {
                progressReporter.StartProgress("Starting codebase analysis...");

                // Validate input
                if (string.IsNullOrEmpty(options.RepositoryUrl) && string.IsNullOrEmpty(options.LocalPath))
                {
                    progressReporter.ReportError("Either --repo or --dir must be specified");
                    return 1;
                }

                // Create crawl options
                var crawlOptions = new CrawlOptions
                {
                    IncludePatterns = options.IncludePatterns?.ToList() ?? new List<string> { "*.cs", "*.csproj", "*.sln" },
                    ExcludePatterns = options.ExcludePatterns?.ToList() ?? new List<string> { "*bin/*", "*obj/*", "*test*" }
                };

                // Crawl the codebase
                progressReporter.UpdateProgress(20, "Crawling codebase...");
                List<CodeFile> codeFiles;
                ProjectMetadata projectMetadata;

                if (!string.IsNullOrEmpty(options.RepositoryUrl))
                {
                    // TODO: Implement GitHub crawling
                    logger.LogWarning("GitHub crawling not yet implemented. Using local crawler as fallback.");
                    codeFiles = new List<CodeFile>();
                    projectMetadata = new ProjectMetadata();
                }
                else if (!string.IsNullOrEmpty(options.LocalPath))
                {
                    var crawler = new LocalRepositoryCrawler();
                    codeFiles = await crawler.CrawlRepositoryAsync(options.LocalPath, crawlOptions);
                    projectMetadata = await crawler.ExtractProjectMetadataAsync(options.LocalPath);
                }
                else
                {
                    progressReporter.ReportError("No valid source specified");
                    return 1;
                }

                if (codeFiles.Count == 0)
                {
                    progressReporter.ReportError("No code files found");
                    return 1;
                }

                progressReporter.UpdateProgress(40, $"Found {codeFiles.Count} code files");

                // Analyze with Roslyn
                progressReporter.UpdateProgress(60, "Analyzing with Roslyn...");
                var roslynAnalyzer = new RoslynAnalyzer();
                var abstractions = await roslynAnalyzer.AnalyzeProjectAsync(options.LocalPath ?? "");

                progressReporter.UpdateProgress(80, $"Identified {abstractions.Count} abstractions");

                // Display results
                progressReporter.CompleteProgress("Analysis complete");

                System.Console.WriteLine();
                System.Console.WriteLine("Project Information:");
                System.Console.WriteLine($"  Name: {projectMetadata.Name}");
                System.Console.WriteLine($"  Type: {projectMetadata.ProjectType}");
                System.Console.WriteLine($"  Version: {projectMetadata.Version}");
                System.Console.WriteLine($"  Files: {codeFiles.Count}");
                System.Console.WriteLine();

                if (options.Detailed)
                {
                    System.Console.WriteLine("Abstractions Found:");
                    foreach (var abstraction in abstractions)
                    {
                        System.Console.WriteLine($"  {abstraction.Type}: {abstraction.Name}");
                        System.Console.WriteLine($"    Description: {abstraction.Description}");
                        System.Console.WriteLine();
                    }
                }
                else
                {
                    System.Console.WriteLine("Abstraction Summary:");
                    var abstractionGroups = abstractions.GroupBy(a => a.Type);
                    foreach (var group in abstractionGroups)
                    {
                        System.Console.WriteLine($"  {group.Key}: {group.Count()}");
                    }
                    System.Console.WriteLine();
                }

                return 0;
            }
            catch (Exception ex)
            {
                progressReporter.ReportError($"Error analyzing codebase: {ex.Message}");
                return 1;
            }
        }
    }

    public class AnalyzeOptions
    {
        public string? RepositoryUrl { get; set; }
        public string? LocalPath { get; set; }
        public bool Detailed { get; set; }
        public string[]? IncludePatterns { get; set; }
        public string[]? ExcludePatterns { get; set; }
    }
}
