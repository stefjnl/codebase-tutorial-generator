using DotNetTutorialGenerator.Console.Options;
using DotNetTutorialGenerator.Console.Utilities;
using DotNetTutorialGenerator.Core.Models;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using DotNetTutorialGenerator.Infrastructure.Persistence;
using System.CommandLine;

namespace DotNetTutorialGenerator.Console.Commands
{
    public class GenerateCommand : Command
    {
        public GenerateCommand() : base("generate", "Generate a tutorial from a .NET codebase")
        {
            AddOption(new Option<string>(new[] { "--repo", "-r" }, "GitHub repository URL to analyze"));
            AddOption(new Option<string>(new[] { "--dir", "-d" }, "Local directory path to analyze"));
            AddOption(new Option<string>(new[] { "--llm-provider" }, () => "openai", "LLM provider to use (openai, claude)"));
            AddOption(new Option<string>(new[] { "--output-format" }, () => "markdown", "Output format (markdown, html)"));
            AddOption(new Option<bool>(new[] { "--generate-diagrams" }, () => true, "Generate Mermaid diagrams"));
            AddOption(new Option<int>(new[] { "--max-abstractions" }, () => 10, "Maximum number of abstractions to identify"));
            AddOption(new Option<string>(new[] { "--output", "-o" }, "Output file path"));
            AddOption(new Option<string[]>(new[] { "--include" }, "File patterns to include"));
            AddOption(new Option<string[]>(new[] { "--exclude" }, "File patterns to exclude"));

            Handler = CommandHandler.Create<GenerateOptions, GlobalOptions>(HandleCommand);
        }

        private static async Task<int> HandleCommand(GenerateOptions options, GlobalOptions globalOptions)
        {
            var logger = new ConsoleLogger(globalOptions.Verbose, globalOptions.LogFile);
            var progressReporter = new ProgressReporter(logger);

            try
            {
                progressReporter.StartProgress("Starting tutorial generation...");

                // Validate input
                if (string.IsNullOrEmpty(options.RepositoryUrl) && string.IsNullOrEmpty(options.LocalPath))
                {
                    progressReporter.ReportError("Either --repo or --dir must be specified");
                    return 1;
                }

                // Create crawl options
                var crawlOptions = new CrawlOptions
                {
                    MaxAbstractions = options.MaxAbstractions,
                    IncludePatterns = options.IncludePatterns?.ToList() ?? new List<string> { "*.cs", "*.csproj", "*.sln" },
                    ExcludePatterns = options.ExcludePatterns?.ToList() ?? new List<string> { "*bin/*", "*obj/*", "*test*" }
                };

                // Crawl the codebase
                progressReporter.UpdateProgress(10, "Crawling codebase...");
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

                progressReporter.UpdateProgress(30, $"Found {codeFiles.Count} code files");

                // Identify abstractions
                progressReporter.UpdateProgress(50, "Identifying abstractions...");
                var abstractions = new List<Abstraction>();
                // TODO: Implement abstraction identification using LLM or Roslyn

                progressReporter.UpdateProgress(70, $"Identified {abstractions.Count} abstractions");

                // Analyze relationships
                progressReporter.UpdateProgress(80, "Analyzing relationships...");
                var relationships = new List<RelationshipDiagram>();
                // TODO: Implement relationship analysis

                progressReporter.UpdateProgress(90, $"Found {relationships.Count} relationships");

                // Generate tutorial
                progressReporter.UpdateProgress(95, "Generating tutorial...");
                var tutorialGenerator = new TutorialFileWriter();
                var chapters = await tutorialGenerator.GenerateTutorialAsync(abstractions, relationships);

                string tutorialContent;
                if (options.OutputFormat?.ToLower() == "html")
                {
                    tutorialContent = await tutorialGenerator.GenerateHtmlTutorialAsync(chapters);
                }
                else
                {
                    tutorialContent = await tutorialGenerator.GenerateMarkdownTutorialAsync(chapters);
                }

                // Save tutorial
                var outputPath = options.OutputPath ?? $"tutorial.{(options.OutputFormat?.ToLower() == "html" ? "html" : "md")}";
                await tutorialGenerator.SaveTutorialAsync(tutorialContent, outputPath, options.OutputFormat ?? "markdown");

                progressReporter.CompleteProgress($"Tutorial generated successfully: {outputPath}");
                return 0;
            }
            catch (Exception ex)
            {
                progressReporter.ReportError($"Error generating tutorial: {ex.Message}");
                return 1;
            }
        }
    }
}
