namespace Sigook.Mcp.Models;

public sealed record CatalogItem(Guid Id, string Value);

public sealed record ProvinceModel(Guid Id, string Value, string? Code);

public sealed record CityModel(Guid Id, string Value, string? Code, ProvinceModel? Province);

public sealed record PaginatedList<T>(int PageIndex, int TotalPages, int TotalItems, List<T> Items);

public sealed record IdResponse(Guid Id);

public sealed record AgencyRequestListModel(
    Guid Id,
    int NumberId,
    string JobTitle,
    string CompanyFullName,
    string? Address,
    string? City,
    string? ProvinceName,
    string? PostalCode,
    int RequestStatus,
    int WorkersQuantity,
    int WorkersQuantityWorking);

public sealed record CandidateListModel(
    Guid Id,
    string Name,
    string? Email,
    string? Address,
    string? City,
    string? Province,
    IEnumerable<PhoneNumberModel>? PhoneNumbers,
    IEnumerable<SkillModel>? Skills,
    string? Recruiter,
    DateTime CreatedAt,
    bool HasDocuments,
    bool Dnu,
    string? Source,
    IEnumerable<CatalogItem>? Requests);

public sealed record PhoneNumberModel(Guid? Id, string PhoneNumber);

public sealed record SkillModel(Guid? Id, string Skill);

public sealed record CandidateCreateModel(
    string Name,
    string? Email,
    string? Address,
    string? PostalCode,
    CityModel? City,
    CatalogItem? Gender,
    bool HasVehicle,
    string? ResidencyStatus,
    Guid? SourceId,
    bool Dnu,
    IEnumerable<PhoneNumberModel> PhoneNumbers,
    IEnumerable<SkillModel> Skills,
    string? FileName);

public sealed record CovenantFileModel(string FileName, string Description);

public sealed record RequestApplicantModel(Guid? CandidateId, Guid? WorkerProfileId, string? CreatedBy, string? Comments);

public sealed record NoteModel(string Note, string? Color);

public sealed record UserInfo(string? Sub, string? Name, string? Email, string? Nickname, string? Role);
