using Covenant.Common.Entities.Requests;

namespace Covenant.Common.Entities.Companies;

public class CompanyUser
{
    private CompanyUser()
    {
    }

    public CompanyUser(Guid companyProfileId, User user)
    {
        CompanyProfileId = companyProfileId;
        User = user ?? throw new ArgumentNullException(nameof(user));
        UserId = user.Id;
        Id = user.Id;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Lastname { get; private set; }
    public string Position { get; private set; }
    public string MobileNumber { get; private set; }
    public Guid CompanyProfileId { get; private set; }
    public CompanyProfile CompanyProfile { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public string CreatedBy { get; private set; }
    public IEnumerable<RequestCompanyUser> RequestCompanyUser { get; private set; }

    public void UpdateDetails(string name, string lastname, string position, string mobileNumber)
    {
        Name = name;
        Lastname = lastname;
        Position = position;
        MobileNumber = mobileNumber;
    }
}
