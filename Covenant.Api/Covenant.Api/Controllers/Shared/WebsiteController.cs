using Covenant.Api.Authorization;
using Covenant.Api.Configuration;
using Covenant.Api.Utils.Extensions;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Website;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Candidates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace Covenant.Api.Controllers.Shared;

[Route(RouteName)]
[ApiController]
public class WebsiteController(
    IRazorViewToStringRenderer razorViewToStringRenderer,
    IEmailService emailService,
    IRequestRepository requestRepository,
    IMemoryCache memoryCache,
    ICandidateApplicationService candidateApplicationService,
    IUploadedFilesService uploadedFilesService) : ControllerBase
{
    public const string RouteName = "api/website";

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
        value = value
            .Where(r => model.JobTitle.MatchesSearch(r.Title, r.NumberId))
            .Where(r => model.Location.MatchesSearch(r.Location));
        return Ok(value);
    }

    /// <summary>Receives a candidate application with an optional resume file from the public website.</summary>
    [HttpPost("candidate")]
    [Consumes("multipart/form-data")]
    [EnableRateLimiting(RateLimitingConfiguration.CandidateApplicationPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCandidate()
    {
        var candidate = uploadedFilesService.GetModel<CandidateViewModel>();
        var resumeFile = HttpContext.Request.Form.Files.FirstOrDefault();
        await using var resume = resumeFile?.OpenReadStream();
        var result = await candidateApplicationService.SubmitApplication(candidate, resume, resumeFile?.FileName);
        if (result) return Ok();
        return BadRequest(ModelState.AddErrors(result.Errors));
    }
}
