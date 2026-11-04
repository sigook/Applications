using Covenant.Common.Constants;

namespace Covenant.Common.Models.Accounting.PayStubs;

public class CompanyRegularChargesByWorker(Guid workerId, Guid workerProfileId, decimal agencyRate, decimal regular, decimal otherRegular)
{
    private const int TwentyDays = 20;

    public Guid WorkerId { get; } = workerId;
    public Guid WorkerProfileId { get; } = workerProfileId;
    public decimal AgencyRate { get; } = agencyRate;
    public decimal Regular { get; } = regular + otherRegular;
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
    public double HoursToPay => TotalHours / TwentyDays;
    public decimal AmountToPay => new decimal(HoursToPay) * AgencyRate;
}