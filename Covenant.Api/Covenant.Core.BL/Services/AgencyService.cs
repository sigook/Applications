using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.TimeSheetTotal.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Covenant.Core.BL.Services;

public class AgencyService : IAgencyService
{
    private readonly IAgencyRepository agencyRepository;
    private readonly ICompanyRepository companyRepository;
    private readonly IUserRepository userRepository;
    private readonly IShiftRepository shiftRepository;
    private readonly IWorkerRepository workerRepository;
    private readonly IRequestRepository requestRepository;
    private readonly IWorkerRequestRepository workerRequestRepository;
    private readonly INotificationDataRepository notificationDataRepository;
    private readonly ICatalogRepository catalogRepository;
    private readonly ITimeService timeService;
    private readonly IIdentityServerService identityServerService;
    private readonly IDocumentService documentService;
    private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
    private readonly IEmailService emailService;
    private readonly ILogger<AgencyService> logger;
    private readonly IServiceProvider serviceProvider;

    public AgencyService(
        IAgencyRepository agencyRepository,
        ICompanyRepository companyRepository,
        IUserRepository userRepository,
        IShiftRepository shiftRepository,
        IWorkerRepository workerRepository,
        IRequestRepository requestRepository,
        IWorkerRequestRepository workerRequestRepository,
        INotificationDataRepository notificationDataRepository,
        ICatalogRepository catalogRepository,
        ITimeService timeService,
        IIdentityServerService identityServerService,
        IDocumentService documentService,
        IRazorViewToStringRenderer razorViewToStringRenderer,
        IEmailService emailService,
        ILogger<AgencyService> logger,
        IServiceProvider serviceProvider)
    {
        this.timeService = timeService;
        this.agencyRepository = agencyRepository;
        this.companyRepository = companyRepository;
        this.userRepository = userRepository;
        this.shiftRepository = shiftRepository;
        this.workerRepository = workerRepository;
        this.requestRepository = requestRepository;
        this.workerRequestRepository = workerRequestRepository;
        this.notificationDataRepository = notificationDataRepository;
        this.catalogRepository = catalogRepository;
        this.identityServerService = identityServerService;
        this.documentService = documentService;
        this.razorViewToStringRenderer = razorViewToStringRenderer;
        this.emailService = emailService;
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    public async Task<Result<Guid>> CreateCompany(CompanyProfileDetailModel model)
    {
        var validator = serviceProvider.GetService<IValidator<CompanyProfileDetailModel>>();
        var companyProfileValidation = await validator.ValidateAsync(model);

        if (!companyProfileValidation.IsValid)
        {
            return companyProfileValidation.ToResultFailure<Guid>();
        }

        var agencyId = identityServerService.GetAgencyId();

        var rFullName = CompanyName.Create(model.FullName ?? model.BusinessName);
        var rBusinessName = CompanyName.Create(model.BusinessName ?? model.FullName);

        CovenantFile logo = null;
        if (!string.IsNullOrEmpty(model.Logo?.FileName))
        {
            var rLogo = CovenantFile.Create(model.Logo);
            if (!rLogo) return Result.Fail<Guid>(rLogo.Errors);
            logo = rLogo.Value;
            await companyRepository.Create(logo);
        }

        var rIndustry = CompanyProfileIndustry.Create(model.Industry.Industry, model.Industry.OtherIndustry);

        var user = await identityServerService.CreateUser(new CreateUserModel
        {
            Email = model.Email,
            Password = model.Password,
            UserType = UserType.Company
        });

        var createdBy = identityServerService.GetNickname();
        var rProfile = CompanyProfile.AgencyCreateCompany(
            user.Value,
            agencyId,
            rFullName.Value,
            rBusinessName.Value,
            model.Phone,
            model.PhoneExt,
            model.Fax,
            model.FaxExt,
            model.Website,
            rIndustry.Value,
            logo,
            model.About,
            model.InternalInfo,
            model.RequiresPermissionToSeeOrders,
            createdBy,
            model.CompanyStatus,
            model.SalesRepresentativeId);

        if (!rProfile) return Result.Fail<Guid>(rProfile.Errors);

        var entity = rProfile.Value;

        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();

        return Result.Ok(entity.Id);
    }

    public async Task<Result> UpdateCompany(Guid companyProfileId, CompanyProfileDetailModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        var businessName = CompanyName.Create(model.BusinessName ?? model.FullName);
        if (!businessName) return Result.Fail(businessName.Errors);
        var fullName = CompanyName.Create(model.FullName ?? model.BusinessName);
        if (!fullName) return Result.Fail(fullName.Errors);
        var company = await companyRepository.GetCompanyProfile(cp => cp.Id == companyProfileId);
        if (!string.IsNullOrEmpty(model.Website))
        {
            var website = CvnWebPage.Create(model.Website);
            if (!website) return Result.Fail<Guid>(website.Errors);
            company.Website = website.Value;
        }
        if (!string.IsNullOrEmpty(model.Logo.FileName))
        {
            if (company.LogoId.HasValue)
            {
                await documentService.DeleteFile(company.LogoId.Value);
            }
            var logo = new CovenantFile(model.Logo.FileName, model.Logo.Description);
            await companyRepository.Create(logo);
            company.LogoId = logo.Id;
        }
        company.FullName = model.FullName;
        company.BusinessName = model.BusinessName;
        if (model.Industry.Industry != null)
        {
            company.Industry.IndustryId = model.Industry.Industry.Id;
            company.Industry.OtherIndustry = null;
        }
        else
        {
            company.Industry.IndustryId = null;
            company.Industry.OtherIndustry = model.Industry.OtherIndustry;
        }
        if (company.CompanyStatus != model.CompanyStatus)
        {
            company.CompanyStatus = model.CompanyStatus;
            company.UpdatedBy = identityServerService.GetNickname();
            company.UpdatedAt = DateTime.Now;
        }
        company.SalesRepresentativeId = model.SalesRepresentativeId;
        company.About = model.About;
        company.InternalInfo = model.InternalInfo;
        company.Phone = model.Phone;
        company.PhoneExt = model.PhoneExt;
        company.Fax = model.Fax;
        company.FaxExt = model.FaxExt;

        companyRepository.Update(company);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Guid>> CreateCompanyContactPerson(Guid profileId, CompanyProfileContactPersonModel model)
    {
        var rEmail = CvnEmail.Create(model.Email);
        if (!rEmail) return Result.Fail<Guid>(rEmail.Errors);
        var entity = new CompanyProfileContactPerson(profileId)
        {
            Title = model.Title,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            Position = model.Position,
            MobileNumber = model.MobileNumber,
            OfficeNumber = model.OfficeNumber,
            OfficeNumberExt = model.OfficeNumberExt,
            Email = rEmail.Value
        };
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok(entity.Id);
    }

    public async Task<Result> UpdateCompanyContactPerson(Guid id, CompanyProfileContactPersonModel model)
    {
        var rEmail = CvnEmail.Create(model.Email);
        if (!rEmail) return Result.Fail<CompanyProfileContactPerson>(rEmail.Errors);
        var entity = await companyRepository.GetContactPerson(id);
        if (entity == null)
        {
            return Result.Fail();
        }
        entity.Title = model.Title;
        entity.FirstName = model.FirstName;
        entity.MiddleName = model.MiddleName;
        entity.LastName = model.LastName;
        entity.Position = model.Position;
        entity.MobileNumber = model.MobileNumber;
        entity.OfficeNumber = model.OfficeNumber;
        entity.OfficeNumberExt = model.OfficeNumberExt;
        entity.Email = rEmail.Value;
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Guid>> CreateCompanyJobPosition(Guid profileId, CompanyProfileJobPositionRateModel model)
    {
        var entity = ToJobPosition(profileId, model);
        if (!entity)
        {
            return Result.Fail<Guid>(entity.Errors);
        }
        await companyRepository.Create(entity.Value);
        await companyRepository.SaveChangesAsync();
        return Result.Ok(entity.Value.Id);
    }

    public async Task<Result> UpdateCompanyJobPosition(Guid id, Guid profileId, CompanyProfileJobPositionRateModel model)
    {
        var result = ToJobPosition(profileId, model);
        if (!result)
        {
            return Result.Fail(result.Errors);
        }
        var entity = await companyRepository.GetJobPosition(id);
        if (entity == null)
        {
            return Result.Fail();
        }
        entity.OnShiftUpdated += async (sender, e) => await shiftRepository.Create(e);
        entity.Update(result.Value);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task NotifySinsExpired()
    {
        var now = timeService.GetCurrentDateTime();
        var workers = await workerRepository.GetWorkersSinExpired(now.AddDays(7));
        var workersByAgency = workers.GroupBy(w => w.AgencyEmail);
        foreach (var worker in workersByAgency)
        {
            try
            {
                var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/WorkerSINExpired/AgencyTemplate.cshtml", worker.Select(w => w));
                await emailService.SendEmail(new EmailParams(worker.Key, "Please update SIN", message)
                {
                    EmailSettingName = EmailSettingName.Notification,
                    Cc = new List<string> { worker.FirstOrDefault().RecruitmentEmail }
                });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error notifying about sin expiration");
            }
        }
    }

    public async Task NotifyLicensesExpired()
    {
        var now = timeService.GetCurrentDateTime();
        var workers = await workerRepository.GetWorkerLicensesExpired(now.AddDays(7));
        var workersByAgency = workers.GroupBy(w => w.AgencyEmail);
        foreach (var worker in workersByAgency)
        {
            try
            {
                var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/WorkerLicensesExpired/AgencyTemplate.cshtml", worker.Select(w => w));
                await emailService.SendEmail(new EmailParams(worker.Key, "Expired Licenses", message)
                {
                    EmailSettingName = EmailSettingName.Notification,
                    Cc = new List<string> { worker.FirstOrDefault().RecruitmentEmail }
                });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error notifying about licenses expired");
            }
        }
    }

    public async Task<Result> IncreaseWorkersQuantityByOne(Guid requestId)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail(ApiResources.RequestNotAvailable);
        var result = request.IncreaseWorkersQuantityByOne();
        if (!result) return result;
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Guid>> BookWorker(Guid requestId, Guid workerId, AgencyBookWorkerModel model)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail<Guid>(ApiResources.RequestNotAvailable);
        var workerProfile = await GetWorkerProfile(workerId, requestId);
        if (!workerProfile) return Result.Fail<Guid>(workerProfile.Errors);
        var createdBy = identityServerService.GetNickname();
        var result = request.AddWorker(workerId, model.StartWorking ?? timeService.GetCurrentDateTime().Date, createdBy);
        if (!result) return result;
        await requestRepository.Update(request);
        var applicant = await requestRepository.GetRequestApplicant(ra => ra.RequestId == requestId && ra.WorkerProfileId == workerProfile.Value.Id);
        if (applicant != null) requestRepository.Delete(applicant);
        await requestRepository.SaveChangesAsync();
        var data = await notificationDataRepository.GetWorkerData(requestId, workerId, NotificationType.WorkerHasBeenBooked.Id);
        if (data != null && data.EmailNotification)
        {
            var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnWorkerBook/WorkerTemplate.cshtml", data);
            await emailService.SendEmail(new EmailParams(data.WorkerEmail, "You have been booked", message));
        }
        return result;
    }

    private Result<CompanyProfileJobPositionRate> ToJobPosition(Guid profileId, CompanyProfileJobPositionRateModel model)
    {
        var createdBy = identityServerService.GetNickname();
        Result<CompanyProfileJobPositionRate> result;
        if (model.JobPosition != null && model.JobPosition.Id != Guid.Empty)
        {
            result = CompanyProfileJobPositionRate.Create(profileId, model.JobPosition.Id,
                model.Rate, model.WorkerRate, model.Description, createdBy);
        }
        else
        {
            result = CompanyProfileJobPositionRate.Create(profileId, model.OtherJobPosition,
                model.Rate, model.WorkerRate, model.Description, createdBy);
        }
        if (!result) return result;

        Result rMin = result.Value.UpdateWorkerRateMin(model.WorkerRateMin);
        if (!rMin) return Result.Fail<CompanyProfileJobPositionRate>(rMin.Errors);

        Result rMax = result.Value.UpdateWorkerRateMax(model.WorkerRateMax);
        if (!rMax) return Result.Fail<CompanyProfileJobPositionRate>(rMax.Errors);

        if (model.Shift != null) result.Value.AddShift(model.Shift.ToShift());
        return result;
    }

    private async Task<Result<WorkerProfile>> GetWorkerProfile(Guid workerId, Guid requesId)
    {
        var isShiftAvailableToBook = await IsShiftAvailableToBook(workerId, requesId);
        if (!isShiftAvailableToBook) return Result.Fail<WorkerProfile>(isShiftAvailableToBook.Errors);
        var workerProfile = await workerRepository.GetProfile(wp => wp.WorkerId == workerId);
        if (workerProfile is null) return Result.Fail<WorkerProfile>(ApiResources.WorkerNotFound);
        var canBeBook = workerProfile.CanBeBook(timeService.GetCurrentDateTime());
        return !canBeBook ? Result.Fail<WorkerProfile>(canBeBook.Errors) : Result.Ok(workerProfile);
    }

    private async Task<Result> IsShiftAvailableToBook(Guid workerId, Guid requestId)
    {
        var shiftNewOrder = await requestRepository.GetRequestShift(requestId);
        var activeShifts = (await workerRequestRepository.GetWorkerRequestsByWorkerId(workerId)).Select(wr => wr.Request.Shift);
        var isTimeUsed = false;
        if (shiftNewOrder.Sunday.HasValue)
        {
            isTimeUsed |= activeShifts.Any(s => s.Sunday.HasValue && s.SundayStart < shiftNewOrder.SundayFinish && shiftNewOrder.SundayStart < s.SundayFinish);
        }
        if (shiftNewOrder.Monday.HasValue)
        {
            isTimeUsed |= activeShifts.Any(s => s.Monday.HasValue && s.MondayStart < shiftNewOrder.MondayFinish && shiftNewOrder.MondayStart < s.MondayFinish);
        }
        if (shiftNewOrder.Tuesday.HasValue)
        {
            isTimeUsed |= activeShifts.Any(s => s.Tuesday.HasValue && s.TuesdayStart < shiftNewOrder.TuesdayFinish && shiftNewOrder.TuesdayStart < s.TuesdayFinish);
        }
        if (shiftNewOrder.Wednesday.HasValue)
        {
            isTimeUsed |= activeShifts.Any(s => s.Wednesday.HasValue && s.WednesdayStart < shiftNewOrder.WednesdayFinish && shiftNewOrder.WednesdayStart < s.WednesdayFinish);
        }
        if (shiftNewOrder.Thursday.HasValue)
        {
            isTimeUsed |= activeShifts.Any(s => s.Thursday.HasValue && s.ThursdayStart < shiftNewOrder.ThursdayFinish && shiftNewOrder.ThursdayStart < s.ThursdayFinish);
        }
        if (shiftNewOrder.Friday.HasValue)
        {
            isTimeUsed |= activeShifts.Any(s => s.Friday.HasValue && s.FridayStart < shiftNewOrder.FridayFinish && shiftNewOrder.FridayStart < s.FridayFinish);
        }
        if (shiftNewOrder.Saturday.HasValue)
        {
            isTimeUsed |= activeShifts.Any(s => s.Saturday.HasValue && s.SaturdayStart < shiftNewOrder.SaturdayFinish && shiftNewOrder.SaturdayStart < s.SaturdayFinish);
        }
        return isTimeUsed ? Result.Fail("The worker is associated in other order with the same schedule") : Result.Ok();
    }

    public async Task UpdateWorkerProfileTaxCategory(Guid workerProfileId, WorkerProfileDetailModel model)
    {

        if (model.FederalTaxCategory.HasValue || model.ProvincialTaxCategory.HasValue)
        {
            var workerProfileTaxCategory = await workerRepository.GetWorkerProfileTaxCategory(workerProfileId);
            if (workerProfileTaxCategory != null)
            {
                workerProfileTaxCategory.FederalCategory = model.FederalTaxCategory;
                workerProfileTaxCategory.ProvincialCategory = model.ProvincialTaxCategory;
            }
            else
            {
                var newTaxCategory = new WorkerProfileTaxCategory
                {
                    WorkerProfileId = workerProfileId,
                    FederalCategory = model.FederalTaxCategory,
                    ProvincialCategory = model.ProvincialTaxCategory
                };
                await workerRepository.Create(newTaxCategory);
            }
            await workerRepository.SaveChangesAsync();
        }
    }

    public async Task UpdateWorkerProfileTaxRate(Guid workerProfileId, WorkerProfileDetailModel model)
    {
        var workerProfileTaxCategory = await workerRepository.GetWorkerProfileTaxCategory(workerProfileId);
        if (model.Cpp.HasValue || model.Ei.HasValue)
        {
            if (workerProfileTaxCategory != null)
            {
                workerProfileTaxCategory.Cpp = model.Cpp;
                workerProfileTaxCategory.Ei = model.Ei;
            }
            else
            {
                var newTaxCategory = new WorkerProfileTaxCategory
                {
                    WorkerProfileId = workerProfileId,
                    Cpp = model.Cpp,
                    Ei = model.Ei
                };
                await workerRepository.Create(newTaxCategory);
            }
            await workerRepository.SaveChangesAsync();
        }
    }

    public async Task<Result<CompanyProfileDocument>> CreateCompanyDocument(Guid companyProfileId, CompanyProfileDocumentModel model)
    {
        var covenantFile = CovenantFile.Create(model);
        if (!covenantFile)
        {
            return Result.Fail<CompanyProfileDocument>(covenantFile.Errors);
        }
        var entity = new CompanyProfileDocument(companyProfileId, covenantFile.Value, identityServerService.GetNickname());
        entity.DocumentType = model.DocumentType;
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok(entity);
    }

    public async Task<PaginatedList<CompanyProfileDocumentModel>> GetCompanyDocuments(Guid compnayProfileId, Pagination pagination)
    {
        var agencyPersonnel = await agencyRepository.GetPersonnelByUserId(identityServerService.GetUserId());
        var isAdmin = identityServerService.IsAdmin();
        var companyProfile = await companyRepository.GetCompanyProfile(cp => cp.Id == compnayProfileId);
        var companyDocuments = await companyRepository.GetDocuments(compnayProfileId, pagination);
        foreach (var document in companyDocuments.Items)
        {
            if (document.DocumentType == CompanyProfileDocumentType.Contract)
            {
                document.CanDownload = isAdmin || agencyPersonnel.Any(ap => ap.Id == companyProfile.SalesRepresentativeId);
            }
        }
        return companyDocuments;
    }

    public async Task DeleteCompanyDocument(Guid companyProfileDocumentId)
    {
        var document = await companyRepository.GetDocument(companyProfileDocumentId);
        companyRepository.Delete(document);
        await companyRepository.SaveChangesAsync();
        await documentService.DeleteFile(document.DocumentId);
    }

    public async Task<Result> UpdateEmailCompanyProfile(Guid companyProfileId, UpdateEmailModel model)
    {
        var email = CvnEmail.Create(model.NewEmail);
        if (email)
        {
            var profile = await companyRepository.GetCompanyProfileDetail(cp => cp.Id == companyProfileId);
            if (profile != null)
            {
                var currentUserId = profile.CompanyId;
                var existingUser = await userRepository.GetUserByEmail(email.Value);
                if (existingUser != null)
                {
                    var updateRole = new UpdateRoleModel { Id = currentUserId, UserType = UserType.Company };
                    var roleUpdated = await identityServerService.UpdateUserRole(updateRole);
                    if (roleUpdated)
                    {
                        var agencyPersonnel = agencyRepository.GetPersonnel(existingUser.Id);
                        if (agencyPersonnel == null)
                        {
                            var companyUser = companyRepository.GetCompanyUser(existingUser.Id);
                            if (companyUser != null)
                            {
                                companyRepository.Delete(companyUser);
                                await companyRepository.SaveChangesAsync();
                                return Result.Ok();
                            }
                        }
                        return Result.Fail("This email is associated to an agency user");

                    }
                    return roleUpdated;
                }
                else
                {
                    var updateEmail = new UpdateEmailModel { Id = currentUserId, NewEmail = email.Value };
                    var emailUpdated = await identityServerService.UpdateUserEmail(updateEmail);
                    if (emailUpdated)
                    {
                        var user = await userRepository.GetUserById(currentUserId);
                        user.UpdateEmail(email.Value);
                        userRepository.Update(user);
                        await userRepository.SaveChangesAsync();
                        return Result.Ok();
                    }
                    return emailUpdated;
                }
            }
            else
            {
                return Result.Fail();
            }
        }
        return email;
    }

    public async Task<Result> AddUpdateWorkerHoliday(Guid workerProfileId, WorkerProfileHolidayModel model)
    {
        var entity = new WorkerProfileHoliday(workerProfileId, model.HolidayId, model.StatPaidWorker);
        await workerRepository.CreateWorkerProfileHoliday(entity);
        await workerRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> CreateHoliday(WorkerProfileHolidayModel model)
    {
        var workerProfile = await workerRepository.GetWorkerProfileDetail(model.WorkerProfileId);
        var countryCode = workerProfile.Location.City.Province.Country.Code;
        await catalogRepository.CreateHolidayIfNotExist(countryCode, model.Date);
        await catalogRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> CreateAgencyPersonnel(AgencyPersonnelModel model, Guid? agencyId = null)
    {
        var email = CvnEmail.Create(model.Email);
        if (email)
        {
            agencyId = agencyId ?? identityServerService.GetAgencyId();
            var user = await userRepository.GetUserByEmail(email.Value);
            AgencyPersonnel entity = null;
            if (user == null)
            {
                var newUser = await identityServerService.CreateUser(new CreateUserModel
                {
                    AgencyId = agencyId,
                    Email = email.Value,
                    UserType = UserType.AgencyPersonnel
                });
                if (newUser)
                {
                    entity = AgencyPersonnel.CreatePrimary(agencyId.Value, newUser.Value, model.Name);
                }
                else
                {
                    return Result.Fail(newUser.Errors);
                }
            }
            else
            {
                var list = await agencyRepository.GetPersonnelByUserId(user.Id);
                if (!list.Any() || list.Any(c => c.AgencyId == agencyId))
                {
                    return Result.Fail(ApiResources.EmailAlreadyTaken);
                }
                var result = await identityServerService.UpdateAgencyUser(user.Id, new IdModel(agencyId.Value));
                if (!result)
                {
                    return Result.Fail(result.Errors);
                }
                entity = AgencyPersonnel.Create(agencyId.Value, user, false, model.Name);
            }
            await agencyRepository.Create(entity);
            await agencyRepository.SaveChangesAsync();
            return Result.Ok();
        }
        return Result.Fail(email.Errors);
    }

    public async Task<Result> CreateAgency(AgencyModel model)
    {
        var agencyParentId = identityServerService.GetAgencyId();
        var validator = serviceProvider.GetService<IValidator<AgencyModel>>();
        var agencyValidation = await validator.ValidateAsync(model);
        if (agencyValidation.IsValid)
        {
            var agency = new Agency(model.FullName, model.PhonePrincipal);
            var user = await identityServerService.CreateUser(new CreateUserModel
            {
                Email = model.Email,
                UserType = UserType.Agency,
                AgencyId = agency.Id
            });
            if (user)
            {
                agency.AgencyType = model.AgencyType;
                agency.UserId = user.Value.Id;
                agency.AgencyParentId = agencyParentId;
                var personnel = AgencyPersonnel.CreatePrimary(agency.Id, user.Value, model.FullName);
                await agencyRepository.Create(agency);
                await agencyRepository.Create(personnel);
                await agencyRepository.SaveChangesAsync();
                return Result.Ok();
            }
            return Result.Fail(user.Errors);
        }
        return agencyValidation.ToResultFailure();
    }
}
