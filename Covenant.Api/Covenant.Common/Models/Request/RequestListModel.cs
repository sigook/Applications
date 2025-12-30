using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request;

public class RequestListModel
{
    public Guid Id { get; set; }
    public string JobTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishAt { get; set; }
    public int NumberId { get; set; }
    public int WorkersQuantity { get; set; }
    public int WorkersQuantityWorking { get; set; }
    public string Location { get; set; }
    public string Entrance { get; set; }
    public string CompanyFullName { get; set; }
    public string AgencyFullName { get; set; }
    public string AgencyLogo { get; set; }
    public string Logo { get; set; }
    public RequestStatus RequestStatus { get; set; }
    public string Status { get; set; }
    public bool IsAsap { get; set; }
    public string WorkerApprovedToWork { get; set; }
    public DateTime StartWorking { get; set; }
    public DateTime FinishWorking { get; set; }
    public string DisplayShift { get; set; }
    public bool IsDirectHiring { get; set; }
    public Guid CompanyId { get; set; }
}