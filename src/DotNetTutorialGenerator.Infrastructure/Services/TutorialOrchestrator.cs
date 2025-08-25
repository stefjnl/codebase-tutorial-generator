using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Infrastructure.Services
{
    public class TutorialOrchestrator : ITutorialOrchestrator
    {
        private readonly IRepositoryCrawler _repositoryCrawler;
        private readonly IRoslynAnalyzer _roslynAnalyzer;
        private readonly ILLMService _llmService;
        private readonly ITutorialGenerator _tutorialGenerator;

        public TutorialOrchestrator(
            IRepositoryCrawler repositoryCrawler,
            IRoslynAnalyzer roslynAnalyzer,
            ILLMService llmService,
            ITutorialGenerator tutorialGenerator)
        {
            _repositoryCrawler = repositoryCrawler;
            _roslynAnalyzer = roslynAnalyzer;
            _llmService = llmService;
            _tutorialGenerator = tutorialGenerator;
        }

        public async Task<string> GenerateTutorialAsync(string source, TutorialGenerationOptions options, IProgress<Core.Interfaces.GenerationProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                // Report initial progress
                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 0,
                    CurrentPhase = "Starting",
                    Message = "Initializing tutorial generation process"
                });

                // Step 1: Crawl the repository
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException(cancellationToken);

                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 10,
                    CurrentPhase = "Crawling",
                    Message = "Crawling repository files"
                });

                var crawlOptions = new CrawlOptions
                {
                    IncludePatterns = new List<string>(options.IncludePatterns),
                    ExcludePatterns = new List<string>(options.ExcludePatterns)
                };

                var codeFiles = await _repositoryCrawler.CrawlRepositoryAsync(source, crawlOptions);
                
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException(cancellationToken);

                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 20,
                    CurrentPhase = "Analyzing",
                    Message = $"Found {codeFiles.Count} files. Analyzing project structure"
                });

                // Step 2: Analyze with Roslyn
                var abstractions = await _roslynAnalyzer.AnalyzeProjectAsync(source);
                
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException(cancellationToken);

                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 40,
                    CurrentPhase = "Identifying",
                    Message = $"Identified {abstractions.Count} abstractions. Analyzing relationships"
                });

                // Step 3: Analyze relationships
                var diagrams = await _llmService.AnalyzeRelationshipsAsync(abstractions, codeFiles);
                
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException(cancellationToken);

                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 60,
                    CurrentPhase = "Generating",
                    Message = "Generating tutorial content"
                });

                // Step 4: Generate tutorial
                var chapters = await _tutorialGenerator.GenerateTutorialAsync(abstractions, diagrams);
                
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException(cancellationToken);

                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 80,
                    CurrentPhase = "Formatting",
                    Message = "Formatting tutorial content"
                });

                // Step 5: Convert to markdown
                var tutorialContent = await _tutorialGenerator.GenerateMarkdownTutorialAsync(chapters);
                
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException(cancellationToken);

                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 90,
                    CurrentPhase = "Saving",
                    Message = "Saving tutorial to file"
                });

                // Step 6: Save tutorial
                var outputPath = $"tutorial-{DateTime.Now:yyyyMMdd-HHmmss}.md";
                await _tutorialGenerator.SaveTutorialAsync(tutorialContent, outputPath);

                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = 100,
                    CurrentPhase = "Completed",
                    Message = $"Tutorial generated successfully: {outputPath}"
                });

                return tutorialContent;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                progress?.Report(new Core.Interfaces.GenerationProgress
                {
                    Percentage = -1,
                    CurrentPhase = "Error",
                    Message = $"Error during tutorial generation: {ex.Message}"
                });
                
                throw;
            }
        }

        public async Task<ValidationResult> ValidateSourceAsync(string source, CancellationToken cancellationToken)
        {
            try
            {
                var metadata = await _repositoryCrawler.ExtractProjectMetadataAsync(source);
                
                var result = new ValidationResult
                {
                    IsValid = metadata != null && !string.IsNullOrEmpty(metadata.Name),
                    Message = metadata != null ? $"Project '{metadata.Name}' is valid for tutorial generation" : "Invalid project structure",
                    Issues = new List<string>()
                };

                return result;
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"Error validating source: {ex.Message}",
                    Issues = new List<string> { ex.Message }
                };
            }
        }
    }
}
