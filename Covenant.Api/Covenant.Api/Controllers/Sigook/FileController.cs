using Covenant.Api.Common;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Controllers.Sigook
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly FilesConfiguration filesConfiguration;
        private readonly ILogger<FileController> _logger;
        private readonly IFilesContainer filesContainer;

        public FileController(IWebHostEnvironment environment,
            IConfiguration configuration,
            IOptions<FilesConfiguration> options,
            ILogger<FileController> logger,
            IFilesContainer filesContainer)
        {
            _environment = environment;
            _configuration = configuration;
            filesConfiguration = options.Value;
            _logger = logger;
            this.filesContainer = filesContainer;
        }

        [HttpGet]
        [Route("defaultImage/{id}")]
        [ResponseCache(Duration = 86400)]
        public IActionResult DefaultImage(string id)
        {
            string path = Path.Combine(_environment.WebRootPath, "assets", "images", "default-dev-image.png");
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            return PhysicalFile(path, "image/png");
        }

        [HttpPost]
        [Route("imageProfile")]
        public async Task<IActionResult> ImageProfile(IList<IFormFile> files, [FromQuery] SigookFileOptions options)
        {
            try
            {
                long size = files.Sum(c => c.Length);
                if (size > filesConfiguration.MaximumFileSize) return BadRequest(ModelState.AddError(ApiResources.MaxSizeFileError));
                string[] allowImageFormats = _configuration.GetSection("ImageFormatSupport").Get<string[]>() ?? Array.Empty<string>();
                var fileSaver = new FileSaver(filesConfiguration);
                var result = await fileSaver.SaveImageProfile(files, allowImageFormats, options, async (path, contentType) => await filesContainer.Upload(path, contentType));
                if (result) return Ok(result.Value);
                return BadRequest(ModelState.AddErrors(result.Errors));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ModelState.AddError("We could not save your files"));
            }
        }

        [HttpPost]
        [Route("document")]
        [Route("image")]
        public async Task<IActionResult> PostDocuments(IList<IFormFile> files, [FromQuery] SigookFileOptions options)
        {
            try
            {
                long size = files.Sum(c => c.Length);
                if (size > filesConfiguration.MaximumFileSize) return BadRequest(ModelState.AddError(ApiResources.MaxSizeFileError));
                string[] allowFormats = _configuration.GetSection("DocumentFormatSupport").Get<string[]>() ?? Array.Empty<string>();
                var fileSaver = new FileSaver(filesConfiguration);
                var result = await fileSaver.SaveFiles(files, allowFormats, options, async (path, contentType) => await filesContainer.Upload(path, contentType));
                if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
                return Ok(result.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ModelState.AddError("We could not save your files"));
            }
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] string fileName)
        {
            await filesContainer.DeleteFileIfExists(fileName);
            return Ok();
        }
    }
}