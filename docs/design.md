# DotNet Tutorial Generator - Design Document

## Overview

The DotNet Tutorial Generator is a tool that analyzes .NET codebases and generates beginner-friendly tutorials explaining how the code works. It supports both GitHub repositories and local directories as input sources. The application has been migrated to .NET 9.0 to take advantage of the latest features and performance improvements.

## Architecture

The application follows a clean architecture pattern with the following layers:

### Core Layer
- **Models**: Domain entities and data transfer objects
- **Interfaces**: Abstractions for all external dependencies
- **Services**: Core business logic

### Infrastructure Layer
- **GitHub**: GitHub API integration
- **LLM**: Large Language Model integration (OpenAI, Claude)
- **FileSystem**: Local file system operations
- **Roslyn**: .NET code analysis using Roslyn
- **Persistence**: Tutorial output generation and storage

### Presentation Layer
- **API**: RESTful web API
- **Console**: Command-line interface

## Key Components

### 1. Code Crawling
The application can crawl code from:
- GitHub repositories
- Local directories

It supports configurable file inclusion/exclusion patterns and size limits.

### 2. Abstraction Identification
Using a combination of:
- Roslyn for static code analysis
- LLM for semantic understanding
- Pattern matching for common .NET constructs

### 3. Relationship Analysis
Analyzes relationships between identified abstractions:
- Inheritance
- Implementation
- Dependencies
- Composition

### 4. Tutorial Generation
Generates structured tutorials with:
- Markdown formatting
- Mermaid diagrams
- Code examples
- Explanations of concepts

## Data Flow

1. **Input**: GitHub URL or local directory path
2. **Crawling**: Extract code files based on configuration
3. **Analysis**: Identify abstractions and relationships
4. **Generation**: Create tutorial content using LLM
5. **Output**: Save tutorial in specified format

## API Endpoints

- `POST /api/tutorials/generate` - Start tutorial generation
- `GET /api/tutorials/{id}/status` - Check generation progress
- `GET /api/tutorials/{id}/download` - Download completed tutorial
- `GET /api/health` - Health check endpoint

## CLI Commands

- `generate` - Generate tutorial from codebase
- `analyze` - Analyze codebase without generating tutorial
- `validate` - Validate codebase for tutorial generation

## Configuration

The application can be configured through:
- appsettings.json files
- Environment variables
- Command-line arguments

Key configuration options include:
- LLM provider and settings
- GitHub API token
- Crawl options (file patterns, size limits)
- Output formats
