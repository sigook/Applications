namespace Covenant.Common.Entities.Agencies
{
    public class AgencyContactInformation
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string Position { get; private set; }
        public string MobileNumber { get; private set; }
        public string OfficeNumber { get; private set; }
        public int? OfficeNumberExt { get; private set; }
        public string Email { get; private set; }

        internal AgencyContactInformation()
        {
        }

        public AgencyContactInformation(string title, string firstName, string middleName, string lastName, string position, string mobileNumber, string officeNumber, int? officeNumberExt, string email)
        {
            Title = title;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Position = position;
            MobileNumber = mobileNumber;
            OfficeNumber = officeNumber;
            OfficeNumberExt = officeNumberExt;
            Email = email;
        }
    }
}
