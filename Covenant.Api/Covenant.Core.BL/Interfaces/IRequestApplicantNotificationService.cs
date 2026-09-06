using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;

namespace Covenant.Core.BL.Interfaces;

public interface IRequestApplicantNotificationService
{
    Task Notify(Request request, WorkerProfile workerProfile);
    Task Notify(Request request, Candidate candidate);
}
