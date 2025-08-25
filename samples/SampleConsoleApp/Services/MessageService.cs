namespace SampleConsoleApp.Services
{
    public class MessageService : IMessageService
    {
        public void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to the Sample Console Application!");
            Console.WriteLine("This application demonstrates dependency injection in .NET Console Apps.");
            Console.WriteLine(new string('-', 50));
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }
    }
}
