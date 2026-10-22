namespace Covenant.Common.Entities.Company;

public class CompanyProfileContactPerson
{
    private CompanyProfileContactPerson() { }

    public CompanyProfileContactPerson(Guid companyProfileId) => CompanyProfileId = companyProfileId;

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
    public Guid CompanyProfileId { get; private set; }
    public CompanyProfile CompanyProfile { get; internal set; }

    public void Update(string title, string firstName, string middleName, string lastName, string position, string mobileNumber, string officeNumber, int? officeNumberExt, string email)
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
