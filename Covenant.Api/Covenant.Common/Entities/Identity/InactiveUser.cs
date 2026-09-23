namespace Covenant.Common.Entities.Identity;

public class InactiveUser
{
    public Guid InactiveUserId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
