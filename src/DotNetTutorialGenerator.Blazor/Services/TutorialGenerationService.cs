using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Blazor.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DotNetTutorialGenerator.Blazor.Services
{
    public interface ITutorialGenerationService
    {
        Task<int> StartGenerationAsync(string source, TutorialGenerationOptions options);
        Task<GenerationProgress> GetProgressAsync(int tutorialId);
    }

    public class TutorialGenerationService : ITutorialGenerationService
    {
        private readonly ITutorialOrchestrator _tutorialOrchestrator;
        private readonly IHubContext<TutorialProgressHub> _hubContext;
        private readonly Dictionary<int, GenerationProgress> _progressTracker;

        public TutorialGenerationService(
            ITutorialOrchestrator tutorialOrchestrator,
            IHubContext<TutorialProgressHub> hubContext)
        {
            _tutorialOrchestrator = tutorialOrchestrator;
            _hubContext = hubContext;
            _progressTracker = new Dictionary<int, GenerationProgress>();
        }

        public async Task<int> StartGenerationAsync(string source, TutorialGenerationOptions options)
        {
            // In a real implementation, this would start the tutorial generation process
            // and return a unique tutorial ID
            var tutorialId = new Random().Next(1000, 9999);

            // Simulate progress tracking
            _ = Task.Run(async () =>
            {
                for (int i = 0; i <= 100; i += 10)
                {
                    var progress = new GenerationProgress
                    {
                        Percentage = i,
                        CurrentPhase = $"Processing phase {i / 10 + 1}",
                        Message = $"Progress: {i}%"
                    };

                    _progressTracker[tutorialId] = progress;

                    // Send progress update via SignalR
                    await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                        .SendAsync("ReceiveProgressUpdate", progress);

                    // Send log message
                    await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                        .SendAsync("ReceiveLogMessage", $"Completed {i}% of tutorial generation");

                    await Task.Delay(1000); // Simulate work
                }

                // Send completion
                await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                    .SendAsync("ReceiveCompletion", $"/tutorials/tutorial-{tutorialId}.md");
            });

            return tutorialId;
        }

        public async Task<GenerationProgress> GetProgressAsync(int tutorialId)
        {
            if (_progressTracker.TryGetValue(tutorialId, out var progress))
            {
                return progress;
            }

            return new GenerationProgress
            {
                Percentage = 0,
                CurrentPhase = "Not started",
                Message = "Tutorial generation not started"
            };
        }
    }
}
