using Covenant.Common.Entities.Candidates;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;

namespace Covenant.Core.BL.Interfaces.Requests;

public interface IRequestApplicantNotificationService
{
    Task Notify(Request request, WorkerProfile workerProfile);
    Task Notify(Request request, Candidate candidate);
}
