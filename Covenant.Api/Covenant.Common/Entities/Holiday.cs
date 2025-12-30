namespace Covenant.Common.Entities
{
    public class Holiday
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }

        public Holiday()
        {
        }
        public Holiday(DateTime date)
        {
            Date = date;
        }
    }
}