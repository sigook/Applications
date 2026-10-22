using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces.Shared;

namespace Covenant.Core.BL.Adapters;

public class CandidateAdapter(
    ICatalogRepository catalogRepository,
    ICurrentUserService currentUserService,
    ITeamsService teamsService,
    IFilesContainer filesContainer,
    ISkillService skillService) : ICandidateAdapter
{
    private readonly ICatalogRepository catalogRepository = catalogRepository;
    private readonly ICurrentUserService currentUserService = currentUserService;
    private readonly ITeamsService teamsService = teamsService;
    private readonly IFilesContainer filesContainer = filesContainer;

    public async Task<BulkCandidate> ConvertCandidateCsvToCandidateBulk(CandidateCsvModel model, Guid agencyId, Request request)
    {
        var genders = await catalogRepository.GetGender();
        var gender = genders.FirstOrDefault(g => g.Value.Equals(model.Gender, StringComparison.OrdinalIgnoreCase));
        var sources = await catalogRepository.GetSources();
        var source = sources.FirstOrDefault(s => s.Value.Equals(model.Source, StringComparison.OrdinalIgnoreCase))
            ?? sources.FirstOrDefault(s => s.Value.Equals("Other", StringComparison.OrdinalIgnoreCase));
        var bulkCandidate = new BulkCandidate();
        var candidate = new Candidate(agencyId, model.FullName)
        {
            Address = model.Address,
            GenderId = gender?.Id,
            HasVehicle = false,
            SourceId = source?.Id,
            ResidencyStatus = model.Status
        };
        bulkCandidate.Candidate = candidate;
        bulkCandidate.CandidatesPhone = new CandidatePhone(candidate.Id, model.Phone);
        var skills = await skillService.Canonicalize([.. (model.Skills ?? string.Empty).Split(','), request?.JobTitle]);
        bulkCandidate.CandidatesSkills.AddRange(skills.Select(s => CandidateSkill.Create(candidate.Id, s).Value));
        if (request != null)
        {
            bulkCandidate.RequestApplicant = RequestApplicant.CreateWithCandidate(request.Id, candidate.Id, "Sigook", string.Empty, RequestApplicantStatus.InProgress).Value;
        }
        if (!string.IsNullOrWhiteSpace(model.UrlResume))
        {
            var file = await teamsService.GetTeamsFile(model.UrlResume);
            var extension = Path.GetExtension(model.UrlResume);
            var fileName = $"Resume_{Guid.NewGuid():N}{extension}";
            if (await filesContainer.UploadAsync(file, fileName))
            {
                var covenantFile = new CovenantFile(fileName, "Resume");
                bulkCandidate.CandidatesDocument = new CandidateDocument(candidate.Id, covenantFile);
            }
        }
        return bulkCandidate;
    }

    public Result<Candidate> ConvertCandidateModelToCandidate(CandidateCreateModel model, Guid agencyId)
    {
        var recruiter = currentUserService.GetNickname();
        var candidate = new Candidate(agencyId, model.Name)
        {
            Address = model.Address,
            CityId = model.City?.Id,
            GenderId = model.Gender?.Id,
            HasVehicle = model.HasVehicle,
            SourceId = model.SourceId,
            Recruiter = recruiter,
            ResidencyStatus = model.ResidencyStatus,
        };
        if (!string.IsNullOrEmpty(model.PostalCode))
        {
            var rPostalCode = CvnPostalCode.Create(model.PostalCode);
            if (!rPostalCode)
            {
                return Result.Fail<Candidate>(rPostalCode.Errors);
            }
            candidate.AddPostalCode(rPostalCode.Value);
        }
        if (model.Skills != null)
        {
            foreach (var skill in model.Skills)
            {
                candidate.AddSkill(skill.Skill);
            }
        }
        if (model.PhoneNumbers != null)
        {
            foreach (var number in model.PhoneNumbers)
            {
                candidate.AddPhone(number?.PhoneNumber);
            }
        }
        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var result = CvnEmail.Create(model.Email);
            if (!result)
            {
                return Result.Fail<Candidate>(result.Errors);
            }
            candidate.AddEmail(result.Value);
        }
        return Result.Ok(candidate);
    }

    public WorkerProfileCreateModel ConvertCandidateToWorkerProfile(Candidate candidate)
    {
        var model = new WorkerProfileCreateModel();
        var name = candidate.Name.Split(" ");
        model.FirstName = name.ElementAt(0);
        model.LastName = string.Join(" ", name.Skip(1));
        model.Email = candidate.Email;
        model.Phone = candidate.PhoneNumbers.FirstOrDefault()?.PhoneNumber;
        model.Location = new LocationModel
        {
            Address = candidate.Address,
            PostalCode = candidate.PostalCode,
            City = new CityModel
            {
                Id = candidate.CityId.Value
            }
        };
        model.HasVehicle = candidate.HasVehicle;
        if (candidate.Gender != null)
        {
            model.Gender = new BaseModel<Guid>(candidate.Gender.Id);
        }
        return model;
    }
}
