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

public class CppDeductionImportServiceTest
{
    private const string BlobName = "CPP WEEKLY 2026.pdf";
    private const int WeeklyBrackets2026 = 8928;
    private static readonly string FixturePath =
        Path.Combine(Directory.GetCurrentDirectory(), "Accounting", "Deductions", "cpp_weekly_2026.pdf");

    private readonly Mock<ICraTablesContainer> _container = new();
    private readonly Mock<IDeductionsRepository> _repository = new();
    private readonly CppDeductionImportService _sut;
    private IReadOnlyList<CppDeduction> _stored;

    public CppDeductionImportServiceTest()
    {
        _repository.Setup(r => r.ImportCpp(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<CppDeduction>>(), It.IsAny<int>()))
            .Callback<int, PayPeriod, IReadOnlyList<CppDeduction>, int>((_, _, rows, _) => _stored = rows)
            .ReturnsAsync((int _, PayPeriod _, IReadOnlyList<CppDeduction> rows, int _) => rows.Count);

        _sut = new CppDeductionImportService(_container.Object, new CppPdfParser(), _repository.Object,
            new ImportCppFromBlobModelValidator(), NullLogger<CppDeductionImportService>.Instance);
    }

    [Fact]
    public async Task Stores_Every_Bracket_Of_The_Downloaded_Table()
    {
        GivenTheBlobIs(await File.ReadAllBytesAsync(FixturePath));

        var result = await _sut.ImportFromBlob(Model());

        Assert.True(result.IsSuccess, result.StringErrors);
        Assert.Equal(WeeklyBrackets2026, result.Value);
        Assert.Equal(WeeklyBrackets2026, _stored.Count);
        Assert.All(_stored, row =>
        {
            Assert.Equal(2026, row.Year);
            Assert.Equal(PayPeriod.Weekly, row.PayPeriod);
        });
    }

    [Fact]
    public async Task Asks_The_Repository_To_Keep_The_Two_Most_Recent_Years()
    {
        GivenTheBlobIs(await File.ReadAllBytesAsync(FixturePath));

        await _sut.ImportFromBlob(Model());

        _repository.Verify(r => r.ImportCpp(2026, PayPeriod.Weekly, It.IsAny<IReadOnlyList<CppDeduction>>(), 2), Times.Once);
    }

    [Theory]
    [InlineData("CPP WEEKLY 2026.xlsx", 2026)]
    [InlineData("CPP WEEKLY 2026.pdf", 1999)]
    public async Task Rejects_An_Invalid_Model_Without_Touching_The_Storage(string blobName, int year)
    {
        var result = await _sut.ImportFromBlob(new ImportCppFromBlobModel
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
        _container.Setup(c => c.DownloadStream(BlobName)).ReturnsAsync((Stream)null);

        var result = await _sut.ImportFromBlob(Model());

        Assert.True(result.IsFailure);
        VerifyNothingWasStored();
    }

    [Fact]
    public async Task Keeps_The_Stored_Table_When_The_File_Is_Not_A_Readable_Pdf()
    {
        GivenTheBlobIs(Encoding.UTF8.GetBytes("not a pdf"));

        var result = await _sut.ImportFromBlob(Model());

        Assert.True(result.IsFailure);
        VerifyNothingWasStored();
    }

    private void GivenTheBlobIs(byte[] content) =>
        _container.Setup(c => c.DownloadStream(BlobName)).ReturnsAsync(() => new MemoryStream(content));

    private void VerifyNothingWasStored() =>
        _repository.Verify(r => r.ImportCpp(It.IsAny<int>(), It.IsAny<PayPeriod>(), It.IsAny<IReadOnlyList<CppDeduction>>(), It.IsAny<int>()), Times.Never);

    private static ImportCppFromBlobModel Model() =>
        new() { BlobName = BlobName, PayPeriod = PayPeriod.Weekly, Year = 2026 };
}
