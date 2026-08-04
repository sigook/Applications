namespace Covenant.Common.Models.Security;

public class UpdateEmailModel : IdModel
{
    public UpdateEmailModel() : base()
    {
    }

    public UpdateEmailModel(Guid id) : base(id)
    {
    }

    public string NewEmail { get; set; }
}
