namespace Covenant.Common.Models.Company
{
    public class CompanyUserModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Position { get; set; }
        public string MobileNumber { get; set; }
    }
}