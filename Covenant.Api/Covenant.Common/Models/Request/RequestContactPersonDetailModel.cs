namespace Covenant.Common.Models.Request
{
    public class RequestContactPersonDetailModel : RequestContactPersonModel
    {
        public string Position { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }
        public int? OfficeNumberExt { get; set; }
    }
}