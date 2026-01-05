namespace Covenant.Common.Entities.Agency
{
    public class AgencyContactInformation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }
        public int? OfficeNumberExt { get; set; }
        public string Email { get; set; }
    }
}