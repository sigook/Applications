namespace Covenant.Common.Models.Company;

public class CompanyProfileLocationListModel
{
    public Guid CompanyProfileId { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string ProvinceCode { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public string Entrance { get; set; }
    public string MainIntersection { get; set; }
    public bool IsBilling { get; set; }

    public string FormattedAddress => $"{Address} {City} {ProvinceCode} {PostalCode}";
}
