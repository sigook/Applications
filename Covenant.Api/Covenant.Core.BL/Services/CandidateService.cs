using ClosedXML.Excel;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Covenant.Core.BL.Services;

public class CandidateService : ICandidateService
{
    private readonly IUserRepository userRepository;
    private readonly ICandidateRepository candidateRepository;
    private readonly IRequestRepository requestRepository;
    private readonly IAgencyRepository agencyRepository;
    private readonly IWorkerRepository workerRepository;
    private readonly ICandidateAdapter candidateAdapter;
    private readonly IIdentityServerService identityServerService;
    private readonly IDocumentService documentService;
    private readonly IValidator<CandidateCsvModel> bulkCandidateValidator;
    private readonly ILogger<CandidateService> logger;

    public CandidateService(
        IUserRepository userRepository,
        ICandidateRepository candidateRepository,
        IRequestRepository requestRepository,
        IAgencyRepository agencyRepository,
        IWorkerRepository workerRepository,
        ICandidateAdapter candidateAdapter,
        IIdentityServerService identityServerService,
        IDocumentService documentService,
        IValidator<CandidateCsvModel> bulkCandidateValidator,
        ILogger<CandidateService> logger)
    {
        this.userRepository = userRepository;
        this.candidateRepository = candidateRepository;
        this.requestRepository = requestRepository;
        this.agencyRepository = agencyRepository;
        this.workerRepository = workerRepository;
        this.candidateAdapter = candidateAdapter;
        this.identityServerService = identityServerService;
        this.documentService = documentService;
        this.bulkCandidateValidator = bulkCandidateValidator;
        this.logger = logger;
    }

