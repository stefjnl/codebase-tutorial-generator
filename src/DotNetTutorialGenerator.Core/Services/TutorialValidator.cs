using DotNetTutorialGenerator.Core.Models;
using Microsoft.Extensions.Logging;

namespace DotNetTutorialGenerator.Core.Services
{
    public class TutorialValidator : ITutorialValidator
    {
        private readonly ILogger<TutorialValidator> _logger;

        public TutorialValidator(ILogger<TutorialValidator> logger)
        {
            _logger = logger;
        }

        public async Task<ValidationResult> ValidateAsync(TutorialChapter chapter)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate chapter title
            if (string.IsNullOrWhiteSpace(chapter.Title))
            {
                result.AddError("Chapter title is required");
            }

            // Validate content
            if (string.IsNullOrWhiteSpace(chapter.Content))
            {
                result.AddWarning("Chapter content is empty");
            }

            // Validate code examples
            await ValidateCodeExamplesAsync(chapter, result);

            _logger.LogDebug("Validated chapter: {ChapterTitle}", chapter.Title);
            return result;
        }

        public async Task<ValidationResult> ValidateTutorialStructureAsync(List<TutorialChapter> chapters)
        {
            var result = new ValidationResult { IsValid = true };

            if (chapters == null || chapters.Count == 0)
            {
                result.AddError("Tutorial must have at least one chapter");
                return result;
            }

            // Check for duplicate chapter titles
            var duplicateTitles = chapters
                .GroupBy(c => c.Title)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            foreach (var title in duplicateTitles)
            {
                result.AddError($"Duplicate chapter title found: {title}");
            }

            // Validate each chapter
            foreach (var chapter in chapters)
            {
                var chapterResult = await ValidateAsync(chapter);
                if (!chapterResult.IsValid)
                {
                    result.IsValid = false;
                    result.Errors.AddRange(chapterResult.Errors);
                }
                result.Warnings.AddRange(chapterResult.Warnings);
                result.Suggestions.AddRange(chapterResult.Suggestions);
            }

            _logger.LogDebug("Validated tutorial structure with {ChapterCount} chapters", chapters.Count);
            return result;
        }

        public async Task<ValidationResult> ValidateCodeExamplesAsync(TutorialChapter chapter)
        {
            var result = new ValidationResult { IsValid = true };
            await ValidateCodeExamplesAsync(chapter, result);
            return result;
        }

        private async Task ValidateCodeExamplesAsync(TutorialChapter chapter, ValidationResult result)
        {
            // This is a simplified validation
            // In a real implementation, you might want to validate:
            // - Syntax correctness
            // - Completeness of examples
            // - Relevance to chapter content

            // CodeExamples is a List<string> in the current model
            // In a real implementation, you might want to validate the content of each example
            if (chapter.CodeExamples != null && chapter.CodeExamples.Count > 0)
            {
                foreach (var example in chapter.CodeExamples)
                {
                    if (string.IsNullOrWhiteSpace(example))
                    {
                        result.AddWarning("Code example is empty");
                    }
                }
            }

            await Task.CompletedTask; // Placeholder for async operations
        }
    }
}
