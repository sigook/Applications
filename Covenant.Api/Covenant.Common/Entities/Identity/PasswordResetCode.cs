namespace Covenant.Common.Entities.Identity;

public class PasswordResetCode
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string CodeHash { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public int Attempts { get; private set; }
    public bool Consumed { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    internal PasswordResetCode()
    {
    }

    public PasswordResetCode(Guid userId, string codeHash, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        UserId = userId;
        CodeHash = codeHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public void Consume() => Consumed = true;

    public void RegisterFailedAttempt() => Attempts++;
}
