namespace SigookFunctions.Models
{
    public class TokenExpiryTime
    {
        public TokenExpiryTime(string token, DateTime expiryTime)
        {
            Token = token;
            ExpiryTime = expiryTime;
        }

        public string Token { get; }
        public DateTime ExpiryTime { get; }
    }
}
