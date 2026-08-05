using Covenant.Common.Functionals;
using Covenant.Common.Models.Worker;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfileJobExperience : IWorkerProfileJobExperience
    {
        private const int CompanyMinLength = 2;
        private const int CompanyMaxLength = 50;
        private const int SupervisorMinLength = 2;
        private const int SupervisorMaxLength = 50;
        private const int DutiesMinLength = 2;
        private const int DutiesMaxLength = 5000;

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Company { get; private set; }
        public string Supervisor { get; private set; }
        public string Duties { get; set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool IsCurrentJobPosition { get; private set; }
        public Guid WorkerProfileId { get; set; }
        public WorkerProfile WorkerProfile { get; set; }

        public static Result<WorkerProfileJobExperience> Create(string company, string supervisor, string duties, DateTime startDate, DateTime? endDate, bool isCurrentPosition)
        {
            if (string.IsNullOrEmpty(company)) return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.RequiredMsg(ApiResources.Company));
            int length = company.Length;
            if (length < CompanyMinLength || length > CompanyMaxLength)
                return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.LengthMsg(ApiResources.Company, CompanyMinLength, CompanyMaxLength));
            if (!string.IsNullOrEmpty(supervisor))
            {
                length = supervisor.Length;
                if (length < SupervisorMinLength || length > SupervisorMaxLength)
                    return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.LengthMsg(ApiResources.Supervisor, SupervisorMinLength, SupervisorMaxLength));
            }
            if (string.IsNullOrEmpty(duties)) return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.RequiredMsg(ApiResources.Duties));
            length = duties.Length;
            if (length < DutiesMinLength || length > DutiesMaxLength)
                return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.LengthMsg(ApiResources.Duties, DutiesMinLength, DutiesMaxLength));

            if (startDate == default)
                return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.RequiredMsg(ApiResources.StartDate));
            if (startDate.Date > DateTime.Now.Date)
                return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.LessThanMsg(ApiResources.StartDate, DateTime.Now.AddDays(1).Date.ToShortDateString()));
            if (!isCurrentPosition)
            {
                if (endDate.GetValueOrDefault() == default)
                    return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.RequiredMsg(ApiResources.EndDate));
                if (endDate.GetValueOrDefault().Date < startDate.Date)
                    return Result.Fail<WorkerProfileJobExperience>(ValidationMessages.GreaterThanOrEqualMsg(ApiResources.EndDate, startDate.Date.ToShortDateString()));
            }

            return Result.Ok(new WorkerProfileJobExperience
            {
                Company = company,
                Supervisor = supervisor,
                Duties = duties,
                StartDate = startDate,
                EndDate = endDate,
                IsCurrentJobPosition = isCurrentPosition
            });
        }

        public Result Update(WorkerProfileJobExperience newEntity)
        {
            if (newEntity is null) throw new ArgumentNullException(nameof(newEntity));
            Company = newEntity.Company;
            Supervisor = newEntity.Supervisor;
            Duties = newEntity.Duties;
            StartDate = newEntity.StartDate;
            EndDate = newEntity.EndDate;
            IsCurrentJobPosition = newEntity.IsCurrentJobPosition;
            return Result.Ok();
        }
    }
}