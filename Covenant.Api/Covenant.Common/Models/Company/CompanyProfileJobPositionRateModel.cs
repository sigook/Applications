namespace Covenant.Common.Models.Company
{
    public class CompanyProfileJobPositionRateModel
    {
        public Guid? Id { get; set; }
        public JobPositionDetailModel JobPosition { get; set; }
        public decimal Rate { get; set; }
        public decimal? AsapRate { get; set; }
        public string OtherJobPosition { get; set; }
        public decimal WorkerRate { get; set; }
        public decimal? WorkerRateMin { get; set; }
        public decimal? WorkerRateMax { get; set; }
        public string Description { get; set; }
        public ShiftModel Shift { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Value { get; set; }
        public string DisplayShift { get; set; }
    }
}