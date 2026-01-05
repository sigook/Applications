namespace Covenant.Common.Models.Security
{
    public class DeleteUserModel : IdModel
    {
        public DeleteUserModel()
            : base()
        {
        }

        public DeleteUserModel(Guid id)
            : base(id)
        {
        }

        public Guid ClaimId { get; set; }
    }
}
