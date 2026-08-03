using System.Text;
using Covenant.Api.Validators.Deduction;
using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Services.Accounting;
using Covenant.Infrastructure.Accounting.Deductions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Covenant.Tests.Accounting.Deductions;

public class DeductionImportServiceTest
{
    private readonly Mock<ICraTablesContainer> _container = new();
    private readonly Mock<IDeductionsRepository> _repository = new();
    private readonly DeductionImportService _sut;
    private IReadOnlyList<CppDeduction> _storedCpp;
    private IReadOnlyList<TaxDeduction> _storedTax;

    public DeductionImportServiceTest()
    {
        _repository.Setup(r => r.ImportCpp(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<CppDeduction>>(), It.IsAny<int>()))
            .Callback<int, PayPeriod, IReadOnlyList<CppDeduction>, int>((_, _, rows, _) => _storedCpp = rows)
            .ReturnsAsync((int _, PayPeriod _, IReadOnlyList<CppDeduction> rows, int _) => rows.Count);
        _repository.Setup(r => r.ImportTax(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<TaxDeduction>>(), It.IsAny<int>()))
            .Callback<int, PayPeriod, IReadOnlyList<TaxDeduction>, int>((_, _, rows, _) => _storedTax = rows)
            .ReturnsAsync((int _, PayPeriod _, IReadOnlyList<TaxDeduction> rows, int _) => rows.Count);

        _sut = new DeductionImportService(_container.Object, new CraPdfParser(), _repository.Object,
            new ImportCraTableFromBlobModelValidator(), NullLogger<DeductionImportService>.Instance);
    }

    [Fact]
    public async Task Stores_Every_Bracket_Of_The_Downloaded_Cpp_Table()
    {
        await GivenTheBlobIs(CraTableFixture.CppWeeklyBlobName, CraTableFixture.CppWeeklyPath);

        var result = await _sut.ImportCppFromBlob(CppModel());

        Assert.True(result.IsSuccess, result.StringErrors);
        Assert.Equal(CraTableFixture.CppWeeklyBrackets, result.Value);
        Assert.Equal(CraTableFixture.CppWeeklyBrackets, _storedCpp.Count);
        Assert.All(_storedCpp, row =>
        {
            Assert.Equal(2026, row.Year);
            Assert.Equal(PayPeriod.Weekly, row.PayPeriod);
        });
    }

    [Fact]
    public async Task Stores_The_Federal_And_The_Provincial_Brackets_Of_The_Downloaded_Tax_Table()
    {
        await GivenTheBlobIs(CraTableFixture.TaxMonthlyBlobName, CraTableFixture.TaxMonthlyPath);

        var result = await _sut.ImportTaxFromBlob(TaxModel());

        Assert.True(result.IsSuccess, result.StringErrors);
        Assert.Equal(CraTableFixture.TaxMonthlyBrackets * 2, result.Value);
        Assert.Equal(CraTableFixture.TaxMonthlyBrackets, _storedTax.Count(r => r.TaxType == TaxType.Federal));
        Assert.Equal(CraTableFixture.TaxMonthlyBrackets, _storedTax.Count(r => r.TaxType == TaxType.Provincial));
        Assert.All(_storedTax, row =>
        {
            Assert.Equal(2026, row.Year);
            Assert.Equal(PayPeriod.Monthly, row.PayPeriod);
        });
    }

    [Fact]
    public async Task Asks_The_Repository_To_Keep_The_Two_Most_Recent_Years()
    {
        await GivenTheBlobIs(CraTableFixture.CppWeeklyBlobName, CraTableFixture.CppWeeklyPath);
        await GivenTheBlobIs(CraTableFixture.TaxMonthlyBlobName, CraTableFixture.TaxMonthlyPath);

        await _sut.ImportCppFromBlob(CppModel());
        await _sut.ImportTaxFromBlob(TaxModel());

        _repository.Verify(r => r.ImportCpp(2026, PayPeriod.Weekly, It.IsAny<IReadOnlyList<CppDeduction>>(), 2), Times.Once);
        _repository.Verify(r => r.ImportTax(2026, PayPeriod.Monthly, It.IsAny<IReadOnlyList<TaxDeduction>>(), 2), Times.Once);
    }

    [Theory]
    [InlineData("CPP WEEKLY 2026.xlsx", 2026)]
    [InlineData("CPP WEEKLY 2026.pdf", 1999)]
    public async Task Rejects_An_Invalid_Model_Without_Touching_The_Storage(string blobName, int year)
    {
        var result = await _sut.ImportCppFromBlob(new ImportCraTableFromBlobModel
        {
            BlobName = blobName,
            PayPeriod = PayPeriod.Weekly,
            Year = year
        });

        Assert.True(result.IsFailure);
        _container.Verify(c => c.DownloadStream(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Fails_When_The_Blob_Is_Missing()
    {
        _container.Setup(c => c.DownloadStream(CraTableFixture.CppWeeklyBlobName)).ReturnsAsync((Stream)null);

        var result = await _sut.ImportCppFromBlob(CppModel());

        Assert.True(result.IsFailure);
        VerifyNothingWasStored();
    }

    [Fact]
    public async Task Keeps_The_Stored_Table_When_The_File_Is_Not_A_Readable_Pdf()
    {
        GivenTheBlobIs(CraTableFixture.TaxMonthlyBlobName, Encoding.UTF8.GetBytes("not a pdf"));

        var result = await _sut.ImportTaxFromBlob(TaxModel());

        Assert.True(result.IsFailure);
        VerifyNothingWasStored();
    }

    [Fact]
    public async Task Keeps_The_Stored_Table_When_The_Pdf_Is_Not_The_Expected_One()
    {
        await GivenTheBlobIs(CraTableFixture.TaxMonthlyBlobName, CraTableFixture.CppWeeklyPath);

        var result = await _sut.ImportTaxFromBlob(TaxModel());

        Assert.True(result.IsFailure);
        VerifyNothingWasStored();
    }

    private async Task GivenTheBlobIs(string blobName, string fixturePath) =>
        GivenTheBlobIs(blobName, await File.ReadAllBytesAsync(fixturePath));

    private void GivenTheBlobIs(string blobName, byte[] content) =>
        _container.Setup(c => c.DownloadStream(blobName)).ReturnsAsync(() => new MemoryStream(content));

    private void VerifyNothingWasStored()
    {
        _repository.Verify(r => r.ImportCpp(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<CppDeduction>>(), It.IsAny<int>()), Times.Never);
        _repository.Verify(r => r.ImportTax(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<TaxDeduction>>(), It.IsAny<int>()), Times.Never);
    }

    private static ImportCraTableFromBlobModel CppModel() =>
        new() { BlobName = CraTableFixture.CppWeeklyBlobName, PayPeriod = PayPeriod.Weekly, Year = 2026 };

    private static ImportCraTableFromBlobModel TaxModel() =>
        new() { BlobName = CraTableFixture.TaxMonthlyBlobName, PayPeriod = PayPeriod.Monthly, Year = 2026 };
}
