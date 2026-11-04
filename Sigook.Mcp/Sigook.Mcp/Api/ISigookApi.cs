using System.Text.Json;
using Refit;
using Sigook.Mcp.Models;

namespace Sigook.Mcp.Api;

public interface ISigookApi
{
    [Get("/api/catalog/source")]
    Task<List<CatalogItem>> GetSources(CancellationToken cancellationToken);

    [Get("/api/catalog/gender")]
    Task<List<CatalogItem>> GetGenders(CancellationToken cancellationToken);

    [Get("/api/catalog/skills")]
    Task<List<SkillModel>> GetSkills(CancellationToken cancellationToken);

    [Get("/api/location/province/{countryCode}")]
    Task<List<ProvinceModel>> GetProvinces(string countryCode, CancellationToken cancellationToken);

    [Get("/api/location/city/{provinceId}")]
    Task<List<CityModel>> GetCities(Guid provinceId, CancellationToken cancellationToken);

    [Get("/api/agency/recruiting/requests")]
    Task<PaginatedList<AgencyRequestListModel>> GetRequests([Query] int numberId, [Query] int pageSize, CancellationToken cancellationToken);

    [Get("/api/agency/recruiting/candidates")]
    Task<PaginatedList<CandidateListModel>> GetCandidates([Query] string? name, [Query] string? phone, [Query] int pageSize, CancellationToken cancellationToken);

    [Multipart]
    [Post("/api/agency/recruiting/candidates")]
    Task<ApiResponse<IdResponse>> CreateCandidate([AliasAs("data")] string data, StreamPart? file, CancellationToken cancellationToken);

    [Multipart]
    [Post("/api/agency/recruiting/candidates/{candidateId}/documents")]
    Task<ApiResponse<Guid>> AddCandidateDocument(Guid candidateId, [AliasAs("data")] string data, StreamPart file, CancellationToken cancellationToken);

    [Post("/api/agency/recruiting/candidates/{candidateId}/notes")]
    Task<ApiResponse<IdResponse>> AddCandidateNote(Guid candidateId, [Body] NoteModel model, CancellationToken cancellationToken);

    [Post("/api/agency/recruiting/candidates/{candidateId}/skills")]
    Task<ApiResponse<IdResponse>> AddCandidateSkill(Guid candidateId, [Body] SkillModel model, CancellationToken cancellationToken);

    [Post("/api/agency/recruiting/requests/{requestId}/applicants")]
    Task<ApiResponse<JsonElement>> AddApplicant(Guid requestId, [Body] RequestApplicantModel model, CancellationToken cancellationToken);
}

public interface ISigookIdentityApi
{
    [Get("/connect/userinfo")]
    Task<UserInfo> GetUserInfo(CancellationToken cancellationToken);
}
