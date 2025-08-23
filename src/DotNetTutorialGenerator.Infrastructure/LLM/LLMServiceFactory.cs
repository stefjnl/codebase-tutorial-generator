using System;
using DotNetTutorialGenerator.Core.Interfaces;

namespace DotNetTutorialGenerator.Infrastructure.LLM
{
    public class LLMServiceFactory
    {
        public static ILLMService CreateService(string provider, string apiKey)
        {
            return provider.ToLowerInvariant() switch
            {
                "openai" => new OpenAIService(apiKey),
                "claude" => new ClaudeService(apiKey),
                _ => throw new ArgumentException($"Unsupported LLM provider: {provider}")
            };
        }
    }
}
