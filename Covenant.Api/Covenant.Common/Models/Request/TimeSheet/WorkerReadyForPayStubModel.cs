namespace Covenant.Common.Models.Request.TimeSheet
{
    public class WorkerReadyForPayStubModel
    {
        public Guid WorkerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string BusinessName { get; set; }
    }
}