namespace SampleConsoleApp.Services
{
    public interface IFileService
    {
        void ProcessFile(string fileName);
        string ReadFileContent(string fileName);
        void WriteFileContent(string fileName, string content);
    }
}
