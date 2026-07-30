using Covenant.Common.Entities.Company;

namespace Covenant.Common.Entities.Worker;

public class WorkerComment
{
    private WorkerComment()
    {
    }

    private WorkerComment(Guid workerProfileId, string comment, decimal rate)
    {
        WorkerProfileId = workerProfileId;
        Comment = comment;
        Rate = ValidateRateValues(rate);
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Comment { get; private set; }
    public decimal Rate { get; private set; }
    public WorkerProfile WorkerProfile { get; private set; }
    public Guid WorkerProfileId { get; private set; }
    public CompanyProfile CompanyProfile { get; private set; }
    public Guid? CompanyProfileId { get; private set; }
    public int NumberId { get; private set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public static WorkerComment CommentPostByAgency(Guid workerProfileId, string comment, decimal rate) =>
        new(workerProfileId, comment, rate) { CompanyProfileId = null };

    public static WorkerComment CommentPostByCompany(Guid workerProfileId, Guid companyProfileId, string comment, decimal rate) =>
        new(workerProfileId, comment, rate) { CompanyProfileId = companyProfileId };

    private static decimal ValidateRateValues(decimal rate)
    {
        if (rate < 0) return 0;
        return rate > 5 ? 5 : rate;
    }
}
