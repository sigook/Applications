namespace Covenant.PayStubs.Models
{
    public class CreatePayStubModel
    {
        public long PayStubNumber { get; set; }
        public Guid WorkerProfileId { get; set; }
        public string TypeOfWork { get; set; }
        public DateTime WorkBegins { get; set; }
        public DateTime WorkEnd { get; set; }
        public decimal OtherDeductions { get; set; }
        public string OtherDeductionsDescription { get; set; }
        public double RegularHours { get; set; }
        public decimal UnitPriceRegularHours { get; set; }
        public double OvertimeHours { get; set; }
        public decimal UnitPriceOvertimeHours { get; set; }
        public double MissingHours { get; set; }
        public decimal UnitPriceMissingHours { get; set; }
        public double MissingOvertimeHours { get; set; }
        public decimal UnitPriceMissingOvertimeHours { get; set; }
        public double HolidayPremiumPayHours { get; set; }
        public decimal UnitPriceHolidayPremiumPayHours { get; set; }
        public double Other { get; set; }
        public decimal UnitPriceOther { get; set; }
        public string BonusOthersDescription { get; set; }
        public double Other2 { get; set; }
        public decimal UnitPriceOther2 { get; set; }
        public string BonusOthersDescription2 { get; set; }
        public double Other3 { get; set; }
        public decimal UnitPriceOther3 { get; set; }
        public string BonusOthersDescription3 { get; set; }

        public bool PayVacations { get; set; }

        public IEnumerable<CreatePayStubPublicHolidayModel> Holidays { get; set; } = new List<CreatePayStubPublicHolidayModel>();
    }
}