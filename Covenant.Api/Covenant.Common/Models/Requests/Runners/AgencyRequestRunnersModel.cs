namespace Covenant.Common.Models.Requests.Runners;

public class AgencyRequestRunnersModel
{
    public Guid RequestId { get; set; }
    public int NumberId { get; set; }
    public string CompanyFullName { get; set; }
    public string JobTitle { get; set; }
    public string City { get; set; }
    public string ProvinceName { get; set; }
    public string DisplayShift { get; set; }
    public DateTime? StartAt { get; set; }
    public bool IsAsap { get; set; }
    public bool IsDirectHiring { get; set; }
    public bool UsesRunners { get; set; }
    public int WorkersQuantity { get; set; }
    public int HiredRunners { get; set; }
    public int TotalRunners { get; set; }
    public List<AgencyRunnerListModel> Runners { get; set; } = [];
}
