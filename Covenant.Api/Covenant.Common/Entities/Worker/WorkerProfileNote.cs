using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileNote
    {
        private WorkerProfileNote()
        {
        }

        public Guid Id { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public WorkerProfile WorkerProfile { get; set; }
        public Guid WorkerProfileId { get; set; }

        public static Result<WorkerProfileNote> Create(Guid workerProfileId, string note, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(note))
                return Result.Fail<WorkerProfileNote>(ValidationMessages.RequiredMsg(nameof(note)));
            if (note.Length > 5000)
                return Result.Fail<WorkerProfileNote>(ValidationMessages.LessThanOrEqualMsg(nameof(note), 5000));
            return Result.Ok(new WorkerProfileNote
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                WorkerProfileId = workerProfileId,
                Note = note,
                CreatedBy = createdBy
            });
        }
    }
}