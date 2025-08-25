using Microsoft.Extensions.Logging;
using System.Net;

namespace DotNetTutorialGenerator.Core.Services
{
    public class RetryPolicy
    {
        private readonly ILogger<RetryPolicy> _logger;

        public RetryPolicy(ILogger<RetryPolicy> logger)
        {
            _logger = logger;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, int maxRetries = 3, TimeSpan? delay = null)
        {
            var retryDelay = delay ?? TimeSpan.FromSeconds(1);
            var exceptions = new List<Exception>();

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (IsTransient(ex) || attempt < maxRetries)
                {
                    exceptions.Add(ex);
                    _logger.LogWarning(ex, "Operation failed on attempt {Attempt} of {MaxRetries}", attempt + 1, maxRetries + 1);

                    if (attempt < maxRetries)
                    {
                        await Task.Delay(retryDelay);
                        retryDelay = TimeSpan.FromTicks(retryDelay.Ticks * 2); // Exponential backoff
                    }
                }
            }

            throw new AggregateException($"Operation failed after {maxRetries + 1} attempts", exceptions);
        }

        public async Task ExecuteAsync(Func<Task> operation, int maxRetries = 3, TimeSpan? delay = null)
        {
            var retryDelay = delay ?? TimeSpan.FromSeconds(1);
            var exceptions = new List<Exception>();

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await operation();
                    return;
                }
                catch (Exception ex) when (IsTransient(ex) || attempt < maxRetries)
                {
                    exceptions.Add(ex);
                    _logger.LogWarning(ex, "Operation failed on attempt {Attempt} of {MaxRetries}", attempt + 1, maxRetries + 1);

                    if (attempt < maxRetries)
                    {
                        await Task.Delay(retryDelay);
                        retryDelay = TimeSpan.FromTicks(retryDelay.Ticks * 2); // Exponential backoff
                    }
                }
            }

            throw new AggregateException($"Operation failed after {maxRetries + 1} attempts", exceptions);
        }

        private static bool IsTransient(Exception ex)
        {
            // Check for common transient exceptions
            return ex is HttpRequestException ||
                   ex is WebException ||
                   ex is TimeoutException ||
                   (ex is InvalidOperationException && ex.Message.Contains("connection"));
        }
    }
}
