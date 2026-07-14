namespace Covenant.IdentityServer.Models;

public class IdModel
{
    public IdModel()
    {
    }

    public IdModel(Guid id) => Id = id;

    public Guid Id { get; set; }
}
