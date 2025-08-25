using System;
using DotNetTutorialGenerator.Core.Interfaces;

namespace DotNetTutorialGenerator.Infrastructure.LLM
{
    public class LLMServiceFactory
    {
        public static ILLMService CreateService(string provider, string apiKey, string baseUrl = null)
        {
            return provider.ToLowerInvariant() switch
            {
                "openai" => new OpenAIService(apiKey),
                "claude" => new ClaudeService(apiKey),
                "lmstudio" => new LMStudioService(baseUrl ?? "http://localhost:1234/v1"),
                _ => throw new ArgumentException($"Unsupported LLM provider: {provider}")
            };
        }
    }
}
