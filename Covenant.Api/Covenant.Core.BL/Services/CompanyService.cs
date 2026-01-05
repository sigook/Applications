using ClosedXML.Excel;
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
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Adapters;
using Covenant.Core.BL.Interfaces;
using Covenant.Infrastructure.Repositories.Candidate;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Covenant.Core.BL.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository companyRepository;
    private readonly IUserRepository userRepository;
    private readonly IAgencyRepository agencyRepository;
    private readonly INotificationDataRepository notificationDataRepository;
    private readonly ICatalogRepository catalogRepository;
    private readonly IRequestRepository requestRepository;
    private readonly IInvoiceRepository invoiceRepository;
    private readonly IIdentityServerService identityServerService;
    private readonly IEmailService emailService;
    private readonly IGeocodeService geocodeService;
    private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
    private readonly IValidator<CompanyCsvModel> bulkCompanyValidator;
    private readonly ICompanyAdapter companyAdapter;
    private readonly ILocationService locationService;
    public CompanyService(
        ICompanyRepository companyRepository,
        IUserRepository userRepository,
        IAgencyRepository agencyRepository,
        INotificationDataRepository notificationDataRepository,
        ICatalogRepository catalogRepository,
        IRequestRepository requestRepository,
        IInvoiceRepository invoiceRepository,
        IIdentityServerService identityServerService,
        IEmailService emailService,
        IGeocodeService geocodeService,
        IRazorViewToStringRenderer razorViewToStringRenderer,
        IValidator<CompanyCsvModel> bulkCompanyValidator,
        ICompanyAdapter companyAdapter,
        ILocationService locationService)
    {
        this.companyRepository = companyRepository;
        this.userRepository = userRepository;
        this.agencyRepository = agencyRepository;
        this.notificationDataRepository = notificationDataRepository;
        this.catalogRepository = catalogRepository;
        this.requestRepository = requestRepository;
        this.invoiceRepository = invoiceRepository;
        this.identityServerService = identityServerService;
        this.emailService = emailService;
        this.geocodeService = geocodeService;
        this.razorViewToStringRenderer = razorViewToStringRenderer;
        this.bulkCompanyValidator = bulkCompanyValidator;
        this.companyAdapter = companyAdapter;
        this.locationService = locationService;
    }

    public async Task<Result<Guid>> CreateCompanyProfile(CompanyRegisterByItselfModel model)
    {
        var email = CvnEmail.Create(model.Email);
        if (!email) return Result.Fail<Guid>(email.Errors);
        if (await userRepository.UserExists(email.Value.Email)) return Result.Fail<Guid>(ApiResources.EmailAlreadyTaken);
        var user = await identityServerService.CreateUser(new CreateUserModel
        {
            Email = email.Value,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            UserType = UserType.Company
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
            model.JobPositionRates.Select(j => (j.JobPosition?.Id, j.OtherJobPosition)).ToList(),
            model.Logo.FileName);
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
        entity.FullName = model.FullName;
        entity.BusinessName = model.BusinessName;
        entity.Phone = model.Phone;
        entity.PhoneExt = model.PhoneExt;
        entity.Fax = model.Fax;
        entity.FaxExt = model.FaxExt;
        entity.Website = model.Website;
        entity.Industry.IndustryId = model.Industry.Industry.Id;
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Guid>> CreateCompanyLocation(CompanyProfileLocationDetailModel model, Guid? profileId = null)
    {
        if (!profileId.HasValue)
        {
            var companyId = identityServerService.GetCompanyId();
            var profile = await companyRepository.GetCompanyProfileDetail(cp => cp.CompanyId == companyId);
            profileId = profile.Id;
        }
        var location = await CreateLocation(model);
        if (!location)
        {
            return Result.Fail<Guid>(location.Errors);
        }
        var entity = new CompanyProfileLocation(profileId.Value, location.Value)
        {
            IsBilling = model.IsBilling
        };
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();

        await UpsertProvinceSettingsIfProvided(model);

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
        entity.IsBilling = model.IsBilling;
        companyRepository.Update(entity);
        await companyRepository.SaveChangesAsync();

        await UpsertProvinceSettingsIfProvided(model);

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
            location.Value.Latitude = geocodeLocation.Lat;
            location.Value.Longitude = geocodeLocation.Lng;
        }
        return location;
    }

    public async Task<Result> RequestNewJobPosition(ContactDto contact)
    {
        var companyId = identityServerService.GetCompanyId();
        var companyProfile = await companyRepository.GetCompanyProfile(cp => cp.CompanyId == companyId);
        contact.Email = companyProfile.Company.Email;
        var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnContactAgency/AgencyTemplate.cshtml", contact);
        await emailService.SendEmail(new EmailParams(companyProfile.Agency.User.Email, "Company request new type of job", message));
        return Result.Ok();
    }

    public async Task<Result> RequestNewWorker(Guid requestId, CommentsModel model)
    {
        var companyId = identityServerService.GetCompanyId();
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

    public async Task<PaginatedList<InvoiceListModel>> GetCompanyInvoices(GetCompanyInvoiceFilter filter)
    {
        var companyId = identityServerService.GetCompanyId();
        var companyDetail = await companyRepository.GetCompanyProfileDetail(cp => cp.CompanyId == companyId);
        var locations = await companyRepository.GetCompanyLocations(c => c.CompanyProfile.CompanyId == companyId);
        var mainLocation = locations.FirstOrDefault(l => l.IsBilling);
        PaginatedList<InvoiceListModel> result;
        if (mainLocation.IsUSA)
        {
            result = await invoiceRepository.GetInvoicesForCompanyUSA(companyId, filter);
        }
        else
        {
            result = await invoiceRepository.GetInvoicesForCompany(companyId, filter);
        }
        return result;
    }

    public async Task<Result> CreateCompanyUser(CompanyUserModel model, Guid? companyId = null)
    {
        var email = CvnEmail.Create(model.Email);
        if (email)
        {
            var user = await userRepository.GetUserByEmail(email.Value);
            if (user == null)
            {
                companyId ??= identityServerService.GetCompanyId();
                var userModel = new CreateUserModel
                {
                    Email = email.Value,
                    CompanyId = companyId,
                    UserType = UserType.CompanyUser
                };
                var newUser = await identityServerService.CreateUser(userModel);
                if (newUser)
                {
                    var entity = new CompanyUser(companyId.Value, newUser.Value)
                    {
                        Name = model.Name,
                        Lastname = model.Lastname,
                        Position = model.Position,
                        MobileNumber = model.MobileNumber,
                    };
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

    public async Task<Result> DeleteCompanyUser(Guid userId, Guid? companyId = null)
    {
        companyId ??= identityServerService.GetCompanyId();
        var entity = await companyRepository.GetCompanyUser(userId);
        if (entity != null)
        {
            companyRepository.Delete(entity);
            await companyRepository.SaveChangesAsync();
            var identityServiceResult = await identityServerService.DeleteUserOrClaim(userId, companyId);
            if (identityServiceResult)
            {
                return Result.Ok();
            }
            return identityServiceResult;
        }
        return Result.Fail("User not found");
    }

    public async Task<Result<ResultGenerateDocument<byte[]>>> BulkCompany(Guid agencyId, IFormFile file)
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

                if(city is null)
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
        var profile = await companyRepository.GetCompanyProfile(cp => cp.CompanyId == identityServerService.GetCompanyId());
        var entity = new CompanyProfileContactPerson(profile.Id)
        {
            Title = model.Title,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            Position = model.Position,
            Email = model.Email,
            MobileNumber = model.MobileNumber,
            OfficeNumber = model.OfficeNumber,
            OfficeNumberExt = model.OfficeNumberExt,
        };
        await companyRepository.Create(entity);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task UpsertProvinceSettingsIfProvided(CompanyProfileLocationDetailModel model)
    {
        var provinceId = model.Province?.Id ?? model.City?.Province?.Id;
        var settings = model.Province?.Settings ?? model.City?.Province?.Settings;

        if (provinceId.HasValue && provinceId.Value != Guid.Empty && settings != null)
        {
            await locationService.UpsertProvinceSettings(provinceId.Value, settings);
        }
    }
}
