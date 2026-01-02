using IdentityServer4.Models;

namespace Covenant.IdentityServer.Models
{
    public class CustomSecret : Secret
    {
        public int? Id { get; set; }
    }
}
