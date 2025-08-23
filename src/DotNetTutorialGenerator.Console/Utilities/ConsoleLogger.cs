using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.File;

namespace DotNetTutorialGenerator.Cli.Utilities
{
    public class ConsoleLogger
    {
        private readonly ILogger _logger;

        public ConsoleLogger(bool verbose = false, string? logFile = null)
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(verbose ? LogEventLevel.Debug : LogEventLevel.Information))
                .WriteTo.Console();

            if (!string.IsNullOrEmpty(logFile))
            {
                loggerConfig.WriteTo.File(logFile, rollingInterval: RollingInterval.Day);
            }

            _logger = loggerConfig.CreateLogger();
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                _logger.Error(ex, message);
            }
            else
            {
                _logger.Error(message);
            }
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogVerbose(string message)
        {
            _logger.Verbose(message);
        }
    }
}
