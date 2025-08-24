using DotNetTutorialGenerator.Core.Interfaces;

namespace DotNetTutorialGenerator.Infrastructure.Services
{
    public class TutorialOrchestrator : ITutorialOrchestrator
    {
        public async Task<string> GenerateTutorialAsync(string source, TutorialGenerationOptions options, IProgress<GenerationProgress> progress, CancellationToken cancellationToken)
        {
            // Simulate tutorial generation process
            for (int i = 0; i <= 100; i += 10)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                var progressInfo = new GenerationProgress
                {
                    Percentage = i,
                    CurrentPhase = $"Processing phase {i / 10 + 1}",
                    Message = $"Progress: {i}%"
                };

                progress?.Report(progressInfo);
                await Task.Delay(100, cancellationToken);
            }

            return $"Tutorial for {source} generated successfully";
        }

        public async Task<ValidationResult> ValidateSourceAsync(string source, CancellationToken cancellationToken)
        {
            // Simulate source validation
            await Task.Delay(100, cancellationToken);

            return new ValidationResult
            {
                IsValid = true,
                Message = "Source is valid for tutorial generation",
                Issues = new List<string>()
            };
        }
    }
}
