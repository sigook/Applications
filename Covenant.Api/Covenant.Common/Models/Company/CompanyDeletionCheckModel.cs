namespace Covenant.Common.Models.Company;

public class CompanyDeletionCheckModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public List<CompanyDeletionBlockerModel> Blockers { get; set; } = [];
    public bool CanDelete => Blockers.Count == 0;
}
