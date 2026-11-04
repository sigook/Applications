using Covenant.Common.Configuration;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting.Invoices;
using Covenant.Infrastructure.Repositories.Accounting.PayStubs;
using Covenant.Infrastructure.Repositories.Notifications;
using Covenant.Infrastructure.Repositories.Requests;
using Covenant.Infrastructure.Repositories.Workers;
using Covenant.Integration.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace Covenant.Integration.Tests.Infrastructure;

public class LeftJoinQueriesTest
{
    private static CovenantContext CreateContext() =>
        new(new DbContextOptionsBuilder<CovenantContext>().UseNpgsql(PostgresTestDatabase.CreateDatabase()).Options);

    [Fact]
    public async Task NotificationDataQueries_TranslateToSql()
    {
        await using var context = CreateContext();
        var sut = new NotificationDataRepository(context);
        var id = Guid.NewGuid();

        Assert.Null(await sut.GetAgencyData(id, id, 1));
        Assert.Null(await sut.GetAgencyData(id, 1));
        Assert.Null(await sut.GetCompanyData(id, 1));
        Assert.Null(await sut.GetCompanyData(id, id, 1));
        Assert.Null(await sut.GetWorkerData(id, id, 1));
        Assert.Null(await sut.GetWorkerData(id, 1));
    }

    [Fact]
    public async Task NotificationPreferencesQuery_TranslatesToSql()
    {
        await using var context = CreateContext();

        var result = await new NotificationRepository(context).Get(Guid.NewGuid(), Enum.GetValues<NotificationTarget>());

        Assert.All(result, item => Assert.False(item.EmailNotification));
    }

    [Fact]
    public async Task InvoiceSummaryQuery_TranslatesToSql()
    {
        await using var context = CreateContext();

        Assert.Null(await new InvoiceRepository(context).GetInvoiceSummaryById(Guid.NewGuid()));
    }

    [Fact]
    public async Task RequestDetailQuery_TranslatesToSql()
    {
        await using var context = CreateContext();
        var sut = new RequestRepository(context, Options.Create(new FilesConfiguration()));

        Assert.Null(await sut.GetRequestDetailForAgency(Guid.NewGuid()));
    }

    [Fact]
    public async Task WorkerHolidaysQuery_TranslatesToSql()
    {
        await using var context = CreateContext();
        var sut = new WorkerRepository(context, Options.Create(new FilesConfiguration()));

        Assert.Empty(await sut.GetWorkerProfileHoliday(Guid.NewGuid()));
    }

    [Fact]
    public async Task PayStubYearsQuery_TranslatesToSql()
    {
        await using var context = CreateContext();

        Assert.Empty(await new PayStubRepository(null, context).GetPayStubYearsForWorker(Guid.NewGuid()));
    }
}
