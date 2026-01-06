namespace Covenant.Common.Models.Payment.Bambora
{
    public class CardModel
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Last4 { get; set; }
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
        public string CardHolderName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressCity { get; set; }
        public string AddressZip { get; set; }
        public string Currency { get; set; }
    }
}