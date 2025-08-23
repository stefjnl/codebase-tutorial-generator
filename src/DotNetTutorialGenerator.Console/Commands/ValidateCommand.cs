using DotNetTutorialGenerator.Console.Options;
using DotNetTutorialGenerator.Console.Utilities;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using System.CommandLine;

namespace DotNetTutorialGenerator.Console.Commands
{
    public class ValidateCommand : Command
    {
        public ValidateCommand() : base("validate", "Validate a .NET codebase for tutorial generation")
        {
            AddOption(new Option<string>(new[] { "--repo", "-r" }, "GitHub repository URL to validate"));
            AddOption(new Option<string>(new[] { "--dir", "-d" }, "Local directory path to validate"));
            AddOption(new Option<string[]>(new[] { "--include" }, "File patterns to include"));
            AddOption(new Option<string[]>(new[] { "--exclude" }, "File patterns to exclude"));

            Handler = CommandHandler.Create<ValidateOptions, GlobalOptions>(HandleCommand);
        }

        private static async Task<int> HandleCommand(ValidateOptions options, GlobalOptions globalOptions)
        {
            var logger = new ConsoleLogger(globalOptions.Verbose, globalOptions.LogFile);
            var progressReporter = new ProgressReporter(logger);

            try
            {
                progressReporter.StartProgress("Starting validation...");

                // Validate input
                if (string.IsNullOrEmpty(options.RepositoryUrl) && string.IsNullOrEmpty(options.LocalPath))
                {
                    progressReporter.ReportError("Either --repo or --dir must be specified");
                    return 1;
                }

                // Create crawl options
                var crawlOptions = new Core.Models.CrawlOptions
                {
                    IncludePatterns = options.IncludePatterns?.ToList() ?? new List<string> { "*.cs", "*.csproj", "*.sln" },
                    ExcludePatterns = options.ExcludePatterns?.ToList() ?? new List<string> { "*bin/*", "*obj/*", "*test*" }
                };

                // Validate the codebase
                progressReporter.UpdateProgress(30, "Validating codebase...");
                bool isValid = true;
                string validationMessage = "";

                if (!string.IsNullOrEmpty(options.RepositoryUrl))
                {
                    // TODO: Implement GitHub validation
                    logger.LogWarning("GitHub validation not yet implemented.");
                    isValid = false;
                    validationMessage = "GitHub validation not yet implemented";
                }
                else if (!string.IsNullOrEmpty(options.LocalPath))
                {
                    if (!Directory.Exists(options.LocalPath))
                    {
                        isValid = false;
                        validationMessage = $"Directory does not exist: {options.LocalPath}";
                    }
                    else
                    {
                        var crawler = new LocalRepositoryCrawler();
                        var codeFiles = await crawler.CrawlRepositoryAsync(options.LocalPath, crawlOptions);

                        if (codeFiles.Count == 0)
                        {
                            isValid = false;
                            validationMessage = "No .NET code files found in the directory";
                        }
                        else
                        {
                            isValid = true;
                            validationMessage = $"Found {codeFiles.Count} .NET code files";
                        }
                    }
                }
                else
                {
                    progressReporter.ReportError("No valid source specified");
                    return 1;
                }

                // Display results
                if (isValid)
                {
                    progressReporter.CompleteProgress($"Validation successful: {validationMessage}");
                    return 0;
                }
                else
                {
                    progressReporter.ReportError($"Validation failed: {validationMessage}");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                progressReporter.ReportError($"Error validating codebase: {ex.Message}");
                return 1;
            }
        }
    }

    public class ValidateOptions
    {
        public string? RepositoryUrl { get; set; }
        public string? LocalPath { get; set; }
        public string[]? IncludePatterns { get; set; }
        public string[]? ExcludePatterns { get; set; }
    }
}
