using Covenant.Api.Common;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Resources;
using Microsoft.AspNetCore.Http;
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

        /// <summary>Uploads and saves a profile image.</summary>
        /// <param name="files">Image files to upload.</param>
        /// <param name="options">File storage options.</param>
        [HttpPost]
        [Route("imageProfile")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(FilesResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImageProfile(IList<IFormFile> files, [FromQuery] SigookFileOptions options)
        {
            try
            {
                long size = files.Sum(c => c.Length);
                if (size > filesConfiguration.MaximumFileSize) return BadRequest(ModelState.AddError(ApiResources.MaxSizeFileError));
                string[] allowImageFormats = _configuration.GetSection("ImageFormatSupport").Get<string[]>() ?? [];
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

        /// <summary>Uploads and saves one or more documents or images.</summary>
        /// <param name="files">Files to upload.</param>
        /// <param name="options">File storage options.</param>
        [HttpPost]
        [Route("document")]
        [Route("image")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(List<FilesResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostDocuments(IList<IFormFile> files, [FromQuery] SigookFileOptions options)
        {
            try
            {
                long size = files.Sum(c => c.Length);
                if (size > filesConfiguration.MaximumFileSize) return BadRequest(ModelState.AddError(ApiResources.MaxSizeFileError));
                string[] allowFormats = _configuration.GetSection("DocumentFormatSupport").Get<string[]>() ?? [];
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

        /// <summary>Deletes a stored file by name if it exists.</summary>
        /// <param name="fileName">Name of the file to delete.</param>
        [HttpDelete("{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDocument([FromRoute] string fileName)
        {
            await filesContainer.DeleteFileIfExists(fileName);
            return Ok();
        }
    }
}