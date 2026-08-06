using Microsoft.AspNetCore.Mvc;

namespace Covenant.Api.Controllers.Sigook;

[Route("api/[controller]")]
[ApiController]
public class FileController(IWebHostEnvironment environment) : ControllerBase
{
    private readonly IWebHostEnvironment _environment = environment;

    /// <summary>Returns the default placeholder image.</summary>
    /// <param name="id">Image identifier (unused placeholder route value).</param>
    [HttpGet]
    [Route("defaultImage/{id}")]
    [ResponseCache(Duration = 86400)]
    [Produces("application/octet-stream")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DefaultImage(string id)
    {
        string path = Path.Combine(_environment.WebRootPath, "assets", "images", "default-dev-image.png");
        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }
        return PhysicalFile(path, "image/png");
    }
}
