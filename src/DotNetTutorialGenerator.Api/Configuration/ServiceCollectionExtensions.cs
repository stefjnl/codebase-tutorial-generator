using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Infrastructure.FileSystem;
using DotNetTutorialGenerator.Infrastructure.GitHub;
using DotNetTutorialGenerator.Infrastructure.LLM;
using DotNetTutorialGenerator.Infrastructure.Persistence;
using DotNetTutorialGenerator.Infrastructure.Roslyn;
using Microsoft.Extensions.DependencyInjection;

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

            // Register HTTP client for GitHub API
            services.AddHttpClient<GitHubApiClient>();

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
