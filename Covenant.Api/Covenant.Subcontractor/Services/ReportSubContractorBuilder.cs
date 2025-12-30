using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Subcontractor.Services
{
    public class ReportSubContractorBuilder :
    IWorkerProfileHolder,
    IDateWorkBeginHolder,
    IDateWorkEndsHolder,
    IWageDetailsHolder,
    IPublicHolidaysToPayHolder,
    IOtherDeductionsHolder,
    IReportSubContractorBuilder
    {
        private Guid _workerProfileId;
        private DateTime _dateWorkBegins;
        private DateTime _dateWorkEnd;
        private IReadOnlyCollection<ReportSubcontractorWageDetail> _wageDetails = new List<ReportSubcontractorWageDetail>();
        private IReadOnlyCollection<ReportSubcontractorPublicHoliday> _holidays = new List<ReportSubcontractorPublicHoliday>();
        private IEnumerable<ReportSubContractorOtherDeduction> _otherDeductions = new List<ReportSubContractorOtherDeduction>();

        private ReportSubContractorBuilder() { }
        public static IWorkerProfileHolder Report() => new ReportSubContractorBuilder();

        public IDateWorkBeginHolder WithWorkerProfileId(Guid workerProfileId)
        {
            _workerProfileId = workerProfileId;
            return this;
        }

        public IDateWorkEndsHolder WithWorkBeginning(DateTime workBegins)
        {
            _dateWorkBegins = workBegins;
            return this;
        }

        public IWageDetailsHolder WithWorkEnding(DateTime workEnd)
        {
            _dateWorkEnd = workEnd;
            return this;
        }

        public IPublicHolidaysToPayHolder WithWageDetails(IEnumerable<ReportSubcontractorWageDetail> wageDetails)
        {
            _wageDetails = new List<ReportSubcontractorWageDetail>(wageDetails);
            return this;
        }

        public IOtherDeductionsHolder WithPublicHolidaysToPay(IReadOnlyCollection<ReportSubcontractorPublicHoliday> holidays)
        {
            if (holidays is null) return this;
            _holidays = new List<ReportSubcontractorPublicHoliday>(holidays);
            return this;
        }

        public IReportSubContractorBuilder WithOtherDeductions(ReportSubContractorOtherDeduction otherDeduction)
        {
            _otherDeductions = new[] { otherDeduction };
            return this;
        }

        public IReportSubContractorBuilder WithOtherDeductions(IEnumerable<ReportSubContractorOtherDeduction> otherDeductions)
        {
            _otherDeductions = new List<ReportSubContractorOtherDeduction>(otherDeductions);
            return this;
        }

        public IReportSubContractorBuilder WithNoMoreDeductions() => this;

        public ReportSubcontractor Build()
        {
            decimal regularWage = _wageDetails.Sum(d => d.Regular);
            decimal gross = _wageDetails.Sum(d => d.Gross);
            decimal publicHolidayPay = _holidays.Sum(r => r.Amount).DefaultMoneyRound();
            decimal earnings = gross + publicHolidayPay;
            decimal net = decimal.Subtract(earnings, _otherDeductions.Sum(d => d.Total));
            var reportSubcontractor = new ReportSubcontractor
            {
                WorkerProfileId = _workerProfileId,
                RegularWage = regularWage,
                Gross = gross,
                PublicHolidayPay = publicHolidayPay,
                Earnings = earnings,
                TotalNet = net,
                DateWorkBegins = _dateWorkBegins,
                DateWorkEnd = _dateWorkEnd,
                WeekEnding = _dateWorkBegins.GetWeekEndingCurrentWeek()
            };
            reportSubcontractor.AddWageDetail(_wageDetails);
            reportSubcontractor.AddHolidays(_holidays);
            reportSubcontractor.AddOtherDeductionsDetail(_otherDeductions);
            return reportSubcontractor;
        }
    }

    public interface IWorkerProfileHolder
    {
        IDateWorkBeginHolder WithWorkerProfileId(Guid workerProfileId);
    }

    public interface IDateWorkBeginHolder
    {
        IDateWorkEndsHolder WithWorkBeginning(DateTime workBegins);
    }

    public interface IDateWorkEndsHolder
    {
        IWageDetailsHolder WithWorkEnding(DateTime workEnd);
    }

    public interface IWageDetailsHolder
    {
        IPublicHolidaysToPayHolder WithWageDetails(IEnumerable<ReportSubcontractorWageDetail> wageDetails);
    }

    public interface IPublicHolidaysToPayHolder
    {
        IOtherDeductionsHolder WithPublicHolidaysToPay(IReadOnlyCollection<ReportSubcontractorPublicHoliday> holidays);
    }

    public interface IOtherDeductionsHolder
    {
        IReportSubContractorBuilder WithOtherDeductions(ReportSubContractorOtherDeduction otherDeduction);
        IReportSubContractorBuilder WithOtherDeductions(IEnumerable<ReportSubContractorOtherDeduction> otherDeductions);
        IReportSubContractorBuilder WithNoMoreDeductions();
    }

    public interface IReportSubContractorBuilder
    {
        ReportSubcontractor Build();
    }
}