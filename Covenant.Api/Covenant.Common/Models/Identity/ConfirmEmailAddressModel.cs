using System.ComponentModel.DataAnnotations;

namespace Covenant.Common.Models.Identity;

public class ConfirmEmailAddressModel
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Id { get; set; }
}
