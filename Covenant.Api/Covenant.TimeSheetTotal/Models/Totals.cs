using Covenant.Common.Entities.Request;

namespace Covenant.TimeSheetTotal.Models
{
    public class Totals
    {
        public ITimeSheetTotal TimeSheetTotal { get; }
        public TotalDailyPrice TotalDailyPrice { get; }
        public TotalDailyWage TotalDailyWage { get; }
        public Totals(ITimeSheetTotal timeSheetTotal, TotalDailyPrice totalDailyPrice, TotalDailyWage totalDailyWage)
        {
            TimeSheetTotal = timeSheetTotal;
            TotalDailyPrice = totalDailyPrice;
            TotalDailyWage = totalDailyWage;
        }
    }
}