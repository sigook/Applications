namespace Covenant.Common.Models
{
    public class IndustryDetailModel : BaseModel<Guid>
    {
        public bool HasChildren { get; set; }
    }
}