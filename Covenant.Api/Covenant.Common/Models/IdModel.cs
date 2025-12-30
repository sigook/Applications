namespace Covenant.Common.Models
{
    public class IdModel : BaseModel<Guid>
    {
        public IdModel()
        {
        }

        public IdModel(Guid id) => Id = id;

        public static implicit operator Guid(IdModel model) => model.Id;
        public static implicit operator IdModel(Guid id) => new IdModel(id);
    }
}