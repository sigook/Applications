namespace Covenant.Common.Models.Identity;

public class CreatePasswordModel
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string Password { get; set; }
}
