namespace Covenant.Common.Models.Identity;

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
