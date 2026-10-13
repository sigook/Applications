using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.Infrastructure;

public class DatabaseScriptRunnerTest
{
    private const string AlignScript = "20261013_AlignInvoicePaymentsFromSpreadsheet.sql";

    private static CovenantContext CreateContext(string connectionString) =>
        new(new DbContextOptionsBuilder<CovenantContext>().UseNpgsql(connectionString).Options);

    private static Invoice Invoice(CompanyProfile company, string number, decimal totalNet, DateTime? updatedAt = null, int day = 5) => new()
    {
        CompanyProfile = company,
        CompanyProfileId = company.Id,
        InvoiceNumberId = long.Parse(number.Split('-')[1]),
        InvoiceNumber = number,
        TotalNet = totalNet,
        SubTotal = totalNet,
        CreatedAt = new DateTime(2026, 1, day),
        UpdatedAt = updatedAt
    };

    [Fact]
    public async Task TemplateDatabase_RecordsDataScriptsInScriptHistory()
    {
        await using var context = CreateContext(PostgresTestDatabase.CreateDatabase());
        var applied = await context.Database.SqlQueryRaw<string>("""SELECT "Name" AS "Value" FROM "ScriptHistory";""").ToListAsync();
        Assert.Contains(AlignScript, applied);
    }

    [Fact]
    public async Task AlignInvoicePayments_MatchesByNumberAndAmount_AndRunsOnce()
    {
        var connectionString = PostgresTestDatabase.CreateDatabase();
        var city = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("USA")));
        var agency = FakeData.FakeAgency(city: city);
        var moca = FakeData.FakeCompanyProfile(agency, "MOCA", city, companyEmail: "moca@test.com");
        var tforce = FakeData.FakeCompanyProfile(agency, "TForce", city, companyEmail: "tforce@test.com");
        var paidInSpreadsheet = Invoice(moca, "US-1316-26", 7502.37m);
        var sameNumberOtherAmount = Invoice(moca, "US-1316-26", 1m);
        var editedByUser = Invoice(tforce, "AI-9430-26", 216.42m, updatedAt: new DateTime(2026, 3, 1));
        var tforceLatest = Invoice(tforce, "AI-9431-26", 530.21m, day: 6);

        await using (var seed = CreateContext(connectionString))
        {
            seed.Cities.Add(city);
            seed.CompanyProfiles.AddRange(moca, tforce);
            seed.Invoices.AddRange(paidInSpreadsheet, sameNumberOtherAmount, editedByUser, tforceLatest);
            await seed.SaveChangesAsync();
            await seed.Database.ExecuteSqlRawAsync("""DELETE FROM "ScriptHistory" WHERE "Name" = {0};""", AlignScript);
            await DatabaseScriptRunner.RunAsync(seed);
        }

        await using (var context = CreateContext(connectionString))
        {
            var paid = await context.Invoices.SingleAsync(i => i.Id == paidInSpreadsheet.Id);
            Assert.Equal(InvoiceStatus.Paid, paid.Status);
            Assert.Equal(new DateTime(2026, 2, 27, 5, 0, 0), paid.PaidAt);
            Assert.NotNull(paid.UpdatedAt);
            Assert.Null(paid.UpdatedBy);

            Assert.Equal(InvoiceStatus.Pending, (await context.Invoices.SingleAsync(i => i.Id == sameNumberOtherAmount.Id)).Status);
            Assert.Equal(InvoiceStatus.Pending, (await context.Invoices.SingleAsync(i => i.Id == editedByUser.Id)).Status);
            Assert.Equal(InvoiceStatus.Paid, (await context.Invoices.SingleAsync(i => i.Id == tforceLatest.Id)).Status);

            Assert.Equal(30, (await context.CompanyProfiles.SingleAsync(c => c.Id == moca.Id)).PaymentTermDays);
            Assert.Equal(7, (await context.CompanyProfiles.SingleAsync(c => c.Id == tforce.Id)).PaymentTermDays);

            await context.Database.ExecuteSqlRawAsync("""UPDATE "Invoices" SET "Status" = 'Pending', "PaidAt" = NULL, "UpdatedAt" = NULL WHERE "Id" = {0};""", paidInSpreadsheet.Id);
            await DatabaseScriptRunner.RunAsync(context);
        }

        await using (var context = CreateContext(connectionString))
        {
            Assert.Equal(InvoiceStatus.Pending, (await context.Invoices.SingleAsync(i => i.Id == paidInSpreadsheet.Id)).Status);
        }
    }
}
