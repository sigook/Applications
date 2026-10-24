namespace Covenant.Common.Entities
{
    public class Holiday
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public string CountryCode { get; private set; }

        public Holiday()
        {
        }
        public Holiday(DateTime date)
        {
            Date = date;
        }

        public Holiday(DateTime date, string countryCode) : this(date) => CountryCode = countryCode;
    }
}
