namespace DotNetTutorialGenerator.Infrastructure.LLM.Prompts
{
    public static class TutorialGenerationPrompt
    {
        public static string GeneratePrompt(string abstractionsJson, string relationshipsJson)
        {
            return $@"
You are a .NET expert creating beginner-friendly tutorials. Using the following abstractions and relationships, create a structured tutorial that explains how a .NET application works.

The tutorial should include:
1. An introduction explaining the overall purpose
2. Individual sections for each major abstraction explaining its role
3. Explanations of how the abstractions work together
4. Code examples where relevant
5. Simple diagrams (in Mermaid syntax) to visualize relationships

Abstractions:
{abstractionsJson}

Relationships:
{relationshipsJson}

Respond in JSON format with an array of tutorial chapter objects, each containing a title, content, and any relevant code examples or diagrams.
";
        }
    }
}
