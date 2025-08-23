using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DotNetTutorialGenerator.Infrastructure.Monitoring
{
    public class MetricsCollector
    {
        private readonly ILogger<MetricsCollector> _logger;
        private readonly Dictionary<string, Stopwatch> _timers;

        public MetricsCollector(ILogger<MetricsCollector> logger)
        {
            _logger = logger;
            _timers = new Dictionary<string, Stopwatch>();
        }

        public void StartTimer(string operation)
        {
            var stopwatch = new Stopwatch();
            _timers[operation] = stopwatch;
            stopwatch.Start();
            _logger.LogDebug("Started timer for operation: {Operation}", operation);
        }

        public TimeSpan StopTimer(string operation)
        {
            if (_timers.TryGetValue(operation, out var stopwatch))
            {
                stopwatch.Stop();
                _timers.Remove(operation);
                _logger.LogDebug("Stopped timer for operation: {Operation}, Duration: {Duration}ms", operation, stopwatch.ElapsedMilliseconds);
                return stopwatch.Elapsed;
            }

            _logger.LogWarning("No timer found for operation: {Operation}", operation);
            return TimeSpan.Zero;
        }

        public void TrackTutorialGeneration(TimeSpan duration, int fileCount)
        {
            _logger.LogInformation("Tutorial generation completed in {Duration}ms for {FileCount} files", duration.TotalMilliseconds, fileCount);
        }

        public void TrackLLMUsage(string provider, int tokens)
        {
            _logger.LogInformation("LLM usage tracked - Provider: {Provider}, Tokens: {Tokens}", provider, tokens);
        }

        public void TrackError(string operation, Exception ex)
        {
            _logger.LogError(ex, "Error occurred during operation: {Operation}", operation);
        }

        public void TrackCacheHit(string cacheKey)
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
        }

        public void TrackCacheMiss(string cacheKey)
        {
            _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
        }
    }
}
