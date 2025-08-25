using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Core.Services
{
    public interface ITutorialValidator
    {
        Task<ValidationResult> ValidateAsync(TutorialChapter chapter);
        Task<ValidationResult> ValidateTutorialStructureAsync(List<TutorialChapter> chapters);
        Task<ValidationResult> ValidateCodeExamplesAsync(TutorialChapter chapter);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();

        public static ValidationResult Success() => new() { IsValid = true };

        public static ValidationResult Failure(string error) => new()
        {
            IsValid = false,
            Errors = new List<string> { error }
        };

        public static ValidationResult Failure(IEnumerable<string> errors) => new()
        {
            IsValid = false,
            Errors = errors.ToList()
        };

        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public void AddSuggestion(string suggestion)
        {
            Suggestions.Add(suggestion);
        }
    }
}
