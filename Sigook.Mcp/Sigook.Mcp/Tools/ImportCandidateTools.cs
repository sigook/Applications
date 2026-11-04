using System.ComponentModel;
using ModelContextProtocol.Server;
using Sigook.Mcp.Api;
using Sigook.Mcp.Models;

namespace Sigook.Mcp.Tools;

[McpServerToolType]
public static class ImportCandidateTools
{
    [McpServerTool(Name = "import_candidate"), Description("Imports one candidate in a single call: normalizes the phone, checks duplicates by phone, resolves the city, looks up the source and the request by number, creates the candidate with the resume, applies it to the request and adds a note. Nothing is written when the phone is invalid, the city is unknown or the resume path does not exist. When the phone already belongs to a candidate it is not created again; the request application and the note go to the existing one. The result reports each step (created, applied, noteAdded) so you retry only the failed step with apply_candidate_to_request or add_candidate_note; never call import_candidate or create_candidate again once a candidateId was returned.")]
    public static async Task<object> ImportCandidate(
        ISigookApi api,
        CatalogCache catalog,
        [Description("Full name.")] string name,
        [Description("Phone as it comes from the export, any North American format.")] string phone,
        [Description("Email. Optional.")] string? email = null,
        [Description("City name. Optional; needs provinceOrState.")] string? city = null,
        [Description("Province or state code or name, e.g. PA, ON, Ontario.")] string? provinceOrState = null,
        [Description("CA or USA. Default USA.")] string countryCode = "USA",
        [Description("Source name as listed by list_sources, e.g. 'Zip Recruiter'. Unknown names fall back to Other.")] string? sourceName = null,
        [Description("Residency/work status free text, e.g. 'Authorized to work in the U.S.'. Optional.")] string? residencyStatus = null,
        [Description("Whether the candidate has a vehicle. Default false.")] bool hasVehicle = false,
        [Description("Street address. Optional.")] string? address = null,
        [Description("Postal/zip code. Optional.")] string? postalCode = null,
        [Description("Skills to attach. Optional.")] string[]? skills = null,
        [Description("Local path of the resume (pdf, doc, docx) as returned by read_export. Optional.")] string? resumePath = null,
        [Description("Sigook request number to apply the candidate to. Optional.")] int? requestNumberId = null,
        [Description("Comments stored on the applicant, e.g. screening answers. Optional.")] string? applyComments = null,
        [Description("Note added to the candidate, e.g. apply date, status and labels on the job board. Optional.")] string? note = null,
        CancellationToken cancellationToken = default)
    {
        var warnings = new List<string>();
        var phoneValue = LookupTools.TryNormalizePhone(phone);
        if (phoneValue is null) return Fail("phone", $"Phone '{phone}' does not have 10 digits.");
        if (!string.IsNullOrWhiteSpace(resumePath) && !File.Exists(resumePath)) return Fail("resume", $"Resume not found: {resumePath}");

        CityModel? cityModel = null;
        if (!string.IsNullOrWhiteSpace(city))
        {
            if (string.IsNullOrWhiteSpace(provinceOrState)) return Fail("city", "provinceOrState is required when city is given.");
            var resolution = await catalog.ResolveCity(city, provinceOrState, countryCode);
            if (resolution.City is null)
                return new { success = false, step = "city", error = resolution.Error, resolution.Alternatives, resolution.Provinces };
            cityModel = resolution.City;
        }

        AgencyRequestListModel? request = null;
        if (requestNumberId is not null)
        {
            request = await catalog.Request(requestNumberId.Value);
            if (request is null) warnings.Add($"Request {requestNumberId} not found; candidate not applied.");
        }

        var existing = await api.GetCandidates(null, phoneValue, 5, cancellationToken).OrThrow();
        var duplicate = existing.Items.FirstOrDefault(c => c.PhoneNumbers?.Any(p => p.PhoneNumber == phoneValue) == true) ?? existing.Items.FirstOrDefault();

        Guid candidateId;
        var created = false;
        var resumeUploaded = false;
        if (duplicate is not null)
        {
            candidateId = duplicate.Id;
            warnings.Add($"Phone {phoneValue} already belongs to '{duplicate.Name}' ({duplicate.Id}); candidate not created.");
        }
        else
        {
            var sourceId = await catalog.SourceId(sourceName);
            if (sourceId is null) warnings.Add($"Source '{sourceName}' not found and no 'Other' source; left empty.");
            var fileName = string.IsNullOrWhiteSpace(resumePath) ? null : $"Resume_{Guid.NewGuid():N}{Path.GetExtension(resumePath)}";
            var model = new CandidateCreateModel(
                name.Trim(),
                Blank(email)?.ToLowerInvariant(),
                Blank(address),
                Blank(postalCode),
                cityModel,
                null,
                hasVehicle,
                Blank(residencyStatus),
                sourceId,
                false,
                [new PhoneNumberModel(null, phoneValue)],
                (skills ?? []).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => new SkillModel(null, s.Trim())),
                fileName);
            var create = await api.CreateCandidate(ApiCalls.ToJson(model), fileName is null ? null : ApiCalls.FilePart(resumePath!, fileName), cancellationToken).AsResult();
            if (!create.Success)
                return new { success = false, step = "create", statusCode = create.StatusCode, error = create.Error, warnings };
            candidateId = create.Value!.Id;
            created = true;
            resumeUploaded = fileName is not null;
        }

        bool? applied = null;
        string? applyError = null;
        if (request is not null)
        {
            var apply = await api.AddApplicant(request.Id, new RequestApplicantModel(candidateId, null, null, Blank(applyComments)), cancellationToken).AsResult();
            var alreadyApplicant = !apply.Success && apply.Error?.Contains("already in the request", StringComparison.OrdinalIgnoreCase) == true;
            applied = apply.Success || alreadyApplicant;
            if (alreadyApplicant) warnings.Add($"Candidate was already an applicant of request {request.NumberId}.");
            else if (!apply.Success) applyError = $"({apply.StatusCode}) {apply.Error}";
        }

        bool? noteAdded = null;
        string? noteError = null;
        if (!string.IsNullOrWhiteSpace(note))
        {
            var added = await api.AddCandidateNote(candidateId, new NoteModel(note.Trim(), null), cancellationToken).AsResult();
            noteAdded = added.Success;
            if (!added.Success) noteError = $"({added.StatusCode}) {added.Error}";
        }

        return new
        {
            success = applied != false && noteAdded != false,
            candidateId,
            created,
            duplicate = duplicate is not null,
            phone = phoneValue,
            resumeUploaded,
            requestId = request?.Id,
            requestNumberId = request?.NumberId,
            applied,
            applyError,
            noteAdded,
            noteError,
            warnings
        };
    }

    private static object Fail(string step, string error) => new { success = false, step, error };

    private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
