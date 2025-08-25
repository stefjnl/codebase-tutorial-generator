using Microsoft.AspNetCore.SignalR;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Blazor.Hubs
{
    public class TutorialProgressHub : Hub
    {
        public async Task SendProgressUpdate(int tutorialId, GenerationProgress progress)
        {
            await Clients.Group($"tutorial-{tutorialId}").SendAsync("ReceiveProgressUpdate", progress);
        }

        public async Task SendLogMessage(int tutorialId, string message)
        {
            await Clients.Group($"tutorial-{tutorialId}").SendAsync("ReceiveLogMessage", message);
        }

        public async Task SendCompletion(int tutorialId, string tutorialPath)
        {
            await Clients.Group($"tutorial-{tutorialId}").SendAsync("ReceiveCompletion", tutorialPath);
        }

        public async Task SendError(int tutorialId, string errorMessage)
        {
            await Clients.Group($"tutorial-{tutorialId}").SendAsync("ReceiveError", errorMessage);
        }

        public async Task JoinTutorialGroup(int tutorialId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tutorial-{tutorialId}");
        }

        public async Task LeaveTutorialGroup(int tutorialId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tutorial-{tutorialId}");
        }
    }
}
