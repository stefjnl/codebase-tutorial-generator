namespace DotNetTutorialGenerator.Infrastructure.LLM.Prompts
{
    public static class AbstractionIdentificationPrompt
    {
        public static string GeneratePrompt(string codeContent)
        {
            return $@"
You are a .NET architecture expert. Analyze the following C# code and identify the core abstractions (classes, interfaces, controllers, services, etc.) present in it.

For each abstraction, provide:
1. Name: The name of the abstraction
2. Type: The type (Class, Interface, Controller, Service, Model, etc.)
3. Description: A brief description of its purpose
4. RelatedFileIndices: Any related files (empty array for now)

Code to analyze:
{codeContent}

Respond in JSON format with an array of abstraction objects.
";
        }
    }
}
