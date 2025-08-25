using DotNetTutorialGenerator.Api.Configuration;
using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Services;
using DotNetTutorialGenerator.Infrastructure.Caching;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using DotNetTutorialGenerator.Infrastructure.GitHub;
using DotNetTutorialGenerator.Infrastructure.LLM;
using DotNetTutorialGenerator.Infrastructure.Monitoring;
using DotNetTutorialGenerator.Infrastructure.Persistence;
using DotNetTutorialGenerator.Infrastructure.Roslyn;
using DotNetTutorialGenerator.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotNetTutorialGenerator.Api.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTutorialGeneratorServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add memory cache for LLM response caching
            services.AddMemoryCache();

            // Register infrastructure services (avoid multiple registrations of same interface)
            services.AddScoped<IRepositoryCrawler>(serviceProvider =>
            {
                // Prefer GitHub service if configured, otherwise use local
                var config = serviceProvider.GetService<IConfiguration>();
                var githubToken = config?.GetValue<string>("GitHub:Token");

                if (!string.IsNullOrEmpty(githubToken))
                {
                    var githubClient = serviceProvider.GetRequiredService<GitHubApiClient>();
                    return new GitHubService(githubClient);
                }
                else
                {
                    return new LocalRepositoryCrawler();
                }
            });

            services.AddScoped<IRoslynAnalyzer, RoslynAnalyzer>();
            services.AddScoped<ITutorialGenerator, TutorialFileWriter>();

            // Register LLM service with configuration
            services.AddScoped<ILLMService>(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var llmSettings = config.GetSection("LLMSettings");
                var provider = llmSettings.GetValue<string>("Provider") ?? "openai";
                var apiKey = llmSettings.GetValue<string>("ApiKey") ?? "";

                return provider.ToLowerInvariant() switch
                {
                    "openai" => new OpenAIService(apiKey),
                    "claude" => new ClaudeService(apiKey),
                    _ => new OpenAIService(apiKey)
                };
            });

            // Register new services
            services.AddScoped<ITutorialValidator, TutorialValidator>();
            services.AddScoped<ILLMResponseCache, LLMResponseCache>();
            services.AddScoped<MetricsCollector>();
            services.AddScoped<RetryPolicy>();

            // Register core services that might be missing
            services.AddScoped<ITutorialOrchestrator, TutorialOrchestrator>();

            // Register HTTP client for GitHub API
            services.AddHttpClient<GitHubApiClient>();

            // Register options validators
            services.AddSingleton<IValidateOptions<LLMSettings>, LLMSettingsValidator>();

            return services;
        }

        public static IServiceCollection AddLLMService(this IServiceCollection services, string provider, string apiKey)
        {
            services.AddScoped<ILLMService>(serviceProvider =>
            {
                return provider.ToLowerInvariant() switch
                {
                    "openai" => new OpenAIService(apiKey),
                    "claude" => new ClaudeService(apiKey),
                    _ => new OpenAIService(apiKey)
                };
            });

            return services;
        }
    }
}