using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using DotNetTutorialGenerator.Cli.Commands;
using DotNetTutorialGenerator.Cli.Options;

namespace DotNetTutorialGenerator.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("DotNet Tutorial Generator - Generate tutorials from .NET codebases")
            {
                new GenerateCommand(),
                new AnalyzeCommand(),
                new ValidateCommand()
            };

            // Add global options
            var verboseOption = new Option<bool>(new[] { "--verbose", "-v" }, "Enable verbose output");
            var logFileOption = new Option<string>(new[] { "--log-file" }, "Log file path");
            var configFileOption = new Option<string>(new[] { "--config" }, "Configuration file path");

            rootCommand.AddGlobalOption(verboseOption);
            rootCommand.AddGlobalOption(logFileOption);
            rootCommand.AddGlobalOption(configFileOption);

            // Create parser
            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build();

            return await parser.InvokeAsync(args);
        }
    }
}
