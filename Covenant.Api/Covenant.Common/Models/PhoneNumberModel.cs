namespace Covenant.Common.Models
{
    public class PhoneNumberModel
    {
        public PhoneNumberModel()
        {
        }
        public PhoneNumberModel(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
    }
}