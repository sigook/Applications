using Covenant.Api.Validators.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Covenant.Tests.Worker;

public class WorkerProfileCreateModelValidatorTest
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IWorkerRepository> _workerRepository = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly WorkerProfileCreateModelValidator _sut;
    private readonly Guid _sinTypeId = Guid.NewGuid();
    private readonly Guid _regularTypeId = Guid.NewGuid();

    public WorkerProfileCreateModelValidatorTest()
    {
        _catalogRepository.Setup(r => r.GetIdentificationTypeCode(It.IsAny<Guid>())).ReturnsAsync(IdentificationTypeCode.None);
        _catalogRepository.Setup(r => r.GetIdentificationTypeCode(_sinTypeId)).ReturnsAsync(IdentificationTypeCode.SinSsn);
        _catalogRepository.Setup(r => r.GetIdentificationTypeCode(_regularTypeId)).ReturnsAsync(IdentificationTypeCode.DriversLicense);
        _sut = new WorkerProfileCreateModelValidator(_userRepository.Object, _workerRepository.Object, _catalogRepository.Object);
    }

    private WorkerProfileCreateModel CreateModel(Guid typeId, string number) => new()
    {
        IdentificationType1 = new BaseModel<Guid>(typeId, "type"),
        IdentificationNumber1 = number
    };

    [Fact]
    public async Task SinTypedIdentificationFailsWhenSinIsTaken()
    {
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken("123-456-789", null)).ReturnsAsync(true);
        var result = await _sut.TestValidateAsync(CreateModel(_sinTypeId, "123-456-789"));
        result.ShouldHaveValidationErrorFor(c => c.IdentificationNumber1)
            .WithErrorMessage(ApiResources.SocialInsuranceAlreadyTaken);
    }

    [Fact]
    public async Task SinTypedIdentificationFailsWithShortNumber()
    {
        var result = await _sut.TestValidateAsync(CreateModel(_sinTypeId, "12345"));
        result.ShouldHaveValidationErrorFor(c => c.IdentificationNumber1);
    }

    [Fact]
    public async Task SinTypedSlotsWithDifferentNumbersFail()
    {
        var model = CreateModel(_sinTypeId, "123-456-789");
        model.IdentificationType2 = new BaseModel<Guid>(_sinTypeId, "type");
        model.IdentificationNumber2 = "999-888-777";
        var result = await _sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(c => c.IdentificationNumber2)
            .WithErrorMessage(ApiResources.SocialInsuranceConflict);
    }

    [Fact]
    public async Task RegularTypeSkipsSinRules()
    {
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken(It.IsAny<string>(), null)).ReturnsAsync(true);
        var result = await _sut.TestValidateAsync(CreateModel(_regularTypeId, "12345"));
        result.ShouldNotHaveValidationErrorFor(c => c.IdentificationNumber1);
    }
}
