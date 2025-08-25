# DotNet Tutorial Generator

A tool that analyzes .NET codebases and generates beginner-friendly tutorials explaining how the code works. Supports both GitHub repositories and local directories as input sources.

## Features

- **Multi-source Analysis**: Analyze code from GitHub repositories or local directories
- **.NET-Specific Understanding**: Deep understanding of .NET patterns and practices
- **LLM Integration**: Uses OpenAI GPT-4, Claude, or LM Studio for semantic analysis
- **Clean Architecture**: Well-structured codebase following clean architecture principles
- **Multiple Interfaces**: RESTful API, CLI, and Web UI interfaces
- **Docker Support**: Easy deployment with Docker containers
- **Comprehensive Testing**: Unit and integration tests included
- **Advanced .NET Analysis**: NuGet package analysis, configuration pattern detection, middleware pipeline mapping
- **Caching Layer**: LLM response caching for improved performance
- **Progress Tracking**: Real-time progress reporting for long-running operations
- **Enhanced Error Handling**: Custom exceptions with context and retry policies
- **Metrics Collection**: Performance and usage monitoring
- **Web UI**: Blazor Server web interface for easy tutorial generation and management

## Architecture

The application follows a clean architecture pattern with the following layers:

- **Core**: Domain models and interfaces
- **Infrastructure**: Implementation of core interfaces
- **API**: RESTful web API
- **Console**: Command-line interface
- **Blazor**: Web user interface

### Project Structure

```
DotNetTutorialGenerator/
├── src/
│   ├── DotNetTutorialGenerator.Core/          # Core domain models and interfaces
│   ├── DotNetTutorialGenerator.Infrastructure/ # Implementation of core interfaces
│   │   ├── Caching/                           # LLM response caching
│   │   ├── FileSystem/                        # Local file system operations
│   │   ├── GitHub/                            # GitHub repository integration
│   │   ├── LLM/                               # LLM service implementations
│   │   ├── Monitoring/                        # Performance metrics collection
│   │   ├── Persistence/                       # Data persistence (file writing)
│   │   ├── Roslyn/                            # .NET code analysis using Roslyn
│   │   └── Services/                          # Core service implementations
│   ├── DotNetTutorialGenerator.Api/           # RESTful web API
│   ├── DotNetTutorialGenerator.Console/       # Command-line interface
│   └── DotNetTutorialGenerator.Blazor/        # Blazor web UI
├── tests/                                     # Unit and integration tests
├── samples/                                   # Sample projects for testing
├── output/                                    # Generated tutorial output
├── docs/                                      # Documentation
└── docker/                                    # Docker configuration
```

## Technologies

- **.NET 9.0**: Latest .NET framework for performance and features
- **ASP.NET Core**: Web API and Blazor Server implementation
- **Roslyn**: .NET Compiler Platform for code analysis
- **System.CommandLine**: Modern command-line parsing
- **Blazor Server**: Interactive web UI with real-time updates
- **SignalR**: Real-time communication for progress tracking
- **Docker**: Containerization for easy deployment
- **Serilog**: Structured logging
- **Swashbuckle**: API documentation with Swagger
- **Octokit**: GitHub API client

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop) (optional, for containerized deployment)
- OpenAI, Claude, or LM Studio API key (for LLM features)

## Getting Started

### Building from Source

```bash
git clone <repository-url>
cd DotNetTutorialGenerator
dotnet build
```

### Running with Docker

```bash
# Build and run the API
docker-compose up -d api

# Run the console application
docker run --rm -v $(pwd)/samples:/app/samples dotnettutorialgenerator-console generate --dir /app/samples/SampleWebApi --output /app/samples/output/tutorial.md

# Run the Blazor web UI
docker-compose up -d blazor
```

### Running Locally

```bash
# Run the API
dotnet run --project src/DotNetTutorialGenerator.Api

# Run the console application
dotnet run --project src/DotNetTutorialGenerator.Console -- generate --dir ./samples/SampleWebApi --output ./samples/output/tutorial.md

# Run the Blazor web UI
dotnet run --project src/DotNetTutorialGenerator.Blazor
```

## Usage

### CLI Interface

```bash
# Generate tutorial from local directory
dotnet-tutorial-gen generate --dir ./MyProject --output ./tutorials

# Generate tutorial from GitHub repository
dotnet-tutorial-gen generate --repo https://github.com/user/repo --output ./tutorials

# Analyze a codebase without generating tutorial
dotnet-tutorial-gen analyze --dir ./MyProject --detailed

# Validate a codebase for tutorial generation
dotnet-tutorial-gen validate --dir ./MyProject
```

### API Interface

```bash
# Generate tutorial from GitHub repository
curl -X POST http://localhost:8080/api/tutorials/generate \
  -H "Content-Type: application/json" \
  -d '{
    "repositoryUrl": "https://github.com/dotnet/samples",
    "llmProvider": "openai",
    "outputFormat": "markdown"
  }'

# Check tutorial generation status
curl -X GET http://localhost:8080/api/tutorials/{id}/status

# Download generated tutorial
curl -X GET http://localhost:8080/api/tutorials/{id}/download -o tutorial.md
```

### Web UI

The Blazor web UI provides a user-friendly interface for generating tutorials:

1. Run the Blazor project:
   ```bash
   dotnet run --project src/DotNetTutorialGenerator.Blazor
   ```
2. Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
3. Use the web interface to generate tutorials from GitHub repositories or local files

Features include:
- Real-time progress tracking during tutorial generation
- File upload via drag-and-drop or GitHub URL
- Tutorial history and management
- Syntax-highlighted code display
- Architecture diagram visualization

## Configuration

The application can be configured through:

1. **appsettings.json** files
2. **Environment variables**
3. **Command-line arguments**

Key configuration options include:
- LLM provider and settings
- GitHub API token
- Crawl options (file patterns, size limits)
- Output formats

### LLM Configuration

Configure your LLM provider in `appsettings.json`:

```json
{
  "LLMSettings": {
    "Provider": "openai",  // openai, claude, or lmstudio
    "ApiKey": "your-api-key",
    "BaseUrl": "http://localhost:1234/v1"  // For LM Studio
  }
}
```

### GitHub Configuration

To analyze GitHub repositories, configure your GitHub token:

```json
{
  "GitHub": {
    "Token": "your-github-token"
  }
}
```

## Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/DotNetTutorialGenerator.Core.Tests
```

## Documentation

- [Design Document](docs/design.md)
- [API Documentation](docs/api-documentation.md)
- [.NET 9.0 Migration Summary](docs/migration-summary.md)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.