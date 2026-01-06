namespace Covenant.Common.Models.Worker
{
    public class WorkerCommentModel
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public decimal Rate { get; set; }
        public string Logo { get; set; }
        public int NumberId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}