using System.Text;

namespace SampleConsoleApp.Services
{
    public class FileService : IFileService
    {
        private readonly IMessageService _messageService;

        public FileService(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void ProcessFile(string fileName)
        {
            _messageService.DisplayMessage($"Processing file: {fileName}");

            // Check if file exists
            if (!File.Exists(fileName))
            {
                _messageService.DisplayMessage($"File {fileName} does not exist. Creating it.");
                WriteFileContent(fileName, "This is a sample file created by the Sample Console Application.");
            }

            var content = ReadFileContent(fileName);
            _messageService.DisplayMessage($"File content: {content}");
        }

        public string ReadFileContent(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return string.Empty;
            }

            return File.ReadAllText(fileName, Encoding.UTF8);
        }

        public void WriteFileContent(string fileName, string content)
        {
            File.WriteAllText(fileName, content, Encoding.UTF8);
            _messageService.DisplayMessage($"Content written to {fileName}");
        }
    }
}
