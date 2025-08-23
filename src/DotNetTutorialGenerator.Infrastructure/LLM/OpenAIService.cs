using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Infrastructure.LLM
{
    public class OpenAIService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly int _maxTokens;
        private readonly double _temperature;

        public OpenAIService(string apiKey, string model = "gpt-4-turbo", int maxTokens = 4000, double temperature = 0.3)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _model = model;
            _maxTokens = maxTokens;
            _temperature = temperature;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 1000)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = maxTokens,
                temperature = _temperature
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    return choices[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<List<Abstraction>> IdentifyAbstractionsAsync(List<CodeFile> codeFiles)
        {
            // For simplicity, we're analyzing the first code file
            // In a real implementation, you might want to analyze all files or use a more sophisticated approach
            if (codeFiles.Count == 0)
                return new List<Abstraction>();

            var prompt = Prompts.AbstractionIdentificationPrompt.GeneratePrompt(codeFiles[0].Content);
            var response = await GenerateTextAsync(prompt);

            // In a real implementation, you would parse the JSON response
            // For now, we'll return an empty list
            return new List<Abstraction>();
        }

        public async Task<List<RelationshipDiagram>> AnalyzeRelationshipsAsync(List<Abstraction> abstractions, List<CodeFile> codeFiles)
        {
            var abstractionsJson = JsonSerializer.Serialize(abstractions);
            var prompt = Prompts.RelationshipAnalysisPrompt.GeneratePrompt(abstractionsJson);
            var response = await GenerateTextAsync(prompt);

            // In a real implementation, you would parse the JSON response
            // For now, we'll return an empty list
            return new List<RelationshipDiagram>();
        }

        public async Task<string> GenerateTutorialContentAsync(List<Abstraction> abstractions, List<RelationshipDiagram> diagrams)
        {
            var abstractionsJson = JsonSerializer.Serialize(abstractions);
            var diagramsJson = JsonSerializer.Serialize(diagrams);
            var prompt = Prompts.TutorialGenerationPrompt.GeneratePrompt(abstractionsJson, diagramsJson);
            return await GenerateTextAsync(prompt);
        }

        public async Task<T> GenerateStructuredResponseAsync<T>(string prompt, int maxTokens = 1000)
        {
            var response = await GenerateTextAsync(prompt, maxTokens);
            try
            {
                return JsonSerializer.Deserialize<T>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
