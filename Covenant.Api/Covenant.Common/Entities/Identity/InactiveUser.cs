namespace Covenant.Common.Entities.Identity;

public class InactiveUser
{
    public Guid InactiveUserId { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    internal InactiveUser()
    {
    }

    public InactiveUser(Guid userId) => UserId = userId;
}
