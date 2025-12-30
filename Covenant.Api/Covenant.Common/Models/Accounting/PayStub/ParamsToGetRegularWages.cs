using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Models.Accounting.PayStub;

public readonly struct ParamsToGetRegularWages
{
    public ParamsToGetRegularWages(Guid profileId, DateTime holiday)
    {
        ProfileId = profileId;
        Holiday = holiday;
        End = holiday.GetEnd();
        Start = End.GetStart();
        RangeOfDaysWorkerMustWorkToReceiveHolidayPay = holiday.GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay();
    }

    public Guid ProfileId { get; }
    public DateTime Start { get; }
    public DateTime End { get; }
    public DateTime Holiday { get; }
    public IEnumerable<DateTime> RangeOfDaysWorkerMustWorkToReceiveHolidayPay { get; }
}