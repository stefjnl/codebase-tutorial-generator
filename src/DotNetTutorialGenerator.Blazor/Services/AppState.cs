using Microsoft.AspNetCore.Components;

namespace DotNetTutorialGenerator.Blazor.Services
{
    public interface IAppState
    {
        event Action? OnNotificationChanged;
        string NotificationMessage { get; }
        NotificationType NotificationType { get; }
        bool IsNotificationVisible { get; }

        void SetNotification(string message, NotificationType type);
        void ClearNotification();
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class AppState : IAppState
    {
        public event Action? OnNotificationChanged;

        public string NotificationMessage { get; private set; } = "";
        public NotificationType NotificationType { get; private set; } = NotificationType.Info;
        public bool IsNotificationVisible { get; private set; } = false;

        public void SetNotification(string message, NotificationType type)
        {
            NotificationMessage = message;
            NotificationType = type;
            IsNotificationVisible = true;
            OnNotificationChanged?.Invoke();
        }

        public void ClearNotification()
        {
            IsNotificationVisible = false;
            OnNotificationChanged?.Invoke();
        }
    }
}
