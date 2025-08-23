# DotNet Tutorial Generator

A tool that analyzes .NET codebases and generates beginner-friendly tutorials explaining how the code works. Supports both GitHub repositories and local directories as input sources.

## Features

- **Multi-source Analysis**: Analyze code from GitHub repositories or local directories
- **.NET-Specific Understanding**: Deep understanding of .NET patterns and practices
- **LLM Integration**: Uses OpenAI GPT-4, Claude, or LM Studio for semantic analysis
- **Clean Architecture**: Well-structured codebase following clean architecture principles
- **Multiple Interfaces**: Both RESTful API and CLI interfaces
- **Docker Support**: Easy deployment with Docker containers
- **Comprehensive Testing**: Unit and integration tests included
- **Advanced .NET Analysis**: NuGet package analysis, configuration pattern detection, middleware pipeline mapping
- **Caching Layer**: LLM response caching for improved performance
- **Progress Tracking**: Real-time progress reporting for long-running operations
- **Enhanced Error Handling**: Custom exceptions with context and retry policies
- **Metrics Collection**: Performance and usage monitoring

## Architecture

The application follows a clean architecture pattern with the following layers:

- **Core**: Domain models and interfaces
- **Infrastructure**: Implementation of interfaces (GitHub, LLM, FileSystem, Roslyn)
- **API**: RESTful web API
- **Console**: Command-line interface

## Prerequisites

- .NET 8.0 SDK
- Docker (optional, for containerized deployment)
- OpenAI or Claude API key (for LLM features)

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
```

### Running Locally

```bash
# Run the API
dotnet run --project src/DotNetTutorialGenerator.Api

# Run the console application
dotnet run --project src/DotNetTutorialGenerator.Console -- generate --dir ./samples/SampleWebApi --output ./samples/output/tutorial.md
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

## Project Structure

```
DotNetTutorialGenerator/
├── src/
│   ├── DotNetTutorialGenerator.Core/          # Core domain models and interfaces
│   ├── DotNetTutorialGenerator.Infrastructure/ # Implementation of core interfaces
│   ├── DotNetTutorialGenerator.Api/           # RESTful web API
│   └── DotNetTutorialGenerator.Console/       # Command-line interface
├── tests/                                     # Unit and integration tests
├── docker/                                    # Docker configuration
└── docs/                                      # Documentation
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

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
