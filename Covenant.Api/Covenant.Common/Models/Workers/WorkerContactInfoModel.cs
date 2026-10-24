namespace Covenant.Common.Models.Workers
{
    public class WorkerContactInfoModel
    {
        public Guid WorkerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}