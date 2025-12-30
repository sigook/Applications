namespace Covenant.Common.Models.WebSite;

public class CandidateViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public IList<string> Skills { get; set; }
    public string Status { get; set; }
    public Guid CountryId { get; set; }
    public string Address { get; set; }
    public string FileName { get; set; }
    public bool HasVehicle { get; set; }
    public Guid? RequestId { get; set; }

    public override string ToString()
    {
        return $"{FullName}|{Email}|{Phone}|{CountryId}|{string.Join(", ", Skills)}";
    }
}
