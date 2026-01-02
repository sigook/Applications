using System;

namespace Covenant.IdentityServer.Entities
{
    public class InactiveUser
    {
        public Guid InactiveUserId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
