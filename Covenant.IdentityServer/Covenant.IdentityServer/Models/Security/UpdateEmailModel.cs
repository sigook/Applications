using Covenant.IdentityServer.Models;
using System.ComponentModel.DataAnnotations;

namespace Covenant.IdentityServer.Models.Security;

public class UpdateEmailModel : IdModel
{
    public UpdateEmailModel()
    {
    }

    public UpdateEmailModel(Guid id) : base(id)
    {
    }

    [EmailAddress]
    [Required]
    public string NewEmail { get; set; }
}