    public async Task<Result<ResultGenerateDocument<byte[]>>> BulkCandidates(Guid agencyId, IFormFile file)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true
            });
            var records = csv.GetRecords<CandidateCsvModel>().ToList();
            using var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Report");
            sheet.SetupHeaders(new string[] { "Candidate Name" });
            var startAt = 2;
            var bulkCandidates = new List<BulkCandidate>();
            foreach (var record in records)
            {
                var bulkValidation = await bulkCandidateValidator.ValidateAsync(record);
                var request = default(Request);
                if (bulkValidation.IsValid && int.TryParse(record.OrderID, out var orderId))
                {
                    request = await requestRepository.GetRequest(r => r.AgencyId == agencyId && r.NumberId == orderId);
                    if (request == null)
                    {
                        bulkValidation.Errors.Add(new ValidationFailure("OrderID", "The order number doesn't belong to agency selected"));
                    }
                }
                if (bulkValidation.IsValid)
                {
                    var bulkCandidate = await candidateAdapter.ConvertCandidateCsvToCandidateBulk(record, agencyId, request);
                    bulkCandidates.Add(bulkCandidate);
                }
                else
                {
                    sheet.Cell($"A{startAt}").SetValue(record.FullName).AdjustToContents();
                    var errors = bulkValidation.Errors.Select(e => e.ErrorMessage);
                    for (int i = 0; i < errors.Count(); i++)
                    {
                        var columnLetter = (char)(i + 66);
                        var error = errors.ElementAt(i);
                        sheet.Cell($"{columnLetter}{startAt}").SetValue(error).AdjustToContents();
                    }
                    startAt++;
                }
            }
            if (startAt > 2)
            {
                workbook.SaveAs(stream);
            }
            if (bulkCandidates.Any())
            {
                await candidateRepository.BulkCandidates(bulkCandidates);
                await candidateRepository.SaveChangesAsync();
            }
            return Result.Ok(new ResultGenerateDocument<byte[]>(stream.ToArray(), "Bulk_Errors.xlxs", string.Empty));
        }
        catch (HeaderValidationException hve)
        {
            var headerNames = string.Join('|', hve.InvalidHeaders.SelectMany(ih => ih.Names));
            return Result.Fail<ResultGenerateDocument<byte[]>>($"There are missing the next headers in your excel file: {headerNames}");
        }
    }

    public async Task<Result> ConvertToWorker(Guid id)
    {
        var candidate = await candidateRepository.GetCandidate(c => c.Id == id);
        if (candidate == null) return Result.Fail();
        var worker = await candidateAdapter.ConvertCandidateToWorkerProfile(candidate);
        var agency = await agencyRepository.GetAgencyMasterByLocation(worker.Location.City);
        var profile = new WorkerProfile();
        var result = profile.PatchBasicInformation(worker);
        if (!result) return Result.Fail(result.Errors);
        result = profile.PatchContactInformation(worker);
        if (!result) return Result.Fail(result.Errors);
        result = profile.PatchSkills(candidate.Skills.Select(s => s.Skill).ToList());
        if (!result) return Result.Fail(result.Errors);
        result = profile.AddOtherDocuments(candidate.Documents.Select(d => d.Document).ToList());
        if (!result) return Result.Fail(result.Errors);
        var user = await identityServerService.CreateUser(new CreateUserModel
        {
            Email = candidate.Email,
            UserType = UserType.Worker
        });
        profile.Worker = user.Value;
        profile.AgencyId = agency.Id;
        profile.UpdateTextSearch();
        candidateRepository.Delete(candidate.Documents);
        await workerRepository.Create(profile);
        await workerRepository.SaveChangesAsync();
        if (candidate.Notes.Any())
        {
            foreach (var note in candidate.Notes)
            {
                var workerProfileNote = WorkerProfileNote.Create(profile.Id, note.Note.Note, note.Note.CreatedBy);
                if (workerProfileNote)
                {
                    workerProfileNote.Value.CreatedAt = note.Note.CreatedAt;
                    await workerRepository.Create(workerProfileNote.Value);
                }
            }
            await workerRepository.SaveChangesAsync();
        }
        var requestApplicants = await requestRepository.GetRequestApplicants(c => c.CandidateId == id);
        if (requestApplicants.Any())
        {
            foreach (var requestApplicant in requestApplicants)
            {
                requestApplicant.CandidateId = null;
                requestApplicant.WorkerProfileId = profile.Id;
            }
            await workerRepository.SaveChangesAsync();
        }
        result = await DeleteCandidate(id);
        if (!result) return Result.Fail(result.Errors);
        return Result.Ok();
    }

    public async Task<Result<Guid>> CreateCandidate(CandidateCreateModel model, Guid agencyId, bool validatePhone = true)
    {
        var candidate = candidateAdapter.ConvertCandidateModelToCandidate(model, agencyId);
        if (!candidate) return Result.Fail<Guid>(candidate.Errors);
        if (!string.IsNullOrWhiteSpace(candidate.Value.Email) && (await userRepository.UserExists(candidate.Value.Email) || await candidateRepository.CandidateExists(candidate.Value.Email)))
        {
            return Result.Fail<Guid>(ApiResources.EmailAlreadyTaken);
        }
        if (validatePhone && await candidateRepository.CandidatePhonesExists(candidate.Value.PhoneNumbers.Select(pn => pn.PhoneNumber)))
        {
            return Result.Fail<Guid>("Phone number already exists");
        }
        var candidateDocument = default(CandidateDocument);
        if (!string.IsNullOrWhiteSpace(model.FileName))
        {
            var covenantFile = new CovenantFileModel(model.FileName, "Resume");
            var file = CovenantFile.Create(covenantFile);
            await candidateRepository.Create(file.Value);
            candidateDocument = new CandidateDocument(candidate.Value.Id, file.Value);
        }
        await candidateRepository.Create(candidate.Value);
        if (candidateDocument != null)
        {
            await candidateRepository.Create(candidateDocument);
        }
        await candidateRepository.SaveChangesAsync();
        return Result.Ok(candidate.Value.Id);
    }

    public async Task<Result<Guid>> CreateCandidateDocument(Guid id, CovenantFileModel model)
    {
        var file = CovenantFile.Create(model);
        if (!file)
        {
            return Result.Fail<Guid>(file.Errors);
        }
        var entity = new CandidateDocument(id, file.Value);
        await candidateRepository.Create(file.Value);
        await candidateRepository.Create(entity);
        await candidateRepository.SaveChangesAsync();
        return Result.Ok(entity.DocumentId);
    }

    public async Task<Result> DeleteCandidate(Guid id)
    {
        var candidate = await candidateRepository.GetCandidate(c => c.Id == id);
        if (candidate == null) return Result.Fail("Candidate not found");
        var requestApplicatns = await requestRepository.GetRequestApplicants(ra => ra.CandidateId == id);
        if (requestApplicatns.Any())
        {
            foreach (var requestApplicant in requestApplicatns)
            {
                requestRepository.Delete(requestApplicant);
            }
        }
        candidateRepository.Delete(candidate);
        await candidateRepository.SaveChangesAsync();
        if (candidate.Documents.Any())
        {
            foreach (var document in candidate.Documents)
            {
                await documentService.DeleteFile(document.Document.Id);
            }
        }
        return Result.Ok();
    }

    public async Task<Result> UpdateCandidate(Guid id, CandidateCreateModel model)
    {
        var candidate = await candidateRepository.GetCandidate(c => c.Id == id);
        if (candidate == null) return Result.Fail();
        var result = CvnEmail.Create(model.Email);
        if (!result) return result;
        candidate.AddEmail(result.Value);
        candidate.Name = model.Name;
        candidate.Address = model.Address;
        candidate.ResidencyStatus = model.ResidencyStatus;
        candidate.GenderId = model.Gender?.Id;
        candidate.HasVehicle = model.HasVehicle;
        candidate.Dnu = model.Dnu;
        if (!string.IsNullOrEmpty(model.PostalCode))
        {
            var rPostalCode = CvnPostalCode.Create(model.PostalCode);
            if (!rPostalCode) return rPostalCode;
            candidate.AddPostalCode(rPostalCode.Value);
        }
        await candidateRepository.Update(candidate);
        await candidateRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateRecruiterCandidate(Guid id)
    {
        var candidate = await candidateRepository.GetCandidate(c => c.Id == id);
        if (candidate == null) return Result.Fail();
        candidate.Recruiter = identityServerService.GetNickname();
        await candidateRepository.Update(candidate);
        await candidateRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
