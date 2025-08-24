# DotNet Tutorial Generator - API Documentation

## Overview

The DotNet Tutorial Generator API provides endpoints for generating tutorials from .NET codebases. The API follows REST principles and returns JSON responses. The application has been migrated to .NET 9.0 to take advantage of the latest features and performance improvements.

## Base URL

```
http://localhost:8080/api
```

## Authentication

API endpoints do not require authentication, but some operations may require configuration of external services (like GitHub or LLM providers) through environment variables.

## Endpoints

### Health Check

#### `GET /health`

Check if the API is running and healthy.

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2023-01-01T00:00:00Z"
}
```

**Response Codes:**
- `200 OK` - API is healthy
- `503 Service Unavailable` - API is unhealthy

### Tutorial Generation

#### `POST /tutorials/generate`

Start generating a tutorial from a .NET codebase.

**Request Body:**
```json
{
  "repositoryUrl": "https://github.com/user/repo",
  "localPath": "/path/to/local/code",
  "llmProvider": "openai",
  "outputFormat": "markdown",
  "generateDiagrams": true,
  "maxAbstractions": 10,
  "includePatterns": ["*.cs", "*.csproj"],
  "excludePatterns": ["*test*", "*bin*", "*obj*"]
}
```

**Request Fields:**
- `repositoryUrl` (string, optional): GitHub repository URL to analyze
- `localPath` (string, optional): Local directory path to analyze
- `llmProvider` (string, optional): LLM provider to use (openai, claude). Default: "openai"
- `outputFormat` (string, optional): Output format (markdown, html). Default: "markdown"
- `generateDiagrams` (boolean, optional): Generate Mermaid diagrams. Default: true
- `maxAbstractions` (integer, optional): Maximum number of abstractions to identify. Default: 10
- `includePatterns` (array of strings, optional): File patterns to include. Default: ["*.cs", "*.csproj", "*.sln"]
- `excludePatterns` (array of strings, optional): File patterns to exclude. Default: ["*bin/*", "*obj/*", "*test*"]

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "Processing",
  "message": "Tutorial generation started"
}
```

**Response Codes:**
- `202 Accepted` - Tutorial generation started
- `400 Bad Request` - Invalid request data
- `500 Internal Server Error` - Server error

#### `GET /tutorials/{id}/status`

Check the status of a tutorial generation process.

**Parameters:**
- `id` (string, required): Tutorial generation ID

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "Completed",
  "progress": 100,
  "message": "Tutorial generation completed successfully"
}
```

**Response Fields:**
- `id` (string): Tutorial generation ID
- `status` (string): Current status (Pending, Processing, Completed, Failed)
- `progress` (integer): Progress percentage (0-100)
- `message` (string): Status message

**Response Codes:**
- `200 OK` - Status retrieved successfully
- `404 Not Found` - Tutorial generation not found
- `500 Internal Server Error` - Server error

#### `GET /tutorials/{id}/download`

Download the generated tutorial.

**Parameters:**
- `id` (string, required): Tutorial generation ID

**Response:**
- Binary content of the generated tutorial file

**Response Codes:**
- `200 OK` - Tutorial downloaded successfully
- `404 Not Found` - Tutorial not found or not completed
- `500 Internal Server Error` - Server error

## Error Responses

All error responses follow this format:

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Error description",
    "details": "Additional error details"
  }
}
```

## Configuration

The API can be configured through environment variables:

- `LLMSettings__Provider` - LLM provider (openai, claude)
- `LLMSettings__ApiKey` - LLM API key
- `LLMSettings__Model` - LLM model to use
- `GitHub__Token` - GitHub API token
- `ASPNETCORE_ENVIRONMENT` - Application environment (Development, Production)

## Examples

### Generate tutorial from GitHub repository

```bash
curl -X POST http://localhost:8080/api/tutorials/generate \
  -H "Content-Type: application/json" \
  -d '{
    "repositoryUrl": "https://github.com/dotnet/samples",
    "llmProvider": "openai",
    "outputFormat": "markdown"
  }'
```

### Check tutorial generation status

```bash
curl -X GET http://localhost:8080/api/tutorials/550e8400-e29b-41d4-a716-446655440000/status
```

### Download generated tutorial

```bash
curl -X GET http://localhost:8080/api/tutorials/550e8400-e29b-41d4-a716-446655440000/download \
  -o tutorial.md
