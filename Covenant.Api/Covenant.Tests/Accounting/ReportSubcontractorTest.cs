using Covenant.Common.Entities.Accounting.Subcontractors;
using Xunit;

namespace Covenant.Tests.Accounting;

public class ReportSubcontractorTest
{
    private static ReportSubcontractor Report() =>
        ReportSubcontractor.Create(Guid.NewGuid(), 20m, 900m, new DateTime(2026, 3, 2), new DateTime(2026, 3, 8), new DateTime(2026, 3, 8));

    [Fact]
    public void Create_StartsWithGrossAsEarningsAndNet()
    {
        var workerProfileId = Guid.NewGuid();

        var report = ReportSubcontractor.Create(workerProfileId, 20m, 900m, new DateTime(2026, 3, 2), new DateTime(2026, 3, 8), new DateTime(2026, 3, 8));

        Assert.Equal(workerProfileId, report.WorkerProfileId);
        Assert.Equal(20m, report.RegularWage);
        Assert.Equal(900m, report.Gross);
        Assert.Equal(900m, report.Earnings);
        Assert.Equal(900m, report.TotalNet);
        Assert.Equal(0m, report.PublicHolidayPay);
        Assert.Equal(new DateTime(2026, 3, 8), report.WeekEnding);
    }

    [Fact]
    public void AddWageDetail_AssignsDetailsAndSumsHours()
    {
        var report = Report();

        report.AddWageDetail(
        [
            new ReportSubcontractorWageDetail(20m, 40m, 0m, 0m, 0m, 0m, 0m, 5m),
            new ReportSubcontractorWageDetail(22m, 8m, 0m, 0m, 0m, 0m, 0m, 2m)
        ]);

        Assert.Equal(48m, report.TotalRegular);
        Assert.Equal(7m, report.TotalOvertime);
        Assert.All(report.WageDetails, d => Assert.Equal(report.Id, d.ReportSubcontractorId));
    }

    [Fact]
    public void AddHolidays_AssignsHolidays()
    {
        var report = Report();

        report.AddHolidays([new ReportSubcontractorPublicHoliday(new DateTime(2026, 4, 3), 160m)]);

        var holiday = Assert.Single(report.Holidays);
        Assert.Equal(report.Id, holiday.ReportSubcontractorId);
        Assert.Same(report, holiday.ReportSubcontractor);
    }

    [Fact]
    public void AddOtherDeductionsDetail_SkipsNonPositive_AndTotalsTheRest()
    {
        var report = Report();

        report.AddOtherDeductionsDetail(
        [
            new ReportSubContractorOtherDeduction(2, 15m, "Boots"),
            ReportSubContractorOtherDeduction.CreateDefaultDeduction(10m, "Vest"),
            new ReportSubContractorOtherDeduction(1, 0m, "Free"),
            new ReportSubContractorOtherDeduction(1, -5m, "Negative")
        ]);

        Assert.Equal(40m, report.DeductionOthers);
        Assert.Equal(["Boots", "Vest"], report.OtherDeductionsDetail.Select(d => d.Description));
        Assert.All(report.OtherDeductionsDetail, d => Assert.Equal(report.Id, d.ReportSubcontractorId));
    }

    [Fact]
    public void OtherDeduction_KeepsGivenId_AndRejectsNullReport()
    {
        var id = Guid.NewGuid();

        var deduction = new ReportSubContractorOtherDeduction(1.5, 10m, "Tools", id);

        Assert.Equal(id, deduction.Id);
        Assert.Equal(15m, deduction.Total);
        Assert.Throws<ArgumentNullException>(() => deduction.AssignTo(null));
        Assert.Throws<ArgumentNullException>(() => new ReportSubcontractorWageDetail().AssignTo(null));
        Assert.Throws<ArgumentNullException>(() => new ReportSubcontractorPublicHoliday().AssignTo(null));
    }
}
