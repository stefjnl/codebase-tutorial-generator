using System.CommandLine;
using DotNetTutorialGenerator.Console.Options;
using DotNetTutorialGenerator.Console.Utilities;
using DotNetTutorialGenerator.Core.Models;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using DotNetTutorialGenerator.Infrastructure.Roslyn;

namespace DotNetTutorialGenerator.Console.Commands
{
    public class AnalyzeCommand : Command
    {
        public AnalyzeCommand() : base("analyze", "Analyze a .NET codebase and display information about it")
        {
            AddOption(new Option<string>(new[] { "--repo", "-r" }, "GitHub repository URL to analyze"));
            AddOption(new Option<string>(new[] { "--dir", "-d" }, "Local directory path to analyze"));
            AddOption(new Option<bool>(new[] { "--detailed" }, () => false, "Show detailed analysis"));
            AddOption(new Option<string[]>(new[] { "--include" }, "File patterns to include"));
            AddOption(new Option<string[]>(new[] { "--exclude" }, "File patterns to exclude"));

            Handler = CommandHandler.Create<AnalyzeOptions, GlobalOptions>(HandleCommand);
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

                Console.WriteLine();
                Console.WriteLine("Project Information:");
                Console.WriteLine($"  Name: {projectMetadata.Name}");
                Console.WriteLine($"  Type: {projectMetadata.ProjectType}");
                Console.WriteLine($"  Version: {projectMetadata.Version}");
                Console.WriteLine($"  Files: {codeFiles.Count}");
                Console.WriteLine();

                if (options.Detailed)
                {
                    Console.WriteLine("Abstractions Found:");
                    foreach (var abstraction in abstractions)
                    {
                        Console.WriteLine($"  {abstraction.Type}: {abstraction.Name}");
                        Console.WriteLine($"    Description: {abstraction.Description}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Abstraction Summary:");
                    var abstractionGroups = abstractions.GroupBy(a => a.Type);
                    foreach (var group in abstractionGroups)
                    {
                        Console.WriteLine($"  {group.Key}: {group.Count()}");
                    }
                    Console.WriteLine();
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
