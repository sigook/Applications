using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models.Company.Agency;
using Covenant.Common.Models.Identity;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Resources;
using System;

namespace Covenant.Core.BL.Adapters;

public class CompanyAdapter(IUserAccountService userAccountService, ICurrentUserService currentUserService) : ICompanyAdapter
{
    public async Task<BulkCompany> ConvertCompanyCsvToCompanyBulk(CompanyCsvModel model, Guid agencyId, BaseModel<Guid> industry, CityModel city)
    {

        var bulkCompany = new BulkCompany();

        var user = await userAccountService.CreateUser(new CreateUserModel
        {
            Email = model.ContactEmail,
            UserType = UserType.Company,
            Role = CovenantConstants.Role.Company
        });

        var createdBy = currentUserService.GetNickname();

        var rIndustry = CompanyProfileIndustry.Create(industry);

        var profile = CompanyProfile.AgencyCreateCompany(
            agencyId: agencyId,
            user: user.Value,
            fullName: CompanyName.Create(model.CompanyName).Value,
            phone: model.CompanyHQPhone,
            phoneExt: null,
            fax: model.Fax,
            faxExt: null,
            webSite: model.Website,
            industry: rIndustry.Value,
            logo: null,
            about: string.Empty,
            internalInfo: string.Empty,
            requiresPermissionToSeeRequests: true,
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
            var entity = new CompanyProfileContactPerson(companyProfileId);
            entity.Update(model.Title, model.ContactName, string.Empty, string.Empty, model.Title, model.CompanyHQPhone, string.Empty, null, rEmail.Value);
            return entity;
    }
}
