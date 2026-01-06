using Covenant.Common.Constants;

namespace Covenant.Common.Models.Accounting.PayStub
{
    public class CompanyRegularChargesByWorker
    {
        public CompanyRegularChargesByWorker(Guid workerId, Guid workerProfileId, decimal agencyRate, decimal regular, decimal otherRegular)
        {
            WorkerId = workerId;
            WorkerProfileId = workerProfileId;
            AgencyRate = agencyRate;
            Regular = regular + otherRegular;
        }

        public Guid WorkerId { get; }
        public Guid WorkerProfileId { get; }
        public decimal AgencyRate { get; }
        public decimal Regular { get; }
        public double TotalHours
        {
            get
            {
                if (AgencyRate == decimal.Zero) return default;
                var partialTotalHours = (double)(Regular / AgencyRate);
                return partialTotalHours > CovenantConstants.PublicHolidays.LimitForWorkerWorkedHours
                    ? CovenantConstants.PublicHolidays.LimitForWorkerWorkedHours : partialTotalHours;
            }
        }
        public double HoursToPay => TotalHours / CovenantConstants.PublicHolidays.TwentyDays;
        public decimal AmountToPay => new decimal(HoursToPay) * AgencyRate;
    }
}