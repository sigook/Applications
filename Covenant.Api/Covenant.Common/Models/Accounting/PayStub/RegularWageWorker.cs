using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Models.Accounting.PayStub
{
    /// <summary>
    /// The amount of public holiday pay to which an employee is entitled is all of the regular wages earned
    /// by the employee in the four work weeks before the work week with the public holiday
    /// plus all of the vacation pay payable to the employee with respect to the four work weeks before
    /// the work week with the public holiday, divided by 20.
    /// </summary>
    public class RegularWageWorker
    {
        private const int TwentyDays = 20;

        public decimal RegularWage { get; set; }
        public decimal Vacations { get; set; }
        public bool HolidayWasPaid { get; set; }
        public decimal CustomPublicHolidayValue { get; set; }
        public bool IsEntitledToReceiveHolidayPay { get; set; }
        public string Description { get; private set; }
        public decimal RegularWagePlusVacations => RegularWage + Vacations;

        public decimal AmountToPay
        {
            get
            {
                if (HolidayWasPaid)
                {
                    Description = "The holiday was already paid";
                    return decimal.Zero;
                }

                else if (CustomPublicHolidayValue > decimal.Zero)
                {
                    Description = $"The amount to pay is a custom value ({CustomPublicHolidayValue})";
                    return CustomPublicHolidayValue;
                }

                else if (!IsEntitledToReceiveHolidayPay)
                {
                    Description = "The worker is not entitled to receive the public holiday because he did not work the required days";
                    return decimal.Zero;
                }

                Description = $"The amount was calculated using the regular formula: (Regular wages ({RegularWage}) + Vacations ({Vacations}) received during the last four weeks / 20)";
                return (RegularWagePlusVacations / TwentyDays).DefaultMoneyRound();
            }
        }
    }
}