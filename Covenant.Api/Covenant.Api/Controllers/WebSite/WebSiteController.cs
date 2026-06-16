using Covenant.Api.Authorization;
using Covenant.Api.Common.Models;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.WebSite;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Controllers.WebSite;

[Route("api/[controller]")]
[ApiController]
public class WebSiteController : ControllerBase
{
    private readonly ISigookBusClient client;
    private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
    private readonly IEmailService emailService;
    private readonly IRequestRepository requestRepository;
    private readonly IMemoryCache memoryCache;
    private readonly ServiceBusConfiguration serviceBusConfiguration;
    private readonly IFilesContainer filesContainer;
    private readonly IConfiguration configuration;
    private readonly FilesConfiguration filesConfiguration;
    private readonly ILogger<WebSiteController> logger;

    public WebSiteController(
        ISigookBusClient client,
        IRazorViewToStringRenderer razorViewToStringRenderer,
        IEmailService emailService,
        IRequestRepository requestRepository,
        IMemoryCache memoryCache,
        IOptions<ServiceBusConfiguration> options,
        IFilesContainer filesContainer,
        IConfiguration configuration,
        IOptions<FilesConfiguration> filesOptions,
        ILogger<WebSiteController> logger)
    {
        this.client = client;
        this.razorViewToStringRenderer = razorViewToStringRenderer;
        this.emailService = emailService;
        this.requestRepository = requestRepository;
        this.memoryCache = memoryCache;
        serviceBusConfiguration = options.Value;
        this.filesContainer = filesContainer;
        this.configuration = configuration;
        filesConfiguration = filesOptions.Value;
        this.logger = logger;
    }

    /// <summary>Sends a contact-form email from the public website.</summary>
    /// <param name="contact">Contact form data.</param>
    [HttpPost("contact")]
    [ServiceFilter(typeof(CaptchaFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendEmail([FromBody] ContactDto contact)
    {
        try
        {
            var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/WebSite/Contact/Detail.cshtml", contact);
            await emailService.SendEmail(new EmailParams(string.Empty, contact.Subject, message)
            {
                EmailSettingName = contact.EmailSetting
            });
            return Ok();
        }
        catch (Exception)
        {
            return Ok();
        }
    }

    /// <summary>Gets the available jobs matching the public job search criteria.</summary>
    /// <param name="model">Job search filter criteria.</param>
    [HttpGet("jobs")]
    [ProducesResponseType(typeof(IEnumerable<JobViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobs([FromQuery] JobSearchModel model)
    {
        var value = Enumerable.Empty<JobViewModel>();
        if (memoryCache.TryGetValue(HttpContext.Request.QueryString.Value, out value))
        {
            goto FilterJobs;
        }
        value = await requestRepository.GetAvailableRequest(model.Countries);
        memoryCache.Set(HttpContext.Request.QueryString.Value, value, TimeSpan.FromSeconds(120));
    FilterJobs:
        if (!string.IsNullOrEmpty(model.JobId))
        {
            value = value.Where(r => r.NumberId == model.JobId);
        }
        if (!string.IsNullOrEmpty(model.JobTitle))
        {
            value = value.Where(r => r.Title.Contains(model.JobTitle, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrEmpty(model.Location))
        {
            value = value.Where(r => r.Location.Contains(model.Location, StringComparison.OrdinalIgnoreCase));
        }
        return Ok(value);
    }

    /// <summary>Receives a candidate application with an optional resume file from the public website.</summary>
    [HttpPost("candidate")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCandidate()
    {
        var candidate = HttpContext.Request.Form.DeserializeData<CandidateViewModel>();
        if (HttpContext.Request.Form.Files.Count > 0)
        {
            var resumeFile = HttpContext.Request.Form.Files[0];
            if (resumeFile != null)
            {
                await filesContainer.UploadAsync(resumeFile.OpenReadStream(), candidate.FileName);
            }
        }
        await client.SendMessageAsync(candidate, serviceBusConfiguration.ValidateCandidateQueue);
        return Ok();
    }
}
