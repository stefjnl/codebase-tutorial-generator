namespace DotNetTutorialGenerator.Api.DTOs
{
    public class ValidateRepositoryResponse
    {
        public bool Valid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
