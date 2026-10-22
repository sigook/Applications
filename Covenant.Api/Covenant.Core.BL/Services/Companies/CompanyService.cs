using ClosedXML.Excel;
using Covenant.Common.Models.Accounting.Invoice.Company;
using Covenant.Common.Models.Company.Agency;
using Covenant.Common.Models.Identity;
using Covenant.Common.Models.Notification;
using Covenant.Common.Models.Website;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Repositories.Notifications;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Accounting.Invoices;
using Covenant.Core.BL.Interfaces.Companies;
using Covenant.Documents.Extensions;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Adapters;
using CsvHelper.Configuration;
using CsvHelper;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Covenant.Core.BL.Services.Companies;

public class CompanyService : ICompanyService
{
    private const string CompanyProfileNotFound = "Company profile not found";
    private const string NoteNotFound = "Note not found";
    private readonly ICompanyRepository companyRepository;
    private readonly IUserRepository userRepository;
    private readonly IAgencyRepository agencyRepository;
    private readonly INotificationDataRepository notificationDataRepository;
    private readonly ICatalogRepository catalogRepository;
    private readonly IRequestRepository requestRepository;
    private readonly IInvoiceRepository invoiceRepository;
    private readonly IInvoiceCountryStrategyResolver invoiceStrategies;
    private readonly IUserAccountService userAccountService;
    private readonly ICurrentUserService currentUserService;
    private readonly IEmailService emailService;
    private readonly IGeocodeService geocodeService;
    private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
    private readonly IValidator<CompanyCsvModel> bulkCompanyValidator;
    private readonly ICompanyAdapter companyAdapter;
    private readonly IUploadedFilesService uploadedFilesService;
    private readonly IDocumentService documentService;
    private readonly IValidator<CompanyProfilePaymentTermsUpdateModel> paymentTermsValidator;
    private readonly IConfiguration configuration;
    public CompanyService(
        ICompanyRepository companyRepository,
        IUserRepository userRepository,
        IAgencyRepository agencyRepository,
        INotificationDataRepository notificationDataRepository,
        ICatalogRepository catalogRepository,
        IRequestRepository requestRepository,
        IInvoiceRepository invoiceRepository,
        IInvoiceCountryStrategyResolver invoiceStrategies,
        IUserAccountService userAccountService,
        ICurrentUserService currentUserService,
        IEmailService emailService,
        IGeocodeService geocodeService,
        IRazorViewToStringRenderer razorViewToStringRenderer,
        IValidator<CompanyCsvModel> bulkCompanyValidator,
        ICompanyAdapter companyAdapter,
        IUploadedFilesService uploadedFilesService,
        IDocumentService documentService,
        IValidator<CompanyProfilePaymentTermsUpdateModel> paymentTermsValidator,
        IConfiguration configuration)
    {
        this.companyRepository = companyRepository;
        this.userRepository = userRepository;
        this.agencyRepository = agencyRepository;
        this.notificationDataRepository = notificationDataRepository;
        this.catalogRepository = catalogRepository;
        this.requestRepository = requestRepository;
        this.invoiceRepository = invoiceRepository;
        this.invoiceStrategies = invoiceStrategies;
        this.userAccountService = userAccountService;
        this.currentUserService = currentUserService;
        this.emailService = emailService;
        this.geocodeService = geocodeService;
        this.razorViewToStringRenderer = razorViewToStringRenderer;
        this.bulkCompanyValidator = bulkCompanyValidator;
        this.companyAdapter = companyAdapter;
        this.uploadedFilesService = uploadedFilesService;
        this.documentService = documentService;
        this.paymentTermsValidator = paymentTermsValidator;
        this.configuration = configuration;
    }

