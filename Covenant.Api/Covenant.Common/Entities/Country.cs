namespace Covenant.Common.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }

        public static Country Canada => new Country
        {
            Id = Guid.Parse("ee7b54a0-9427-49d5-9869-c46bae32a018"),
            Code = "CA",
            Value = "Canada"
        };

        public static Country UnitedStates => new Country
        {
            Id = Guid.Parse("59c77d32-1a2d-4947-b661-7c6539e70277"),
            Code = "USA",
            Value = "United States"
        };
    }
}
