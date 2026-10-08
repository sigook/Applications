namespace Covenant.Common.Models.Sales.Dashboard;

public class RecentClientModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime LastInteractionAt { get; set; }
}
