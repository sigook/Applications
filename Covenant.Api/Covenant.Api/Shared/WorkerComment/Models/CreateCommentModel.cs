namespace Covenant.Api.Shared.WorkerComment.Models
{
    public class CreateCommentModel
    {
        public string Comment { get; set; }
        private int _rate;
        public int Rate
        {
            get => _rate;
            set => _rate =  value < 0  ? 0 : value;
        }
    }
}