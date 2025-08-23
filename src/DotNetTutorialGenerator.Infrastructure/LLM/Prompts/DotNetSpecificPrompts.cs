namespace DotNetTutorialGenerator.Infrastructure.LLM.Prompts
{
    public static class DotNetSpecificPrompts
    {
        public const string AbstractionIdentification = @"
        Analyze this .NET codebase and identify core abstractions.
        Focus on: Controllers, Services, Models, Repositories, Middleware, etc.
        Consider: Dependency Injection patterns, Entity Framework usage, etc.
        
        Provide your response in the following format:
        ## Abstractions Found
        1. **[Abstraction Type]**: [Name]
           - Purpose: [Brief description]
           - Location: [File path]
           - Dependencies: [List of dependencies]";

        public const string ArchitectureAnalysis = @"
        Identify the architecture pattern used in this .NET codebase: Clean Architecture, MVC, Web API, etc.
        
        Analyze:
        - Project structure and organization
        - Separation of concerns
        - Dependency flow
        - Use of interfaces and dependency injection
        
        Provide your response in the following format:
        ## Architecture Analysis
        **Pattern**: [Identified pattern]
        **Justification**: [Explanation of why this pattern was identified]
        **Strengths**: [List of architectural strengths]
        **Suggestions**: [List of potential improvements]";

        public const string NuGetPackageAnalysis = @"
        Analyze the NuGet packages used in this .NET project.
        
        For each significant package, provide:
        - Package name and version
        - Purpose in this project
        - Key features being utilized
        - Any alternatives that might be considered
        
        Format your response as:
        ## NuGet Package Analysis
        ### [Package Name]
        - Version: [Version number]
        - Purpose: [Explanation]
        - Key Features: [List]
        - Alternatives: [If applicable]";

        public const string ConfigurationPatternDetection = @"
        Identify configuration patterns used in this .NET application.
        
        Look for:
        - IOptions pattern usage
        - IConfiguration usage
        - Environment-specific configurations
        - Custom configuration sections
        
        Format your response as:
        ## Configuration Patterns
        ### [Pattern Name]
        - Implementation: [How it's implemented]
        - Files: [Configuration files used]
        - Benefits: [Advantages in this context]";

        public const string MiddlewarePipelineAnalysis = @"
        Analyze the middleware pipeline in this .NET application.
        
        Identify:
        - Custom middleware implementations
        - Order of middleware registration
        - Purpose of each middleware
        - Potential issues or improvements
        
        Format your response as:
        ## Middleware Pipeline
        1. **[Middleware Name]**
           - Order: [Position in pipeline]
           - Purpose: [What it does]
           - Configuration: [How it's configured]";

        public const string AuthPatternDetection = @"
        Identify authentication and authorization patterns in this .NET application.
        
        Look for:
        - Authentication schemes (JWT, Cookie, etc.)
        - Authorization policies
        - Role-based or claim-based security
        - Custom authentication handlers
        
        Format your response as:
        ## Authentication/Authorization Patterns
        ### [Pattern Type]
        - Implementation: [How it's implemented]
        - Configuration: [Where it's configured]
        - Security Considerations: [Any notes on security]";
    }
}
