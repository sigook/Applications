namespace Covenant.Common.Configuration
{
    public class Rates
    {
        public Rates()
        {
        }

        public Rates(
            decimal nightShift,
            decimal holiday,
            decimal overTime,
            decimal vacations,
            decimal bonus,
            decimal canadaPensionPlan,
            decimal employmentInsurance,
            decimal employerInsurance,
            decimal taxes,
            decimal hst)
        {
            NightShift = nightShift;
            Holiday = holiday;
            OverTime = overTime;
            Vacations = vacations;
            Bonus = bonus;
            CanadaPensionPlan = canadaPensionPlan;
            EmploymentInsurance = employmentInsurance;
            EmployerInsurance = employerInsurance;
            Taxes = taxes;
            Hst = hst;
        }

        public decimal NightShift { get; set; }
        public decimal Holiday { get; set; }
        public decimal OverTime { get; set; }
        public decimal Vacations { get; set; }
        public decimal Bonus { get; set; }
        public decimal CanadaPensionPlan { get; set; }
        public decimal EmploymentInsurance { get; set; }
        public decimal EmployerInsurance { get; set; }
        public decimal Taxes { get; set; }
        public decimal Hst { get; set; }

        public static readonly Rates DefaultRates = new Rates(1.5m, 1.5m, 1.5m, 0.04m, 1.4m, 0.051m, 0.0162m, 2.212m, 0.1m, 0.13m);
    }
}