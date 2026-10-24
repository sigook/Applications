using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Workers
{
    public class WorkerProfileNote
    {
        private WorkerProfileNote()
        {
        }

        public Guid Id { get; private set; }
        public string Note { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public WorkerProfile WorkerProfile { get; private set; }
        public Guid WorkerProfileId { get; private set; }

        public static Result<WorkerProfileNote> Create(Guid workerProfileId, string note, string createdBy, DateTime? createdAt = null)
        {
            if (string.IsNullOrWhiteSpace(note))
                return Result.Fail<WorkerProfileNote>(ValidationMessages.RequiredMsg(nameof(note)));
            if (note.Length > 5000)
                return Result.Fail<WorkerProfileNote>(ValidationMessages.LessThanOrEqualMsg(nameof(note), 5000));
            return Result.Ok(new WorkerProfileNote
            {
                Id = Guid.NewGuid(),
                CreatedAt = createdAt ?? DateTime.Now,
                WorkerProfileId = workerProfileId,
                Note = note,
                CreatedBy = createdBy
            });
        }
    }
}