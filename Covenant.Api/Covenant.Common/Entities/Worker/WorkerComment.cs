namespace Covenant.Common.Entities.Worker
{
    public class WorkerComment
    {
        private WorkerComment() 
        { 
        }

        private WorkerComment(Guid workerId, string comment, decimal rate)
        {
            WorkerId = workerId;
            Comment = comment;
            Rate = ValidateRateValues(rate);
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Comment { get; private set; }
        public decimal Rate { get; private set; }
        public User Worker { get; private set; }
        public Guid WorkerId { get; private set; }
        public Agency.Agency Agency { get; private set; }
        public Guid? AgencyId { get; private set; }
        public User Company { get; private set; }
        public Guid? CompanyId { get; private set; }
        public int NumberId { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public static WorkerComment CommentPostByAgency(Guid workerId, Guid agencyId, string comment, decimal rate) =>
            new WorkerComment(workerId, comment, rate) { AgencyId = agencyId, CompanyId = null };

        public static WorkerComment CommentPostByCompany(Guid workerId, Guid companyId, string comment, decimal rate) =>
            new WorkerComment(workerId, comment, rate) { CompanyId = companyId, AgencyId = null };

        public WorkerComment Update(string comment, decimal rate)
        {
            Comment = comment;
            Rate = ValidateRateValues(rate);
            return this;
        }

        private static decimal ValidateRateValues(decimal rate)
        {
            if (rate < 0) return 0;
            return rate > 5 ? 5 : rate;
        }
    }
}