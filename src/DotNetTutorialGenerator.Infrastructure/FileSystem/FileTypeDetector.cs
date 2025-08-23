using System;
using System.IO;

namespace DotNetTutorialGenerator.Infrastructure.FileSystem
{
    public static class FileTypeDetector
    {
        public static string DetectFileType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".cs" => "C#",
                ".csproj" => "C# Project",
                ".sln" => "Solution",
                ".vb" => "Visual Basic",
                ".vbproj" => "VB.NET Project",
                ".fs" => "F#",
                ".fsproj" => "F# Project",
                ".json" => "JSON",
                ".xml" => "XML",
                ".config" => "Config",
                ".md" => "Markdown",
                ".txt" => "Text",
                ".dll" => "Assembly",
                ".exe" => "Executable",
                ".png" => "Image",
                ".jpg" => "Image",
                ".jpeg" => "Image",
                ".gif" => "Image",
                ".css" => "CSS",
                ".js" => "JavaScript",
                ".ts" => "TypeScript",
                ".html" => "HTML",
                ".htm" => "HTML",
                _ => "Unknown"
            };
        }

        public static bool IsDotNetProjectFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension is ".csproj" or ".vbproj" or ".fsproj";
        }

        public static bool IsDotNetSolutionFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension is ".sln";
        }

        public static bool IsSourceCodeFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension is ".cs" or ".vb" or ".fs";
        }

        public static string GetProjectTypeFromProjectFile(string projectFilePath)
        {
            if (!IsDotNetProjectFile(projectFilePath))
                return "Unknown";

            try
            {
                var content = File.ReadAllText(projectFilePath);

                if (content.Contains("Microsoft.NET.Sdk.Web"))
                    return "Web Application";

                if (content.Contains("Microsoft.NET.Sdk.Worker"))
                    return "Worker Service";

                if (content.Contains("Microsoft.NET.Sdk.Razor"))
                    return "Razor Class Library";

                if (content.Contains("Microsoft.NET.Sdk.BlazorWebAssembly"))
                    return "Blazor WebAssembly";

                return "Class Library";
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }
    }
}
