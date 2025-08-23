using System.CommandLine;

namespace DotNetTutorialGenerator.Cli.Options
{
    public class GlobalOptions
    {
        public bool Verbose { get; set; }
        public string? LogFile { get; set; }
        public string? ConfigFile { get; set; }
    }
}
