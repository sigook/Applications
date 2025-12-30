using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Utils.Extensions;
using Covenant.PayStubs.Models;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.PayStubs.Utils
{
    public static class PayStubMappers
    {
        public static PayStubWageDetail ToWageDetail(this TotalDailyWage source, ITimeSheetTotal timeSheetTotal) =>
            new PayStubWageDetail(
                source.WorkerRate,
                source.Regular,
                source.OtherRegular,
                source.Missing,
                source.MissingOvertime,
                source.NightShift,
                source.Holiday,
                source.Overtime,
                timeSheetTotal.Id)
            {
                TimeSheetTotal = new TimeSheetTotalPayroll(timeSheetTotal)
            };

        internal static IEnumerable<IGrouping<double, TimeSheetApprovedPayrollModel>> GroupTimeSheetByWeek(this IEnumerable<TimeSheetApprovedPayrollModel> timeSheet) =>
            TimeSheetApprovedPayrollModel.GroupTimeSheetByWeek(timeSheet);

        internal static decimal GetRegularWage(this IEnumerable<PayStubItem> items) =>
            items.Where(i => i.Type == PayStubItemType.Regular).Sum(i => i.Total).DefaultMoneyRound();

        internal static Result<List<PayStubItem>> ToPayStubItems(this CreatePayStubModel model)
        {
            var list = new List<PayStubItem>();
            if (model.RegularHours > 0)
            {
                var regular = PayStubItem.CreateRegular(model.RegularHours, model.UnitPriceRegularHours);
                if (!regular) return Result.Fail<List<PayStubItem>>(regular.Errors);
                list.Add(regular.Value);
            }
            if (model.OvertimeHours > 0)
            {
                var overtime = PayStubItem.CreateOvertime(model.OvertimeHours, model.UnitPriceOvertimeHours);
                if (!overtime) return Result.Fail<List<PayStubItem>>(overtime.Errors);
                list.Add(overtime.Value);
            }

            if (model.MissingHours > 0)
            {
                var missing = PayStubItem.CreateMissing(model.MissingHours, model.UnitPriceMissingHours);
                if (!missing) return Result.Fail<List<PayStubItem>>(missing.Errors);
                list.Add(missing.Value);
            }

            if (model.MissingOvertimeHours > 0)
            {
                var missingOvertime = PayStubItem.CreateMissingOvertime(model.MissingOvertimeHours, model.UnitPriceMissingOvertimeHours);
                if (!missingOvertime) return Result.Fail<List<PayStubItem>>(missingOvertime.Errors);
                list.Add(missingOvertime.Value);
            }

            if (model.HolidayPremiumPayHours > 0)
            {
                var holiday = PayStubItem.CreateHolidayPremiumPay(model.HolidayPremiumPayHours, model.UnitPriceHolidayPremiumPayHours);
                if (!holiday) return Result.Fail<List<PayStubItem>>(holiday.Errors);
                list.Add(holiday.Value);
            }

            if (model.Other > 0)
            {
                var other = PayStubItem.CreateBonusOthersItem(model.Other, model.UnitPriceOther, model.BonusOthersDescription);
                if (!other) return Result.Fail<List<PayStubItem>>(other.Errors);
                list.Add(other.Value);
            }

            if (model.Other2 > 0)
            {
                var other2 = PayStubItem.CreateBonusOthersItem(model.Other2, model.UnitPriceOther2, model.BonusOthersDescription2);
                if (!other2) return Result.Fail<List<PayStubItem>>(other2.Errors);
                list.Add(other2.Value);
            }

            if (model.Other3 > 0)
            {
                var other3 = PayStubItem.CreateBonusOthersItem(model.Other3, model.UnitPriceOther3, model.BonusOthersDescription3);
                if (!other3) return Result.Fail<List<PayStubItem>>(other3.Errors);
                list.Add(other3.Value);
            }

            return Result.Ok(list);
        }
    }
}