using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.Agencies;

public class WorkerRequestHistoryModel
{
    public Guid Id { get; set; }
    public int NumberId { get; set; }
    public string JobTitle { get; set; }
    public string CompanyFullName { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string ProvinceName { get; set; }
    public string PostalCode { get; set; }
    public string Entrance { get; set; }
    public DateTime CreatedAt { get; set; }
    public RequestStatus RequestStatus { get; set; }
    public WorkerRequestStatus WorkerRequestStatus { get; set; }
    public bool IsAsap { get; set; }
    public DateTime? StartWorking { get; set; }
    public DateTime? FinishWorking { get; set; }

    public string Location => $"{Address} {City} {ProvinceName} {PostalCode}";
}
