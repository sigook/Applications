namespace Covenant.Common.Entities.Company
{
    public class CompanyProfileHoliday
    {
        private decimal _statPaidCompany;

        private CompanyProfileHoliday() 
        { 
        }

        public CompanyProfileHoliday(Guid companyProfileId, Guid holidayId, decimal statPaidCompany, Guid id = default)
        {
            CompanyProfileId = companyProfileId;
            HolidayId = holidayId;
            StatPaidCompany = statPaidCompany;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; private set; }
        public CompanyProfile CompanyProfile { get; set; }
        public Guid CompanyProfileId { get; private set; }
        public Holiday Holiday { get; set; }
        public Guid HolidayId { get; private set; }

        public decimal StatPaidCompany
        {
            get => _statPaidCompany;
            private set => _statPaidCompany = value < 0 ? 0 : value;
        }

        public void UpdateStat(decimal statPaidCompany)
        {
            StatPaidCompany = statPaidCompany;
        }
    }
}