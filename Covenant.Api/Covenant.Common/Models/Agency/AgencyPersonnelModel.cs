namespace Covenant.Common.Models.Agency
{
    public class AgencyPersonnelModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid AgencyId { get; set; }
    }
}