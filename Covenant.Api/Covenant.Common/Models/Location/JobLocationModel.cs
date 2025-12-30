namespace Covenant.Common.Models.Location
{
    public class JobLocationModel
    {
        public Guid LocationId { get; set; }
        public string Address { get; set; }
        public IdModel City { get; set; }
        public string PostalCode { get; set; }
        public string Entrance { get; set; }
        public string MainIntersection { get; set; }
    }
}