using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Services;
using DotNetTutorialGenerator.Infrastructure.Caching;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using DotNetTutorialGenerator.Infrastructure.GitHub;
using DotNetTutorialGenerator.Infrastructure.LLM;
using DotNetTutorialGenerator.Infrastructure.Monitoring;
using DotNetTutorialGenerator.Infrastructure.Persistence;
using DotNetTutorialGenerator.Infrastructure.Roslyn;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DotNetTutorialGenerator.Api.Configuration;

namespace DotNetTutorialGenerator.Api.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTutorialGeneratorServices(this IServiceCollection services)
        {
            // Register infrastructure services
            services.AddScoped<IRepositoryCrawler, GitHubService>();
            services.AddScoped<IRepositoryCrawler, LocalRepositoryCrawler>();
            services.AddScoped<IRoslynAnalyzer, RoslynAnalyzer>();
            services.AddScoped<ILLMService, OpenAIService>(); // Default to OpenAI
            services.AddScoped<ITutorialGenerator, TutorialFileWriter>();

            // Register new services
            services.AddScoped<ITutorialValidator, TutorialValidator>();
            services.AddScoped<ILLMResponseCache, LLMResponseCache>();
            services.AddScoped<MetricsCollector>();
            services.AddScoped<RetryPolicy>();

            // Register HTTP client for GitHub API
            services.AddHttpClient<GitHubApiClient>();

            // Register options validators
            services.AddSingleton<IValidateOptions<LLMSettings>, LLMSettingsValidator>();

            return services;
        }

        public static IServiceCollection AddLLMService(this IServiceCollection services, string provider, string apiKey)
        {
            services.AddSingleton<ILLMService>(provider.ToLowerInvariant() switch
            {
                "openai" => new OpenAIService(apiKey),
                "claude" => new ClaudeService(apiKey),
                _ => new OpenAIService(apiKey)
            });

            return services;
        }
    }
}
