namespace Covenant.Common.Models.Company
{
    public class CompanyProfileJobPositionRateModel
    {
        public Guid? Id { get; set; }
        public Guid? CompanyProfileId { get; set; }
        public string JobPosition { get; set; }
        public decimal Rate { get; set; }
        public decimal WorkerRate { get; set; }
        public decimal? WorkerRateMin { get; set; }
        public decimal? WorkerRateMax { get; set; }
        public double? OvertimeStartsAfter { get; set; }
        public string Description { get; set; }
        public ShiftModel Shift { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string DisplayShift { get; set; }
    }
}
