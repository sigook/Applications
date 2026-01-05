using Covenant.Common.Entities.Worker;

namespace Covenant.Common.Entities.Accounting.PayStub
{
    public class PayStub
    {
        private PayStub()
        {
        }

        public PayStub(
            Guid workerProfileId,
            string payStubNumber,
            long payStubNumberId,
            string typeOfWork,
            DateTime dateWorkBegins,
            DateTime dateWorkEnd,
            DateTime paymentDate,
            decimal regularWage,
            decimal grossPayment,
            decimal vacations,
            decimal publicHolidayPay,
            decimal totalEarnings,
            decimal cpp,
            decimal ei,
            decimal federalTax,
            decimal provincialTax,
            decimal totalDeductions,
            decimal totalPaid,
            DateTime createdAt,
            Guid id = default)
        {
            Id = id == default ? Guid.NewGuid() : id;
            WorkerProfileId = workerProfileId;
            PayStubNumber = payStubNumber;
            PayStubNumberId = payStubNumberId;
            TypeOfWork = typeOfWork;
            DateWorkBegins = dateWorkBegins;
            DateWorkEnd = dateWorkEnd;
            PaymentDate = paymentDate;
            RegularWage = regularWage;
            GrossPayment = grossPayment;
            Vacations = vacations;
            PublicHolidayPay = publicHolidayPay;
            TotalEarnings = totalEarnings;
            Cpp = cpp;
            Ei = ei;
            FederalTax = federalTax;
            ProvincialTax = provincialTax;
            TotalDeductions = totalDeductions;
            TotalPaid = totalPaid;
            CreatedAt = createdAt;
        }

        public Guid Id { get; private set; }
        public Guid WorkerProfileId { get; private set; }
        public long NumberId { get; private set; }
        public WorkerProfile WorkerProfile { get; internal set; }
        public string PayStubNumber { get; private set; }
        public long PayStubNumberId { get; private set; }
        public string TypeOfWork { get; private set; }
        public DateTime DateWorkBegins { get; private set; }
        public DateTime DateWorkEnd { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public DateTime WeekEnding { get; set; }
        public decimal RegularWage { get; private set; }
        public decimal GrossPayment { get; private set; }
        public decimal Vacations { get; private set; }
        public decimal PublicHolidayPay { get; private set; }
        public decimal TotalEarnings { get; private set; }
        public decimal Cpp { get; private set; }
        public decimal Ei { get; private set; }
        public decimal FederalTax { get; private set; }
        public decimal ProvincialTax { get; private set; }
        public decimal OtherDeductions { get; private set; }
        public decimal TotalDeductions { get; private set; }
        public decimal TotalPaid { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IEnumerable<PayStubItem> Items { get; private set; } = new List<PayStubItem>();
        public IEnumerable<PayStubWageDetail> WageDetails { get; private set; } = new List<PayStubWageDetail>();
        public IEnumerable<PayStubPublicHoliday> Holidays { get; private set; } = new List<PayStubPublicHoliday>();
        public IEnumerable<PayStubOtherDeduction> OtherDeductionsDetail { get; private set; } = new List<PayStubOtherDeduction>();

        public void AddItems(IEnumerable<PayStubItem> items)
        {
            Items = new List<PayStubItem>(items);
            foreach (var item in Items) item.AssignTo(this);
        }

        public void AddWageDetails(IEnumerable<PayStubWageDetail> wageDetails)
        {
            WageDetails = new List<PayStubWageDetail>(wageDetails);
            foreach (var wageDetail in WageDetails) wageDetail.AssignTo(this);
        }

        public void AddHolidays(IEnumerable<PayStubPublicHoliday> holidays)
        {
            Holidays = new List<PayStubPublicHoliday>(holidays);
            foreach (var holiday in Holidays) holiday.AssignTo(this);
        }

        public void AddOtherDeductionsDetail(IEnumerable<PayStubOtherDeduction> otherDeductions)
        {
            var list = new List<PayStubOtherDeduction>();
            decimal totalOtherDeductions = 0;
            foreach (var deduction in otherDeductions)
            {
                if (deduction.Total <= 0) continue;
                deduction.AssignTo(this);
                totalOtherDeductions += deduction.Total;
                list.Add(deduction);
            }
            OtherDeductionsDetail = new List<PayStubOtherDeduction>(list);
            OtherDeductions = totalOtherDeductions;
        }

        public static string PayrollNumber(long number, DateTime date) => $"PS-{number:0000}-{date:yy}";
    }
}