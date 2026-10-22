using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileOtherDocument
    {
        private WorkerProfileOtherDocument()
        {
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid WorkerProfileId { get; private set; }
        public WorkerProfile WorkerProfile { get; private set; }
        public Guid DocumentId { get; private set; }
        public CovenantFile Document { get; private set; }

        public static Result<WorkerProfileOtherDocument> Create(Guid workerProfileId, CovenantFile document)
        {
            if (document is null) return Result.Fail<WorkerProfileOtherDocument>(ValidationMessages.RequiredMsg(nameof(document)));
            return Result.Ok(new WorkerProfileOtherDocument
            {
                WorkerProfileId = workerProfileId,
                Document = document,
                DocumentId = document.Id
            });
        }
    }
}