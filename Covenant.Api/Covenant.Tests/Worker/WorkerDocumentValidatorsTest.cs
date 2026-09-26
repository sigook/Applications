using Covenant.Api.Validators.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using FluentValidation.TestHelper;
using Xunit;

namespace Covenant.Tests.Worker;

public class WorkerDocumentValidatorsTest
{
    private readonly WorkerProfileLicenseModelValidator _licenseValidator = new();
    private readonly WorkerDocumentFileValidator _documentValidator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("First Aid!")]
    public void LicenseWithInvalidDescriptionFails(string description)
    {
        var model = new WorkerProfileLicenseModel { License = new CovenantFileModel("license.pdf", description) };
        _licenseValidator.TestValidate(model).ShouldHaveValidationErrorFor(l => l.License.Description);
    }

    [Fact]
    public void LicenseWithValidDescriptionPasses()
    {
        var model = new WorkerProfileLicenseModel { License = new CovenantFileModel("license.pdf", "Forklift G-2") };
        _licenseValidator.TestValidate(model).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DocumentDescriptionLongerThan100CharactersFails()
    {
        var model = new CovenantFileModel("cert.pdf", new string('a', 101));
        _documentValidator.TestValidate(model).ShouldHaveValidationErrorFor(d => d.Description);
    }

    [Fact]
    public void DocumentWithValidDescriptionPasses()
    {
        _documentValidator.TestValidate(new CovenantFileModel("cert.pdf", "WHMIS 2025")).ShouldNotHaveAnyValidationErrors();
    }
}