    public async Task<Result> UpdateContactInformation(Guid profileId, CompanyProfileDetailModel model)
    {
        var webPage = CvnWebPage.Create(model.Website);
        if (!webPage) return webPage;
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail(CompanyProfileNotFound);
        entity.UpdatePhones(model.Phone, model.PhoneExt, model.Fax, model.FaxExt);
        entity.UpdateWebsite(model.Website);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateVaccinationInfo(Guid profileId, VaccinationRequiredModel model)
    {
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail(CompanyProfileNotFound);
        var result = entity.UpdateVaccinationInfo(model.Required, model.Comments);
        if (!result) return result;
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateRequiresPermissionToSeeRequests(Guid profileId, bool requiresPermission)
    {
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail(ResultError.NotFound(CompanyProfileNotFound));
        entity.UpdatePermissionToSeeRequests(requiresPermission);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdatePaidHolidays(Guid profileId, bool paidHolidays)
    {
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail(ResultError.NotFound(CompanyProfileNotFound));
        entity.UpdatePaidHolidays(paidHolidays);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateOvertime(Guid profileId, double overtimeStartsAfterHours)
    {
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail(ResultError.NotFound(CompanyProfileNotFound));
        var result = entity.UpdateOvertimeStartsAfter(TimeSpan.FromHours(overtimeStartsAfterHours));
        if (!result) return result;
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdatePaymentTerms(Guid profileId, CompanyProfilePaymentTermsUpdateModel model)
    {
        var validation = await paymentTermsValidator.ValidateAsync(model);
        if (!validation.IsValid) return validation.ToResultFailure();
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail(ResultError.NotFound(CompanyProfileNotFound));
        entity.UpdatePaymentTermDays(model.PaymentTermDays);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteContactPerson(Guid id)
    {
        var contact = await companyRepository.GetContactPerson(id);
        if (contact is null) return Result.Fail("Contact person not found");
        companyRepository.Delete(contact);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateCompanyUser(Guid id, CompanyUserModel model)
    {
        var entity = await companyRepository.GetCompanyUser(id);
        if (entity is null) return Result.Fail("Company user not found");
        entity.UpdateDetails(model.Name, model.Lastname, model.Position, model.MobileNumber);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteCompanyLocation(Guid id)
    {
        var location = await companyRepository.GetLocation(id);
        if (location is null) return Result.Fail("Location not found");
        companyRepository.Delete(location);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Guid> CreateInvoiceRecipient(Guid profileId, CompanyProfileInvoiceRecipientModel model)
    {
        var id = await companyRepository.CreateInvoiceRecipient(profileId, model);
        await companyRepository.SaveChangesAsync();
        return id;
    }

    public async Task UpdateInvoiceRecipient(Guid id, CompanyProfileInvoiceRecipientModel model)
    {
        await companyRepository.UpdateInvoiceRecipient(id, model);
        await companyRepository.SaveChangesAsync();
    }

    public Task UpdateInvoiceNotes(Guid profileId, string htmlNotes) =>
        companyRepository.UpdateCompanyProfileInvoiceNotes(profileId, htmlNotes);

    public async Task<Result> DeleteInvoiceRecipient(Guid id)
    {
        var invoiceRecipient = await companyRepository.GetInvoiceRecipient(id);
        if (invoiceRecipient is null) return Result.Fail("Invoice recipient not found");
        companyRepository.Delete(invoiceRecipient);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<NoteModel>> CreateNote(Guid profileId, NoteModel model)
    {
        var note = CovenantNote.Create(model.Note, model.Color, currentUserService.GetNickname());
        if (!note) return Result.Fail<NoteModel>(note.Errors);
        var entity = new CompanyProfileNote(profileId, note.Value);
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok(new NoteModel
        {
            Id = entity.NoteId,
            CreatedBy = entity.Note.CreatedBy,
            CreatedAt = entity.Note.CreatedAt,
            Color = entity.Note.Color
        });
    }

    public async Task<Result> UpdateNote(Guid profileId, Guid noteId, NoteModel model)
    {
        var nickname = currentUserService.GetNickname();
        var note = CovenantNote.Create(model.Note, model.Color, nickname);
        if (!note) return note;
        var entity = await companyRepository.GetNote(profileId, noteId);
        if (entity is null) return Result.Fail(NoteNotFound);
        entity.Update(note.Value, nickname);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteNote(Guid profileId, Guid noteId)
    {
        var entity = await companyRepository.GetNote(profileId, noteId);
        if (entity is null) return Result.Fail(NoteNotFound);
        entity.Delete(currentUserService.GetNickname());
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> PetitionJobPosition(Guid profileId, JobPositionPetitionModel model)
    {
        var emailsString = configuration.GetValue<string>("EmailsPetitionNewJobPosition");
        if (string.IsNullOrEmpty(emailsString)) return Result.Fail("Petition not available");
        if (string.IsNullOrEmpty(model?.JobPosition)) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.JobPosition));
        var profile = await companyRepository.GetCompanyProfileDetail(cp => cp.Id == profileId);
        if (profile is null) return Result.Fail("Invalid company");
        var nickname = currentUserService.GetNickname();
        var subject = $"Rol {model.JobPosition} requested by {nickname}";
        foreach (var email in emailsString.Split(","))
        {
            await emailService.SendEmail(new EmailParams(email, subject,
                $"Client:{profile.NumberId} {profile.FullName} <br/> {model.Message} <br/> Please notify {nickname} after create the role")
            {
                EmailSettingName = EmailSettingName.Notification
            });
        }
        return Result.Ok();
    }

    public async Task<Result> UpdateLogo(Guid profileId)
    {
        var logoFileName = uploadedFilesService.GetFileNames().FirstOrDefault();
        if (logoFileName is null) return Result.Fail("Logo file is required");
        var filesValidation = uploadedFilesService.Validate();
        if (!filesValidation) return filesValidation;
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail("Company profile not found");
        var oldLogoId = entity.LogoId;
        var logo = new CovenantFile(logoFileName);
        await companyRepository.Create(logo);
        entity.ReplaceLogo(logo.Id);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        await uploadedFilesService.Upload([logo.FileName]);
        if (oldLogoId.HasValue) await documentService.DeleteFile(oldLogoId.Value);
        return Result.Ok();
    }

    public async Task<Result<Guid>> CreateCompanyProfile(CompanyRegisterByItselfModel model)
    {
        var email = CvnEmail.Create(model.Email);
        if (!email) return Result.Fail<Guid>(email.Errors);
        if (await userRepository.UserExists(email.Value.Email)) return Result.Fail<Guid>(ApiResources.EmailAlreadyTaken);
        var user = await userAccountService.CreateUser(new CreateUserModel
        {
            Email = email.Value,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            UserType = UserType.Company,
            Role = CovenantConstants.Role.Company
        });
        if (!user) return Result.Fail<Guid>(user.Errors);
        var location = model.Locations.First();
        var agency = await agencyRepository.GetAgencyMasterByLocation(location.City);
        if (agency is null) return Result.Fail<Guid>(ApiResources.AgencyNotFound);
        var rLocation = Location.Create(location.City.Id, location.Address, location.PostalCode);
        if (!rLocation) return Result.Fail<Guid>(rLocation.Errors);
        var entity = CompanyProfile.CompanyRegisterByItself(
            user.Value,
            agency.Id,
            model.Name,
            model.Phone,
            model.PhoneExt,
            rLocation.Value,
            location.IsBilling,
            model.JobPositionRates.Select(j => j.JobPosition).ToList(),
            model.Logo?.FileName);
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();
        var data = await notificationDataRepository.GetAgencyData(entity.AgencyId);
        if (data != null && data.EmailNotification)
        {
            data.CompanyFullName = entity.FullName;
            var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnNewCompany/AgencyTemplate.cshtml", data);
            await emailService.SendEmail(new EmailParams(data.AgencyEmail, "New Company", message));
        }
        return Result.Ok(entity.Id);
    }

    public async Task<Result> UpdateProfile(Guid profileId, CompanyProfileDetailModel model)
    {
        var entity = await companyRepository.GetCompanyProfile(cp => cp.Id == profileId);
        if (entity is null) return Result.Fail();
        entity.ChangeFullName(model.FullName);
        entity.UpdatePhones(model.Phone, model.PhoneExt, model.Fax, model.FaxExt);
        entity.UpdateWebsite(model.Website);
        entity.Industry.Update(model.Industry.Industry.Id, entity.Industry.OtherIndustry);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Guid>> CreateCompanyLocation(CompanyProfileLocationDetailModel model, Guid? profileId = null)
    {
        if (!profileId.HasValue)
        {
            var companyId = currentUserService.GetCompanyId();
            var profile = await companyRepository.GetCompanyProfileDetail(cp => cp.CompanyId == companyId);
            profileId = profile.Id;
        }
        var location = await CreateLocation(model);
        if (!location)
        {
            return Result.Fail<Guid>(location.Errors);
        }
        var entity = new CompanyProfileLocation(profileId.Value, location.Value);
        entity.UpdateIsBilling(model.IsBilling);
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();

        return Result.Ok(entity.LocationId);
    }

    public async Task<Result> UpdateCompanyLocation(Guid id, CompanyProfileLocationDetailModel model)
    {
        var entity = await companyRepository.GetLocation(id);
        if (entity == null)
        {
            return Result.Fail();
        }
        var location = await CreateLocation(model);
        if (!location)
        {
            return Result.Fail<Guid>(location.Errors);
        }
        entity.Location.Update(location.Value);
        entity.UpdateIsBilling(model.IsBilling);
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();

        return Result.Ok();
    }

    private async Task<Result<Location>> CreateLocation(CompanyProfileLocationDetailModel model)
    {
        var location = Location.Create(model.City?.Id, model.Address, model.PostalCode, model.Entrance, model.MainIntersection, model.Latitude, model.Longitude);
        if (!location)
        {
            return location;
        }
        if (!location.Value.Latitude.HasValue || !location.Value.Longitude.HasValue)
        {
            var city = await catalogRepository.GetCity(model.City.Id);
            var address = $"{model.Address} {city.Value} {city.Province.Code} {model.PostalCode}";
            var geocodeLocation = await geocodeService.GetLocationGeocode(address);
            location.Value.UpdateCoordinates(geocodeLocation.Lat, geocodeLocation.Lng);
        }
        return location;
    }

    public async Task<Result> RequestNewJobPosition(ContactDto contact)
    {
        var companyId = currentUserService.GetCompanyId();
        var companyProfile = await companyRepository.GetCompanyProfile(cp => cp.CompanyId == companyId);
        contact.Email = companyProfile.Company.Email;
        var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnContactAgency/AgencyTemplate.cshtml", contact);
        await emailService.SendEmail(new EmailParams(companyProfile.Agency.User.Email, "Company request new type of job", message));
        return Result.Ok();
    }

    public async Task<Result> RequestNewWorker(Guid requestId, CommentsModel model)
    {
        var companyId = currentUserService.GetCompanyId();
        var companyProfile = await companyRepository.GetCompanyProfile(cp => cp.CompanyId == companyId);
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail(ApiResources.RequestNotAvailable);
        var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnNewWorkerRequested/AgencyTemplate.cshtml",
            new OnNewWorkerRequestedAgencyTemplateViewModel
            {
                JobTitle = request.JobTitle,
                CompanyFullName = companyProfile.FullName,
                Comment = model.Comments
            });
        await emailService.SendEmail(new EmailParams(companyProfile.Agency.User.Email, "A new worker was requested", message)
        {
            Cc = new List<string> { companyProfile.Agency.RecruitmentEmail }
        });
        return Result.Ok();
    }

    public Task<PaginatedList<InvoiceListModel>> GetCompanyInvoices(GetCompanyInvoiceFilter filter) =>
        invoiceRepository.GetInvoicesForCompany(currentUserService.GetCompanyId(), filter);

    public async Task<InvoiceSummaryModel> GetCompanyInvoice(Guid invoiceId)
    {
        var summary = await invoiceRepository.GetInvoiceSummaryForCompany(currentUserService.GetCompanyId(), invoiceId);
        if (summary is null) return null;
        (await invoiceStrategies.ResolveForAgency(summary.AgencyId)).Decorate(summary);
        return summary;
    }

    public async Task<Result> CreateCompanyUser(CompanyUserModel model, Guid? companyProfileId = null)
    {
        var email = CvnEmail.Create(model.Email);
        if (email)
        {
            var user = await userRepository.GetUserByEmail(email.Value);
            if (user == null)
            {
                var profile = await GetCompanyProfileIds(companyProfileId);
                if (profile is null) return Result.Fail("Company profile not found");
                var userModel = new CreateUserModel
                {
                    Email = email.Value,
                    CompanyId = profile.CompanyId,
                    UserType = UserType.CompanyUser,
                    Role = CovenantConstants.Role.CompanyUser
                };
                var newUser = await userAccountService.CreateUser(userModel);
                if (newUser)
                {
                    var entity = new CompanyUser(profile.Id, newUser.Value);
                    entity.UpdateDetails(model.Name, model.Lastname, model.Position, model.MobileNumber);
                    await companyRepository.Create(entity);
                    await companyRepository.SaveChangesAsync();
                    return Result.Ok();
                }
                return newUser;
            }
            return Result.Fail(ApiResources.EmailAlreadyTaken);
        }
        return email;
    }

    public async Task<Result> DeleteCompanyUser(Guid userId, Guid? companyProfileId = null)
    {
        var profile = await GetCompanyProfileIds(companyProfileId);
        if (profile is null) return Result.Fail("Company profile not found");
        var entity = await companyRepository.GetCompanyUser(userId);
        if (entity != null)
        {
            companyRepository.Delete(entity);
            await companyRepository.SaveChangesAsync();
            var identityServiceResult = await userAccountService.DeleteUserOrClaim(userId, profile.CompanyId);
            if (identityServiceResult)
            {
                return Result.Ok();
            }
            return identityServiceResult;
        }
        return Result.Fail("User not found");
    }

    public Task<CompanyDeletionCheckModel> CheckCompanyDeletion(Guid companyProfileId) =>
        companyRepository.GetDeletionCheck(companyProfileId);

    public async Task<Result> DeleteCompanyProfile(Guid companyProfileId)
    {
        var check = await companyRepository.GetDeletionCheck(companyProfileId);
        if (check is null) return Result.Fail("Company profile not found");
        if (!check.CanDelete)
        {
            var detail = string.Join(", ", check.Blockers.Select(b => $"{b.Entity} ({b.Count})"));
            return Result.Fail($"The company cannot be deleted because it has related records: {detail}");
        }

        var profile = await companyRepository.GetCompanyProfileId(p => p.Id == companyProfileId);
        var companyUserIds = await companyRepository.GetCompanyUserIds(companyProfileId);
        await companyRepository.DeleteCompanyProfile(companyProfileId);

        foreach (var userId in companyUserIds)
        {
            var companyUserResult = await userAccountService.DeleteUserOrClaim(userId, profile.CompanyId);
            if (!companyUserResult) return companyUserResult;
        }
        return await userAccountService.DeleteUserOrClaim(profile.CompanyId, profile.CompanyId);
    }

    private Task<CompanyProfileIdsModel> GetCompanyProfileIds(Guid? companyProfileId) =>
        companyProfileId.HasValue
            ? companyRepository.GetCompanyProfileId(p => p.Id == companyProfileId.Value)
            : companyRepository.GetCompanyProfileId(p => p.CompanyId == currentUserService.GetCompanyId());

    public async Task<Result<ResultGenerateDocument<byte[]>>> BulkCompany(Guid agencyId, Stream file)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true,
                PrepareHeaderForMatch = args => args.Header.Trim()
            });
            var records = csv.GetRecords<CompanyCsvModel>().ToList();
            using var stream = new MemoryStream();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Report");
            sheet.SetupHeaders(new string[] { "Company Name" });
            var startAt = 2;
            var bulkCompanies = new List<BulkCompany>();
            var industries = await catalogRepository.GetIndustries();
            foreach (var record in records)
            {
                var bulkValidation = await bulkCompanyValidator.ValidateAsync(record);

                BaseModel<Guid> industry = industries.FirstOrDefault(i => i.Value.Equals(record.PrimaryIndustry, StringComparison.OrdinalIgnoreCase));
                if (industry is null)
                {
                    bulkValidation.Errors.Add(new ValidationFailure("PrimaryIndustry", $"The industry '{record.PrimaryIndustry}' does not exist in the system. Please create it before proceeding with the bulk upload."));
                }

                CityModel city = await catalogRepository.GetCity(record.CompanyCity);

                if (city is null)
                {
                    bulkValidation.Errors.Add(new ValidationFailure("CompanyCity", $"The City '{record.CompanyCity}' does not exist in the system. Please create it before proceeding with the bulk upload."));
                }

                if (bulkValidation.IsValid)
                {
                    var bulkCompany = await companyAdapter.ConvertCompanyCsvToCompanyBulk(record, agencyId, industry, city);
                    bulkCompanies.Add(bulkCompany);
                }
                else
                {
                    sheet.Cell($"A{startAt}").SetValue(record.CompanyName).AdjustToContents();
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
            if (bulkCompanies.Any())
            {
                await companyRepository.BulkCompanies(bulkCompanies);
                await companyRepository.SaveChangesAsync();
            }

            return Result.Ok(new ResultGenerateDocument<byte[]>(stream.ToArray(), "Bulk_Errors.xlxs", string.Empty));
        }
        catch (HeaderValidationException hve)
        {
            var headerNames = string.Join('|', hve.InvalidHeaders.SelectMany(ih => ih.Names));
            return Result.Fail<ResultGenerateDocument<byte[]>>($"There are missing the next headers in your excel file: {headerNames}");
        }
    }

    public async Task<Result> CreateContact(CompanyProfileContactPersonModel model)
    {
        var profile = await companyRepository.GetCompanyProfile(cp => cp.CompanyId == currentUserService.GetCompanyId());
        var entity = new CompanyProfileContactPerson(profile.Id);
        entity.Update(model.Title, model.FirstName, model.MiddleName, model.LastName, model.Position, model.MobileNumber, model.OfficeNumber, model.OfficeNumberExt, model.Email);
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

}
