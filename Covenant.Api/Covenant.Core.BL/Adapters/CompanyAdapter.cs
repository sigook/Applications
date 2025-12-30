using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Security;
using Covenant.Common.Resources;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Services;
using System;

namespace Covenant.Core.BL.Adapters;

public class CompanyAdapter : ICompanyAdapter
{

    public readonly IIdentityServerService identityServerService;

    public CompanyAdapter(IIdentityServerService identityServerService)
    {
        this.identityServerService = identityServerService;
    }

    public async Task<BulkCompany> ConvertCompanyCsvToCompanyBulk(CompanyCsvModel model, Guid agencyId, BaseModel<Guid> industry, CityModel city)
    {

        var bulkCompany = new BulkCompany();

        var user = await identityServerService.CreateUser(new CreateUserModel
        {
            Email = model.ContactEmail,
            UserType = UserType.Company
        });

        var createdBy = identityServerService.GetNickname();

        var rIndustry = CompanyProfileIndustry.Create(industry);

        var profile = CompanyProfile.AgencyCreateCompany(
            agencyId: agencyId,
            user: user.Value,
            fullName: CompanyName.Create(model.CompanyName).Value,
            businessName: CompanyName.Create(model.CompanyName).Value,
            phone: model.CompanyHQPhone,
            phoneExt: null,
            fax: model.Fax,
            faxExt: null,
            webSite: model.Website,
            industry: rIndustry.Value,
            logo: null,
            about: string.Empty,
            internalInfo: string.Empty,
            requiresPermissionToSeeOrders: true,
            createdBy: createdBy,
            companyStatus: CompanyStatus.Potential,
            salesRepresentativeId: null).Value;
        
        bulkCompany.CompanyProfile = profile;

        var location = Location.Create(cityId: city.Id, address: model.FullAddress, postalCode: model.CompanyZipCode);
        var companyProfileLocation = new CompanyProfileLocation(profile.Id, location.Value);
        bulkCompany.CompanyLocation = companyProfileLocation;


        var contactPerson = ConvertCompanyCsvToContactPerson(model, profile.Id);

        bulkCompany.ContactPerson = contactPerson;

        return bulkCompany;
    }

    private CompanyProfileContactPerson ConvertCompanyCsvToContactPerson(CompanyCsvModel model, Guid companyProfileId)
    {
            var rEmail = CvnEmail.Create(model.ContactEmail);
            var entity = new CompanyProfileContactPerson(companyProfileId)
            {
                Title = model.Title,
                FirstName = model.ContactName,
                MiddleName = string.Empty,
                LastName = string.Empty,
                Position = model.Title,
                MobileNumber = model.CompanyHQPhone,
                OfficeNumber = string.Empty,
                Email = rEmail.Value
            };

            return entity;  
    }
}
