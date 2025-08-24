using DotNetTutorialGenerator.Core.Interfaces;
using DotNetTutorialGenerator.Blazor.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace DotNetTutorialGenerator.Blazor.Services
{
    public interface ITutorialGenerationService
    {
        Task<int> StartGenerationAsync(string source, TutorialGenerationOptions options);
        Task<GenerationProgress> GetProgressAsync(int tutorialId);
        Task CancelGenerationAsync(int tutorialId);
    }

    public class TutorialGenerationService : ITutorialGenerationService
    {
        private readonly ITutorialOrchestrator _tutorialOrchestrator;
        private readonly IHubContext<TutorialProgressHub> _hubContext;
        private readonly ConcurrentDictionary<int, GenerationProgress> _progressTracker;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _cancellationTokenSources;

        public TutorialGenerationService(
            ITutorialOrchestrator tutorialOrchestrator,
            IHubContext<TutorialProgressHub> hubContext)
        {
            _tutorialOrchestrator = tutorialOrchestrator;
            _hubContext = hubContext;
            _progressTracker = new ConcurrentDictionary<int, GenerationProgress>();
            _cancellationTokenSources = new ConcurrentDictionary<int, CancellationTokenSource>();
        }

        public async Task<int> StartGenerationAsync(string source, TutorialGenerationOptions options)
        {
            var tutorialId = new Random().Next(1000, 9999);
            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSources[tutorialId] = cancellationTokenSource;

            // Initialize progress
            var initialProgress = new GenerationProgress
            {
                Percentage = 0,
                CurrentPhase = "Starting tutorial generation",
                Message = "Initializing tutorial generation process"
            };
            _progressTracker[tutorialId] = initialProgress;

            // Send initial progress update via SignalR
            await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                .SendAsync("ReceiveProgressUpdate", initialProgress);

            // Start actual tutorial generation
            _ = Task.Run(async () =>
            {
                try
                {
                    var progressReporter = new Progress<GenerationProgress>(async progress =>
                    {
                        _progressTracker[tutorialId] = progress;
                        await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                            .SendAsync("ReceiveProgressUpdate", progress);
                        await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                            .SendAsync("ReceiveLogMessage", progress.Message);
                    });

                    var result = await _tutorialOrchestrator.GenerateTutorialAsync(
                        source,
                        options,
                        progressReporter,
                        cancellationTokenSource.Token);

                    // Send completion
                    await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                        .SendAsync("ReceiveCompletion", $"/tutorials/tutorial-{tutorialId}.md");

                    // Update final progress
                    var finalProgress = new GenerationProgress
                    {
                        Percentage = 100,
                        CurrentPhase = "Completed",
                        Message = result
                    };
                    _progressTracker[tutorialId] = finalProgress;
                }
                catch (OperationCanceledException)
                {
                    var cancelProgress = new GenerationProgress
                    {
                        Percentage = 0,
                        CurrentPhase = "Cancelled",
                        Message = "Tutorial generation was cancelled"
                    };
                    _progressTracker[tutorialId] = cancelProgress;
                    await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                        .SendAsync("ReceiveError", "Tutorial generation was cancelled");
                }
                catch (Exception ex)
                {
                    var errorProgress = new GenerationProgress
                    {
                        Percentage = -1,
                        CurrentPhase = "Error",
                        Message = $"Error during tutorial generation: {ex.Message}"
                    };
                    _progressTracker[tutorialId] = errorProgress;
                    await _hubContext.Clients.Group($"tutorial-{tutorialId}")
                        .SendAsync("ReceiveError", ex.Message);
                }
                finally
                {
                    _cancellationTokenSources.TryRemove(tutorialId, out _);
                }
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

        public async Task CancelGenerationAsync(int tutorialId)
        {
            if (_cancellationTokenSources.TryGetValue(tutorialId, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
            }
        }
    }
}
