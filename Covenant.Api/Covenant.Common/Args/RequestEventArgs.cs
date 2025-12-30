namespace Covenant.Common.Args
{
    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs(Guid requestId, Guid agencyId)
        {
            RequestId = requestId;
            AgencyId = agencyId;
        }
        public RequestEventArgs(Guid requestId, Guid agencyId, string jobTitle, string currency, decimal? workerRate, decimal? workerSalary)
            : this(requestId, agencyId)
        {
            JobTitle = jobTitle;
            Currency = currency;
            WorkerRate = workerRate;
            WorkerSalary = workerSalary;
        }

        public Guid RequestId { get; }
        public Guid AgencyId { get; }
        public string JobTitle { get; set; }
        public string Currency { get; set; }
        public decimal? WorkerRate { get; set; }
        public decimal? WorkerSalary { get; set; }
    }
}