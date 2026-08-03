using System.Text;
using Covenant.Api.Validators.Deduction;
using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Accounting;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Services.Accounting;
using Covenant.Infrastructure.Services;
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

        _sut = Service(new CraPdfParser());
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
        _container.Verify(c => c.Download(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Fails_When_The_Blob_Is_Missing()
    {
        _container.Setup(c => c.Download(CraTableFixture.CppWeeklyBlobName)).ReturnsAsync((byte[])null);

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

    [Fact]
    public async Task Stores_A_Contiguous_Cpp_Table()
    {
        var result = await ImportCpp(CppTable());

        Assert.True(result.IsSuccess, result.StringErrors);
    }

    [Fact]
    public async Task Rejects_An_Empty_Cpp_Table() => await AssertCppIsRejected([]);

    [Fact]
    public async Task Rejects_A_Short_Cpp_Table() => await AssertCppIsRejected(CppTable(10));

    [Fact]
    public async Task Rejects_A_Cpp_Table_That_Does_Not_Start_At_Zero()
    {
        var rows = CppTable();
        rows[0] = rows[0] with { From = 0.01m };

        await AssertCppIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Gap_Between_Cpp_Brackets()
    {
        var rows = CppTable();
        rows.RemoveAt(500);

        await AssertCppIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Duplicated_Cpp_Bracket()
    {
        var rows = CppTable();
        rows.Insert(500, rows[500]);

        await AssertCppIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Contribution_That_Goes_Down()
    {
        var rows = CppTable();
        rows[500] = rows[500] with { Cpp = 0m };

        await AssertCppIsRejected(rows);
    }

    [Fact]
    public async Task Stores_A_Contiguous_Tax_Table()
    {
        var result = await ImportTax(TaxTable());

        Assert.True(result.IsSuccess, result.StringErrors);
    }

    [Fact]
    public async Task Rejects_An_Empty_Tax_Table() => await AssertTaxIsRejected([]);

    [Fact]
    public async Task Rejects_A_Short_Tax_Table() => await AssertTaxIsRejected(TaxTable(10));

    [Fact]
    public async Task Rejects_A_File_Without_The_Provincial_Table()
    {
        var rows = TaxTable();
        rows.RemoveAll(r => r.TaxType == TaxType.Provincial);

        await AssertTaxIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Tax_Table_That_Does_Not_Start_At_Zero()
    {
        var rows = TaxTable();
        rows[0] = rows[0] with { From = 10m };

        await AssertTaxIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Gap_Between_Tax_Brackets()
    {
        var rows = TaxTable();
        rows.RemoveAt(100);

        await AssertTaxIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Duplicated_Tax_Bracket()
    {
        var rows = TaxTable();
        rows.Insert(100, rows[100]);

        await AssertTaxIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_An_Amount_That_Goes_Down()
    {
        var rows = TaxTable();
        rows[100] = WithClaimCode(rows[100], 0, 0m);

        await AssertTaxIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Claim_Code_That_Loses_Its_Amount()
    {
        var rows = TaxTable();
        rows[100] = WithClaimCode(rows[100], 0, null);

        await AssertTaxIsRejected(rows);
    }

    [Fact]
    public async Task Rejects_A_Bracket_Without_Any_Amount()
    {
        var rows = TaxTable();
        rows[100] = rows[100] with { ClaimCodes = new decimal?[TaxRow.ClaimCodeCount] };

        await AssertTaxIsRejected(rows);
    }

    private async Task AssertCppIsRejected(List<CppRow> rows)
    {
        var result = await ImportCpp(rows);

        Assert.True(result.IsFailure);
        VerifyNothingWasStored();
    }

    private async Task AssertTaxIsRejected(List<TaxRow> rows)
    {
        var result = await ImportTax(rows);

        Assert.True(result.IsFailure);
        VerifyNothingWasStored();
    }

    private Task<Result<int>> ImportCpp(IReadOnlyList<CppRow> rows)
    {
        var parser = new Mock<ICraPdfParser>();
        parser.Setup(p => p.ParseCpp(It.IsAny<byte[]>())).Returns(rows);
        GivenTheBlobIs(CraTableFixture.CppWeeklyBlobName, Encoding.UTF8.GetBytes("%PDF"));
        return Service(parser.Object).ImportCppFromBlob(CppModel());
    }

    private Task<Result<int>> ImportTax(IReadOnlyList<TaxRow> rows)
    {
        var parser = new Mock<ICraPdfParser>();
        parser.Setup(p => p.ParseTax(It.IsAny<byte[]>())).Returns(rows);
        GivenTheBlobIs(CraTableFixture.TaxMonthlyBlobName, Encoding.UTF8.GetBytes("%PDF"));
        return Service(parser.Object).ImportTaxFromBlob(TaxModel());
    }

    private DeductionImportService Service(ICraPdfParser parser) =>
        new(_container.Object, parser, _repository.Object,
            new ImportCraTableFromBlobModelValidator(), NullLogger<DeductionImportService>.Instance);

    private async Task GivenTheBlobIs(string blobName, string fixturePath) =>
        GivenTheBlobIs(blobName, await File.ReadAllBytesAsync(fixturePath));

    private void GivenTheBlobIs(string blobName, byte[] content) =>
        _container.Setup(c => c.Download(blobName)).ReturnsAsync(content);

    private void VerifyNothingWasStored()
    {
        _repository.Verify(r => r.ImportCpp(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<CppDeduction>>(), It.IsAny<int>()), Times.Never);
        _repository.Verify(r => r.ImportTax(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<TaxDeduction>>(), It.IsAny<int>()), Times.Never);
    }

    private static TaxRow WithClaimCode(TaxRow row, int claimCode, decimal? amount)
    {
        var claimCodes = row.ClaimCodes.ToArray();
        claimCodes[claimCode] = amount;
        return row with { ClaimCodes = claimCodes };
    }

    private static List<CppRow> CppTable(int brackets = 2000) =>
        [.. Enumerable.Range(0, brackets).Select(index => new CppRow(index * 0.10m, index * 0.10m + 0.09m, index * 0.01m))];

    private static List<TaxRow> TaxTable(int brackets = 300) =>
    [
        .. Enum.GetValues<TaxType>().SelectMany(taxType => Enumerable.Range(0, brackets)
            .Select(index => new TaxRow(taxType, index * 10m, (index + 1) * 10m, ClaimCodes(index))))
    ];

    private static decimal?[] ClaimCodes(int index) =>
    [
        .. Enumerable.Range(0, TaxRow.ClaimCodeCount)
            .Select(claimCode => index < claimCode ? (decimal?)null : (index - claimCode) * 0.25m)
    ];

    private static ImportCraTableFromBlobModel CppModel() =>
        new() { BlobName = CraTableFixture.CppWeeklyBlobName, PayPeriod = PayPeriod.Weekly, Year = 2026 };

    private static ImportCraTableFromBlobModel TaxModel() =>
        new() { BlobName = CraTableFixture.TaxMonthlyBlobName, PayPeriod = PayPeriod.Monthly, Year = 2026 };
}
