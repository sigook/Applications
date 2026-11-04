using System.ComponentModel;
using ModelContextProtocol.Server;
using Sigook.Mcp.Api;
using Sigook.Mcp.Models;

namespace Sigook.Mcp.Tools;

[McpServerToolType]
public static class CandidateTools
{
    [McpServerTool(Name = "create_candidate"), Description("Creates a candidate in Sigook for the current agency, optionally uploading a resume file in the same call. Phone must already be in ### ###-#### format (see normalize_phone). The API rejects duplicated emails and phones; the error text is returned as-is so you can report it. Returns the new candidate id.")]
    public static async Task<object> CreateCandidate(
        ISigookApi api,
        [Description("Full name.")] string name,
        [Description("Phone in ### ###-#### format.")] string phone,
        [Description("Email. Optional but strongly recommended.")] string? email = null,
        [Description("Street address. Optional.")] string? address = null,
        [Description("Postal/zip code. Optional.")] string? postalCode = null,
        [Description("Sigook city id from resolve_city. Optional.")] Guid? cityId = null,
        [Description("City name matching cityId. Optional.")] string? cityName = null,
        [Description("Sigook source id from list_sources. Optional.")] Guid? sourceId = null,
        [Description("Sigook gender id from list_genders. Optional.")] Guid? genderId = null,
        [Description("Whether the candidate has a vehicle. Default false.")] bool hasVehicle = false,
        [Description("Residency/work status free text, e.g. 'Authorized to work in the U.S.'. Optional.")] string? residencyStatus = null,
        [Description("Skills to attach. Optional.")] string[]? skills = null,
        [Description("Local path of the resume (pdf, doc, docx) as returned by read_export. Optional.")] string? resumePath = null,
        CancellationToken cancellationToken = default)
    {
        string? fileName = null;
        if (!string.IsNullOrWhiteSpace(resumePath))
        {
            if (!File.Exists(resumePath)) return new { success = false, error = $"Resume not found: {resumePath}" };
            fileName = $"Resume_{Guid.NewGuid():N}{Path.GetExtension(resumePath)}";
        }
        var model = new CandidateCreateModel(
            name.Trim(),
            Blank(email),
            Blank(address),
            Blank(postalCode),
            cityId.HasValue ? new CityModel(cityId.Value, cityName ?? string.Empty, null, null) : null,
            genderId.HasValue ? new CatalogItem(genderId.Value, string.Empty) : null,
            hasVehicle,
            Blank(residencyStatus),
            sourceId,
            false,
            [new PhoneNumberModel(null, phone.Trim())],
            (skills ?? []).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => new SkillModel(null, s.Trim())),
            fileName);
        var result = await api.CreateCandidate(ApiCalls.ToJson(model), fileName is null ? null : ApiCalls.FilePart(resumePath!, fileName), cancellationToken).AsResult();
        return result.Success
            ? new { success = true, candidateId = result.Value!.Id, resumeUploaded = fileName is not null }
            : new { success = false, statusCode = result.StatusCode, error = result.Error };
    }

    [McpServerTool(Name = "add_candidate_document"), Description("Uploads an extra document (resume, certificate...) to an existing candidate.")]
    public static async Task<object> AddCandidateDocument(
        ISigookApi api,
        [Description("Candidate id.")] Guid candidateId,
        [Description("Local file path (pdf, doc, docx, png, jpg).")] string filePath,
        [Description("Short description, e.g. Resume. Default Resume.")] string description = "Resume",
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath)) return new { success = false, error = $"File not found: {filePath}" };
        var fileName = $"{description.Replace(' ', '_')}_{Guid.NewGuid():N}{Path.GetExtension(filePath)}";
        var result = await api.AddCandidateDocument(candidateId, ApiCalls.ToJson(new CovenantFileModel(fileName, description)), ApiCalls.FilePart(filePath, fileName), cancellationToken).AsResult();
        return result.Success
            ? new { success = true, documentId = result.Value }
            : new { success = false, statusCode = result.StatusCode, error = result.Error };
    }

    [McpServerTool(Name = "add_candidate_note"), Description("Adds a note to a candidate. Use it for information that has no field in Sigook, like screening answers or the apply date on the job board.")]
    public static async Task<object> AddCandidateNote(
        ISigookApi api,
        [Description("Candidate id.")] Guid candidateId,
        [Description("Note text.")] string note,
        CancellationToken cancellationToken = default)
    {
        var result = await api.AddCandidateNote(candidateId, new NoteModel(note, null), cancellationToken).AsResult();
        return result.Success
            ? new { success = true, noteId = result.Value!.Id }
            : new { success = false, statusCode = result.StatusCode, error = result.Error };
    }

    [McpServerTool(Name = "apply_candidate_to_request"), Description("Registers an existing candidate as an applicant of a request (job order). Use find_request to get the request id. Comments are shown to recruiters on the applicant. The API records the signed-in person as creator and adds the request job title to the candidate skills.")]
    public static async Task<object> ApplyCandidateToRequest(
        ISigookApi api,
        [Description("Request id (Guid) from find_request.")] Guid requestId,
        [Description("Candidate id from create_candidate or find_candidates.")] Guid candidateId,
        [Description("Optional comments, e.g. screening answers from the job board. Stored on the applicant even though the response does not echo them.")] string? comments = null,
        CancellationToken cancellationToken = default)
    {
        var result = await api.AddApplicant(requestId, new RequestApplicantModel(candidateId, null, null, Blank(comments)), cancellationToken).AsResult();
        return result.Success
            ? new { success = true, applicant = result.Value }
            : new { success = false, statusCode = result.StatusCode, error = result.Error };
    }

    private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
