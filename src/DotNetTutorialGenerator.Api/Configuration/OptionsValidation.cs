using Microsoft.Extensions.Options;

namespace DotNetTutorialGenerator.Api.Configuration
{
    public class LLMSettings
    {
        public string Provider { get; set; } = "openai";
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-4";
        public string BaseUrl { get; set; } = string.Empty;
        public int MaxTokens { get; set; } = 2000;
        public double Temperature { get; set; } = 0.7;
    }

    public class LLMSettingsValidator : IValidateOptions<LLMSettings>
    {
        public ValidateOptionsResult Validate(string name, LLMSettings options)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(options.Provider))
            {
                errors.Add("LLM Provider must be specified.");
            }
            else if (!new[] { "openai", "claude", "lmstudio" }.Contains(options.Provider.ToLower()))
            {
                errors.Add("LLM Provider must be one of: openai, claude, lmstudio.");
            }

            if (string.IsNullOrWhiteSpace(options.ApiKey) && options.Provider != "lmstudio")
            {
                errors.Add("API Key is required for cloud providers.");
            }

            if (string.IsNullOrWhiteSpace(options.Model))
            {
                errors.Add("Model must be specified.");
            }

            if (options.MaxTokens <= 0)
            {
                errors.Add("MaxTokens must be greater than zero.");
            }

            if (options.Temperature < 0 || options.Temperature > 1)
            {
                errors.Add("Temperature must be between 0 and 1.");
            }

            return errors.Count > 0
                ? ValidateOptionsResult.Fail(errors)
                : ValidateOptionsResult.Success;
        }
    }
}
