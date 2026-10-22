namespace Covenant.Common.Models.Request;

public class AgencyRequestApplicantsModel
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
    public int WorkersQuantity { get; set; }
    public int ConfirmedApplicants { get; set; }
    public int TotalApplicants { get; set; }
    public List<AgencyApplicantListModel> Applicants { get; set; } = [];
}
