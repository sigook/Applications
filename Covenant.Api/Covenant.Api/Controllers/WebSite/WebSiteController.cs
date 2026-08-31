using Covenant.Api.Authorization;
using Covenant.Api.Utils.Extensions;
using Covenant.Core.BL.Extensions;
using Covenant.Core.BL.Interfaces;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Controllers.WebSite;

[Route("api/[controller]")]
[ApiController]
public class WebSiteController(
    ISigookBusClient client,
    IRazorViewToStringRenderer razorViewToStringRenderer,
    IEmailService emailService,
    IRequestRepository requestRepository,
    IMemoryCache memoryCache,
    IOptions<ServiceBusConfiguration> options,
    IFilesContainer filesContainer,
    ICandidateService candidateService) : ControllerBase
{
    private readonly ISigookBusClient client = client;
    private readonly IRazorViewToStringRenderer razorViewToStringRenderer = razorViewToStringRenderer;
    private readonly IEmailService emailService = emailService;
    private readonly IRequestRepository requestRepository = requestRepository;
    private readonly IMemoryCache memoryCache = memoryCache;
    private readonly ServiceBusConfiguration serviceBusConfiguration = options.Value;
    private readonly IFilesContainer filesContainer = filesContainer;
    private readonly ICandidateService candidateService = candidateService;

    /// <summary>Sends a contact-form email from the public website.</summary>
    /// <param name="contact">Contact form data.</param>
    [HttpPost("contact")]
    [ServiceFilter(typeof(CaptchaFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendEmail([FromBody] ContactDto contact)
    {
        try
        {
            var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Website/Contact/Detail.cshtml", contact);
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

    /// <summary>Applies a candidate to a request from an invitation email (anonymous).</summary>
    [HttpPost("candidate/{candidateId:guid}/{requestId:guid}/apply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CandidateApply([FromRoute] Guid candidateId, [FromRoute] Guid requestId)
    {
        var result = await candidateService.Apply(candidateId, requestId);
        if (!result) return BadRequest(ModelState.AddErrors(result.Errors));
        return Ok(result.Value);
    }
}
