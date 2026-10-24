using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Identity;
using Covenant.Common.Models.Locations;
using Covenant.Core.BL.Adapters;
using Moq;
using Xunit;

namespace Covenant.Tests.Companies;

public class CompanyAdapterTest
{
    private readonly Mock<IUserAccountService> _userAccountService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly CompanyAdapter _sut;

    public CompanyAdapterTest()
    {
        _currentUserService.Setup(c => c.GetNickname()).Returns("recruiter");
        _sut = new CompanyAdapter(_userAccountService.Object, _currentUserService.Object);
    }

    [Fact]
    public async Task ConvertCompanyCsvToCompanyBulk_BuildsProfileLocationAndContactPerson()
    {
        var agencyId = Guid.NewGuid();
        var city = new CityModel(Guid.NewGuid(), "Toronto");
        var user = new User(CvnEmail.Create("contact@acme.com").Value, Guid.NewGuid());
        CreateUserModel created = null;
        _userAccountService.Setup(u => u.CreateUser(It.IsAny<CreateUserModel>()))
            .Callback<CreateUserModel>(m => created = m)
            .ReturnsAsync(Result.Ok(user));
        var model = new CompanyCsvModel
        {
            CompanyName = "Acme Inc",
            Website = "https://acme.com",
            CompanyHQPhone = "4165550000",
            Fax = "4165550001",
            CompanyZipCode = "M5V 2T6",
            FullAddress = "100 King St",
            ContactName = "John Contact",
            ContactEmail = "contact@acme.com",
            Title = "Manager"
        };

        var result = await _sut.ConvertCompanyCsvToCompanyBulk(model, agencyId, new BaseModel<Guid>(Guid.NewGuid(), "Construction"), city);

        Assert.Equal("contact@acme.com", created.Email);
        Assert.Equal(UserType.Company, created.UserType);
        Assert.Equal(CovenantConstants.Role.Company, created.Role);
        Assert.Equal("Acme Inc", result.CompanyProfile.FullName);
        Assert.Equal(agencyId, result.CompanyProfile.AgencyId);
        Assert.Equal(CompanyStatus.Potential, result.CompanyProfile.CompanyStatus);
        Assert.True(result.CompanyProfile.RequiresPermissionToSeeRequests);
        Assert.Equal(result.CompanyProfile.Id, result.CompanyLocation.CompanyProfileId);
        Assert.Equal(city.Id, result.CompanyLocation.Location.CityId);
        Assert.Equal("100 King St", result.CompanyLocation.Location.Address);
        Assert.Equal(result.CompanyProfile.Id, result.ContactPerson.CompanyProfileId);
        Assert.Equal("John Contact", result.ContactPerson.FirstName);
        Assert.Equal("contact@acme.com", result.ContactPerson.Email);
    }
}
