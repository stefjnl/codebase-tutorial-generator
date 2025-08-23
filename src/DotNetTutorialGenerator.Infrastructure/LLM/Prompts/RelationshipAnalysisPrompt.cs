namespace DotNetTutorialGenerator.Infrastructure.LLM.Prompts
{
    public static class RelationshipAnalysisPrompt
    {
        public static string GeneratePrompt(string abstractionsJson)
        {
            return $@"
You are a .NET architecture expert. Analyze the following abstractions and identify the relationships between them.

For each relationship, provide:
1. Source abstraction name
2. Target abstraction name
3. Type of relationship (Inheritance, Implementation, Dependency, etc.)
4. Description of the relationship

Abstractions to analyze:
{abstractionsJson}

Respond in JSON format with an array of relationship objects that can be used to generate a Mermaid diagram.
";
        }
    }
}
