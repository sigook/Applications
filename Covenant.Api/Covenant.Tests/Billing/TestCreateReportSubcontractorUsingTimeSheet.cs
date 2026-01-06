using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Subcontractor.Services;
using Moq;
using Xunit;

namespace Covenant.Tests.Billing;

public class TestCreateReportSubcontractorUsingTimeSheet
{
    private readonly Mock<ICatalogRepository> _catalogRepository;
    private readonly Mock<ISubcontractorRepository> _repository;
    private readonly Mock<ISubContractorPublicHolidays> _publicHolidays;

    public TestCreateReportSubcontractorUsingTimeSheet()
    {
        _catalogRepository = new Mock<ICatalogRepository>();
        _repository = new Mock<ISubcontractorRepository>();
        _publicHolidays = new Mock<ISubContractorPublicHolidays>();
    }

    [Theory]
    [InlineData(1, 1, "7")]
    [InlineData(1, 2, "7")]
    public async Task Create(int weeks, int workers, string regularWage)
    {
        var sut = new CreateReportSubcontractorUsingTimeSheet(TimeLimits.DefaultTimeLimits,
            Rates.DefaultRates, _catalogRepository.Object, _repository.Object, _publicHolidays.Object);
        var result = await sut.Create(Data(new DateTime(2021, 01, 03), weeks, workers));
        Assert.True(result);
        Assert.Equal(workers, result.Value.Count);
        Assert.True(result.Value.All(c =>
        {
            decimal expected = decimal.Parse(regularWage);
            return c.RegularWage == expected && c.Gross == expected && c.Earnings == expected && c.TotalNet == expected;
        }));
        Assert.All(result.Value, p => Assert.Equal(weeks * 7, p.WageDetails.Count()));
        _repository.Verify(r => r.Create(It.IsAny<ReportSubcontractor>()), Times.Exactly(workers));
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1, 1, 6)]
    public async Task Overtime(int weeks, int regular, int overtime)
    {
        var defaultRates = Rates.DefaultRates;
        defaultRates.OverTime = 1;
        var sut = new CreateReportSubcontractorUsingTimeSheet(TimeLimits.DefaultTimeLimits,
            defaultRates, _catalogRepository.Object, _repository.Object, _publicHolidays.Object);

        var now = new DateTime(2021, 01, 03);
        IEnumerable<TimeSheetApprovedPayrollModel> data = Data(now, weeks: weeks, overtimeStarts: 1);

        var result = await sut.Create(data);
        Assert.True(result);

        var report = result.Value.Single();
        Assert.Equal(regular, report.TotalRegular);
        Assert.Equal(overtime, report.TotalOvertime);
    }

    private readonly Dictionary<int, DateTime[]> _holidays = new Dictionary<int, DateTime[]>
    {
        {2,new []{new  DateTime(2021,01,04)}},
        {3,new []{new  DateTime(2021,01,15)}},
        {4,new DateTime[0]},
        {5,new []{new  DateTime(2021,01,21),new  DateTime(2021,01,22)}}
    };

    [Theory]
    [InlineData(1, 1)]
    public async Task Holidays(int weeks, int expected)
    {
        var now = new DateTime(2021, 01, 03);

        _catalogRepository.Setup(r => r.GetHolidaysInWeek(It.IsAny<DateTime>()))
            .Returns<DateTime>(firstDate => Task.FromResult(_holidays[firstDate.GetWeekOfYearStartSunday()].ToList()));

        _publicHolidays.Setup(ph => ph.GetSubcontractorWorkerHolidays(It.IsAny<IEnumerable<DateTime>>(), It.IsAny<Guid>()))
            .Returns<IEnumerable<DateTime>, Guid>((holidaysInWeek, _) => Task.FromResult<IReadOnlyCollection<ReportSubcontractorPublicHoliday>>(
                holidaysInWeek.Select(h => new ReportSubcontractorPublicHoliday(h, 1)).ToList()));

        IEnumerable<TimeSheetApprovedPayrollModel> data = Data(now, weeks);
        var sut = new CreateReportSubcontractorUsingTimeSheet(TimeLimits.DefaultTimeLimits,
            Rates.DefaultRates, _catalogRepository.Object, _repository.Object, _publicHolidays.Object);
        var result = await sut.Create(data);
        Assert.True(result);
        var report = result.Value.Single();
        Assert.Equal(expected, report.Holidays.Count());
        Assert.Equal(expected, report.PublicHolidayPay);
    }

    private static IEnumerable<TimeSheetApprovedPayrollModel> Data(
        DateTime now,
        int weeks = 1,
        int workers = 1,
        double overtimeStarts = default) =>
        Enumerable.Range(0, workers).SelectMany(w =>
        {
            var workerId = Guid.NewGuid();
            Guid workerProfileId = workerId;
            return Enumerable.Range(0, weeks * 7).Select(i =>
                new TimeSheetApprovedPayrollModel(
                    overtimeStarts.Equals(default) ? CompanyProfile.DefaultOvertimeStarts : TimeSpan.FromHours(overtimeStarts),
                    default,
                    1,
                    default,
                    default,
                    default,
                    workerId,
                    workerProfileId,
                    default,
                    now.AddDays(i).GetWeekOfYearStartSunday(),
                    now.AddDays(i),
                    now.AddDays(i),
                    now.AddDays(i).AddHours(1),
                    default,
                    default,
                    default,
                    default,
                    default,
                    default,
                    default,
                    default,
                    default,
                    default));
        });
}