using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using DotNetTutorialGenerator.Api.DTOs;

namespace DotNetTutorialGenerator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutorialController : ControllerBase
    {
        private static readonly Dictionary<string, TutorialStatus> _tutorialStatuses = new();
        private static readonly Dictionary<string, TutorialResponse> _tutorials = new();

        [HttpPost("generate")]
        public async Task<ActionResult<TutorialResponse>> GenerateTutorial([FromBody] GenerateTutorialRequest request)
        {
            // Validate request
            if (string.IsNullOrEmpty(request.RepositoryUrl) && string.IsNullOrEmpty(request.LocalPath))
            {
                return BadRequest("Either RepositoryUrl or LocalPath must be provided");
            }

            var tutorialId = Guid.NewGuid().ToString();

            // Create initial status
            var status = new TutorialStatus
            {
                TutorialId = tutorialId,
                Status = "Processing",
                Progress = 0,
                Message = "Starting tutorial generation",
                CreatedAt = DateTime.UtcNow
            };

            _tutorialStatuses[tutorialId] = status;

            // In a real implementation, you would start the actual tutorial generation process here
            // For now, we'll simulate the process
            _ = Task.Run(async () =>
            {
                try
                {
                    // Simulate processing steps
                    for (int i = 0; i <= 10; i++)
                    {
                        await Task.Delay(1000); // Simulate work
                        status.Progress = i * 10;
                        status.Message = $"Processing step {i} of 10";
                    }

                    // Create a mock tutorial response
                    var tutorialResponse = new TutorialResponse
                    {
                        TutorialId = tutorialId,
                        Title = "Sample Tutorial",
                        Content = "# Sample Tutorial\n\nThis is a sample tutorial generated from the codebase.",
                        Format = request.OutputFormat,
                        GeneratedAt = DateTime.UtcNow,
                        DownloadUrl = $"/api/tutorials/{tutorialId}/download"
                    };

                    _tutorials[tutorialId] = tutorialResponse;
                    status.Status = "Completed";
                    status.Message = "Tutorial generation completed successfully";
                    status.CompletedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    status.Status = "Failed";
                    status.Message = $"Tutorial generation failed: {ex.Message}";
                    status.CompletedAt = DateTime.UtcNow;
                }
            });

            return AcceptedAtAction(nameof(GetTutorialStatus), new { id = tutorialId }, status);
        }

        [HttpGet("{id}/status")]
        public ActionResult<TutorialStatus> GetTutorialStatus([Required] string id)
        {
            if (!_tutorialStatuses.ContainsKey(id))
            {
                return NotFound("Tutorial not found");
            }

            return _tutorialStatuses[id];
        }

        [HttpGet("{id}/download")]
        public ActionResult GetTutorialDownload([Required] string id)
        {
            if (!_tutorials.ContainsKey(id))
            {
                return NotFound("Tutorial not found");
            }

            var tutorial = _tutorials[id];
            var contentType = tutorial.Format.ToLower() switch
            {
                "html" => "text/html",
                "pdf" => "application/pdf",
                _ => "text/markdown"
            };

            return Content(tutorial.Content, contentType);
        }
    }
}
