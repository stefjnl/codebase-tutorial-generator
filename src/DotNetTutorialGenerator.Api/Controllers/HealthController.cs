using Microsoft.AspNetCore.Mvc;

namespace DotNetTutorialGenerator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "DotNetTutorialGenerator API"
            });
        }
    }
}
