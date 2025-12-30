using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;

namespace Covenant.Core.BL.Adapters;

public class CandidateAdapter : ICandidateAdapter
{
    private IEnumerable<BaseModel<Guid>> genders;

    private readonly IAgencyRepository agencyRepository;
    private readonly ICatalogRepository catalogRepository;
    private readonly IIdentityServerService identityServerService;
    private readonly ITeamsService teamsService;
    private readonly IFilesContainer filesContainer;

    public CandidateAdapter(
        IAgencyRepository agencyRepository,
        ICatalogRepository catalogRepository,
        IIdentityServerService identityServerService,
        ITeamsService teamsService,
        IFilesContainer filesContainer)
    {
        this.agencyRepository = agencyRepository;
        this.catalogRepository = catalogRepository;
        this.identityServerService = identityServerService;
        this.teamsService = teamsService;
        this.filesContainer = filesContainer;
    }

    private IEnumerable<BaseModel<Guid>> Genders
    {
        get
        {
            if (genders is null)
            {
                genders = catalogRepository
                    .GetGender()
                    .GetAwaiter()
                    .GetResult();
            }
            return genders;
        }
    }

    public async Task<BulkCandidate> ConvertCandidateCsvToCandidateBulk(CandidateCsvModel model, Guid agencyId, Request request)
    {
        var gender = Genders.FirstOrDefault(g => g.Value == model.Gender);
        var bulkCandidate = new BulkCandidate();
        var candidate = new Candidate(agencyId, model.FullName)
        {
            Address = model.Address,
            GenderId = gender?.Id,
            HasVehicle = false,
            Source = model.Source,
            ResidencyStatus = model.Status
        };
        bulkCandidate.Candidate = candidate;
        bulkCandidate.CandidatesPhone = new CandidatePhone(candidate.Id, model.Phone);
        if (!string.IsNullOrWhiteSpace(model.Skills))
        {
            var skills = model.Skills
                .Split(',')
                .Select(s => CandidateSkill.Create(candidate.Id, s).Value);
            bulkCandidate.CandidatesSkills.AddRange(skills);
        }
        if (request != null)
        {
            var requestSkill = CandidateSkill.Create(candidate.Id, request.JobTitle).Value;
            bulkCandidate.CandidatesSkills.Add(requestSkill);
            bulkCandidate.RequestApplicant = RequestApplicant.CreateWithCandidate(request.Id, candidate.Id, "Sigook", string.Empty).Value;
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
        bulkCandidate.CandidatesSkills = bulkCandidate.CandidatesSkills.Distinct().ToList();
        return bulkCandidate;
    }

    public Result<Candidate> ConvertCandidateModelToCandidate(CandidateCreateModel model, Guid agencyId)
    {
        var recruiter = identityServerService.GetNickname();
        var candidate = new Candidate(agencyId, model.Name)
        {
            Address = model.Address,
            GenderId = model.Gender?.Id,
            HasVehicle = model.HasVehicle,
            Source = model.Source,
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

    public async Task<WorkerProfileCreateModel> ConvertCandidateToWorkerProfile(Candidate candidate)
    {
        var model = new WorkerProfileCreateModel();
        var name = candidate.Name.Split(" ");
        model.FirstName = name.ElementAt(0);
        model.LastName = string.Join(" ", name.Skip(1));
        model.Email = candidate.Email;
        model.Phone = candidate.PhoneNumbers.FirstOrDefault()?.PhoneNumber;
        var agencyLocation = (await agencyRepository.GetLocations(identityServerService.GetAgencyId())).FirstOrDefault();
        model.Location = new LocationModel
        {
            Address = candidate.Address,
            PostalCode = candidate.PostalCode,
            City = new CityModel
            {
                Id = agencyLocation.City.Id
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
