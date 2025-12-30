using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Request
{
    public class Request
    {
        private const int MaximumNumberOfRecruiters = 10;
        private const int MinimumWorkersQuantity = 1;
        private const int MaximumLengthJobTitle = 500;
        private const int MaximumIncentive = 720;
        public const int MaximumLengthIncentiveDescription = 5000;
        public const int DaysToWaitToResendInvitation = 7;

        private int _workersQuantityWorking;
        private int _workersQuantity = MinimumWorkersQuantity;
        private readonly List<WorkerRequest> _workers = new List<WorkerRequest>();
        private RequestStatus _status = RequestStatus.Requested;
        private readonly List<RequestRecruiter> _recruiters = new List<RequestRecruiter>();

        private static readonly TimeSpan MinimumDurationBreak = TimeSpan.Zero;
        private static readonly TimeSpan MaximumDurationBreak = TimeSpan.FromHours(1);

        private Request()
        {
        }

        public Request(User company, Agency.Agency agency, CompanyProfileJobPositionRate jobPositionRate, int workersQuantity = MinimumWorkersQuantity)
        {
            Company = company;
            Agency = agency;
            JobPositionRate = jobPositionRate;
            CompanyId = company.Id;
            AgencyId = agency.Id;
            JobPositionRateId = jobPositionRate.Id;
            WorkersQuantity = workersQuantity;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public int NumberId { get; set; }
        public string JobTitle { get; set; }
        public string BillingTitle { get; set; }
        public bool IsOpen { get; set; } = true;
        public Guid? JobPositionRateId { get; set; }
        public CompanyProfileJobPositionRate JobPositionRate { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string InternalRequirements { get; set; }
        public string Responsibilities { get; set; }
        public TimeSpan DurationBreak { get; set; }
        public bool BreakIsPaid { get; set; }
        public bool JobIsOnBranchOffice { get; set; }
        public Guid JobLocationId { get; set; }
        public Location JobLocation { get; set; }
        public decimal? Incentive { get; set; }
        public string IncentiveDescription { get; set; }
        public decimal? AgencyRate { get; set; }
        public decimal? WorkerRate { get; set; }
        public decimal? WorkerSalary { get; set; }
        public bool PunchCardOptionEnabled { get; set; }
        public bool HolidayIsPaid { get; set; } = true;
        public User Company { get; set; }
        public Guid CompanyId { get; set; }
        public Agency.Agency Agency { get; set; }
        public Guid AgencyId { get; set; }
        public bool IsAsap { get; set; }
        public DurationTerm DurationTerm { get; set; } = DurationTerm.LongTerm;
        public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
        public Shift Shift { get; set; }
        public Guid? ShiftId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? InvitationSentItAt { get; set; }
        public string CreatedBy { get; set; }
        public string DisplayRecruiters { get; set; }
        public RequestComission RequestComission { get; set; }
        public IEnumerable<RequestCompanyUser> RequestCompanyUser { get; set; }
        public IEnumerable<RequestNote> Notes { get; set; } = new List<RequestNote>();
        public IReadOnlyCollection<RequestRecruiter> Recruiters => _recruiters;
        public IReadOnlyCollection<WorkerRequest> Workers => _workers;
        private string TheOrderCanNotBeChanged => $"The order can't be changed because is: {Status}";
        private int CountWorkersWorking => _workers.Count(c => c.WorkerRequestStatus == WorkerRequestStatus.Booked);
        public bool CanBeUpdated => Status != RequestStatus.Cancelled;
        public bool IsAvailableToApply => Status == RequestStatus.Requested || Status == RequestStatus.InProcess;
        public RequestStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
                UpdateIsOpen();
            }
        }
        public int WorkersQuantity
        {
            get => _workersQuantity;
            set
            {
                if (value < MinimumWorkersQuantity) _workersQuantity = MinimumWorkersQuantity;
                else _workersQuantity = value;
                UpdateIsOpen();
            }
        }
        public int WorkersQuantityWorking
        {
            get => _workersQuantityWorking;
            private set
            {
                _workersQuantityWorking = value;
                UpdateIsOpen();
            }
        }

        public event EventHandler OnNewShift;

        private void UpdateIsOpen()
        {
            switch (Status)
            {
                case RequestStatus.Requested:
                case RequestStatus.InProcess:
                    IsOpen = WorkersQuantityWorking < WorkersQuantity;
                    break;
                case RequestStatus.Cancelled:
                    IsOpen = false;
                    break;
                default:
                    IsOpen = false;
                    break;
            }
        }

        public Result<Guid> AddWorker(Guid workerId, DateTime startWorking, string createdBy = null)
        {
            if (!IsAvailableToApply) return Result.Fail<Guid>(TheOrderCanNotBeChanged);
            if (CountWorkersWorking >= WorkersQuantity) return Result.Fail<Guid>("The Order is complete");
            WorkerRequest workerRequest = _workers.SingleOrDefault(s => s.WorkerId == workerId);
            switch (workerRequest?.WorkerRequestStatus)
            {
                case WorkerRequestStatus.Booked: return Result.Ok(workerRequest.Id);
                case WorkerRequestStatus.Rejected:
                    {
                        workerRequest.Book();
                        break;
                    }
                case null:
                    {
                        workerRequest = WorkerRequest.AgencyBook(workerId, Id, createdBy);
                        _workers.Add(workerRequest);
                        break;
                    }
            }

            workerRequest.UpdateStartWorking(startWorking);
            WorkersQuantityWorking = CountWorkersWorking;
            return Result.Ok(workerRequest.Id);
        }

        public Result RejectWorker(Guid workerId, string detail, string rejectedBy = null)
        {
            if (!IsAvailableToApply) return Result.Fail(TheOrderCanNotBeChanged);
            WorkerRequest workerRequest = _workers.SingleOrDefault(a => a.WorkerId == workerId);
            if (workerRequest is null) return Result.Fail("Worker isn't in the order");
            if (workerRequest.IsRejected) return Result.Fail("Worker is already rejected");
            workerRequest.Reject(detail, null, rejectedBy);
            WorkersQuantityWorking = CountWorkersWorking;
            return Result.Ok();
        }

        public Result PutInProcess()
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            Status = RequestStatus.InProcess;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result Cancel(DateTime now)
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            foreach (WorkerRequest worker in Workers)
            {
                Result rReject = worker.Reject("Order canceled", now);
                if (!rReject) return rReject;
            }
            Status = RequestStatus.Cancelled;
            FinishAt = now;
            UpdatedAt = now;
            return Result.Ok();
        }

        public Result Open(DateTime now)
        {
            switch (Status)
            {
                case RequestStatus.Requested:
                case RequestStatus.InProcess: return Result.Ok();
                case RequestStatus.Cancelled:
                    {
                        Status = RequestStatus.InProcess;
                        FinishAt = null;
                        UpdatedAt = now;
                        DurationTerm = DurationTerm.LongTerm;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Result.Ok();
        }

        public Result UpdateRequirements(string requirements)
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            Requirements = requirements;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateDescription(string description)
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            Description = description;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateDurationBreak(TimeSpan durationBreak)
        {
            if (durationBreak < MinimumDurationBreak) durationBreak = MinimumDurationBreak;
            if (durationBreak > MaximumDurationBreak)
                return Result.Fail(ValidationMessages.GreaterThanOrEqualMsg(ApiResources.DurationBreak, MaximumDurationBreak));
            DurationBreak = durationBreak;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateJobTitle(string jobTitle)
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            if (string.IsNullOrEmpty(jobTitle)) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.JobTitle));
            if (jobTitle.Length > MaximumLengthJobTitle) return Result.Fail(ValidationMessages.LessThanOrEqualMsg(ApiResources.JobTitle, MaximumLengthJobTitle));
            JobTitle = jobTitle;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateBillingTitle(string title)
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            if (title?.Length > MaximumLengthJobTitle) return Result.Fail(ValidationMessages.LessThanOrEqualMsg(nameof(BillingTitle), MaximumLengthJobTitle));
            BillingTitle = title;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateIsAsap(bool? isAsap = null)
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            if (isAsap.HasValue) IsAsap = isAsap.Value;
            else IsAsap = !IsAsap;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateIncentive(decimal? newIncentive, string description)
        {
            if (newIncentive.GetValueOrDefault() <= decimal.Zero)
            {
                Incentive = null;
                return Result.Ok();
            }
            if (newIncentive > MaximumIncentive) return Result.Fail(ValidationMessages.LessThanOrEqualMsg(ApiResources.Incentive, MaximumIncentive));
            Incentive = newIncentive;
            if (string.IsNullOrEmpty(description))
            {
                IncentiveDescription = null;
                return Result.Ok();
            }
            if (description.Length > MaximumLengthIncentiveDescription)
                return Result.Fail(ValidationMessages.LessThanOrEqualMsg(ApiResources.IncentiveDescription, MaximumLengthIncentiveDescription));
            IncentiveDescription = description;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateShift(Shift newShift)
        {
            if (newShift is null) return Result.Ok();
            if (ShiftId is null || Shift is null)
            {
                OnNewShift?.Invoke(this, new EventArgs());
                Shift = newShift;
                ShiftId = newShift.Id;
            }
            else Shift.Update(newShift);
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateJobLocation(Location location, bool jobIsOnBranchOffice)
        {
            if (location is null) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.Location));
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            if (JobLocation is null)
            {
                JobLocation = location;
            }
            JobLocationId = location.Id;
            UpdatedAt = DateTime.Now;
            JobIsOnBranchOffice = jobIsOnBranchOffice;
            return Result.Ok();
        }

        public Result UpdatePunchCardVisibilityStatusInApp(bool? visibleInApp = null)
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            if (visibleInApp.HasValue) PunchCardOptionEnabled = visibleInApp.Value;
            else PunchCardOptionEnabled = !PunchCardOptionEnabled;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result IncreaseWorkersQuantityByOne()
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            WorkersQuantity++;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result DecreaseWorkersQuantityByOne()
        {
            if (!CanBeUpdated) return Result.Fail(TheOrderCanNotBeChanged);
            if (WorkersQuantity <= 1 || WorkersQuantity <= WorkersQuantityWorking) return Result.Fail($"The order has to have at least {WorkersQuantity} worker");
            WorkersQuantity--;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        private void UpdateDisplayRecruiters(string last = null)
        {
            ICollection<string> names = _recruiters.Where(w => !string.IsNullOrEmpty(w.Recruiter?.Name))
                .Select(c => c.Recruiter.Name).ToList();
            if (!string.IsNullOrEmpty(last)) names.Add(last);
            DisplayRecruiters = string.Join("|", names);
        }

        public Result AddRecruiter(AgencyPersonnel recruiter)
        {
            if (_recruiters.Count >= MaximumNumberOfRecruiters) return Result.Fail($"The maximum number of recruiters is {MaximumNumberOfRecruiters}");
            if (recruiter is null) throw new ArgumentNullException(nameof(recruiter));
            if (_recruiters.Any(a => a.RecruiterId == recruiter.Id)) return Result.Fail();
            _recruiters.Add(new RequestRecruiter(Id, recruiter.Id));
            UpdateDisplayRecruiters(recruiter.Name);
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result RemoveRecruiter(Guid id)
        {
            var entity = _recruiters.FirstOrDefault(c => c.RecruiterId == id);
            if (entity != null)
            {
                _recruiters.Remove(entity);
            }
            UpdateDisplayRecruiters();
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result CanInvitationBeSendIt(DateTime now)
        {
            if (InvitationSentItAt is null) return Result.Ok();
            TimeSpan timeSinceItWasSendIt = now.Date - InvitationSentItAt.GetValueOrDefault().Date;
            if (timeSinceItWasSendIt >= TimeSpan.FromDays(DaysToWaitToResendInvitation)) return Result.Ok();
            return Result.Fail($"The invitation must be sent it only once every {DaysToWaitToResendInvitation} days.");
        }

        public static Result<Request> AgencyCreateRequest(
            Guid agencyId,
            Guid companyId,
            Location location,
            DateTime startAt,
            Guid? jobPositionRateId = null,
            DateTime? finishAt = null,
            decimal? workerRate = default,
            decimal? agencyRate = default,
            string jobTitle = null,
            int workersQuantity = MinimumWorkersQuantity,
            string description = null,
            string requirements = null,
            bool jobIsOnBranchOffice = true,
            DurationTerm durationTerm = DurationTerm.LongTerm,
            EmploymentType employmentType = EmploymentType.FullTime,
            TimeSpan durationBreak = default,
            bool breakIsPaid = false,
            decimal? incentive = null,
            string incentiveDescription = null,
            string createdBy = null,
            bool isPunchCardOptionEnabled = false)
        {
            return Result.Ok(new Request
            {
                AgencyId = agencyId,
                CompanyId = companyId,
                JobTitle = jobTitle,
                WorkersQuantity = workersQuantity,
                JobPositionRateId = jobPositionRateId,
                Description = description,
                JobIsOnBranchOffice = jobIsOnBranchOffice,
                JobLocation = location,
                DurationBreak = durationBreak,
                BreakIsPaid = breakIsPaid,
                Incentive = incentive,
                IncentiveDescription = incentiveDescription,
                Requirements = requirements,
                StartAt = startAt,
                FinishAt = finishAt,
                DurationTerm = durationTerm,
                EmploymentType = employmentType,
                WorkerRate = workerRate,
                AgencyRate = agencyRate,
                CreatedBy = createdBy,
                PunchCardOptionEnabled = isPunchCardOptionEnabled
            });
        }

        public static Result<Request> AgencyCreateRequest(
            Guid agencyId,
            Guid companyId,
            Guid locationId,
            DateTime startAt,
            Guid? jobPositionRateId = null,
            DateTime? finishAt = null,
            decimal? workerRate = default,
            decimal? agencyRate = default,
            string jobTitle = null,
            int workersQuantity = MinimumWorkersQuantity,
            string description = null,
            string requirements = null,
            bool jobIsOnBranchOffice = true,
            DurationTerm durationTerm = DurationTerm.LongTerm,
            EmploymentType employmentType = EmploymentType.FullTime,
            TimeSpan durationBreak = default,
            bool breakIsPaid = false,
            decimal? incentive = null,
            string incentiveDescription = null,
            string createdBy = null,
            bool isPunchCardOptionEnabled = false)
        {
            return Result.Ok(new Request
            {
                AgencyId = agencyId,
                CompanyId = companyId,
                JobTitle = jobTitle,
                WorkersQuantity = workersQuantity,
                JobPositionRateId = jobPositionRateId,
                Description = description,
                JobIsOnBranchOffice = jobIsOnBranchOffice,
                JobLocationId = locationId,
                DurationBreak = durationBreak,
                BreakIsPaid = breakIsPaid,
                Incentive = incentive,
                IncentiveDescription = incentiveDescription,
                Requirements = requirements,
                StartAt = startAt,
                FinishAt = finishAt,
                DurationTerm = durationTerm,
                EmploymentType = employmentType,
                WorkerRate = workerRate,
                AgencyRate = agencyRate,
                CreatedBy = createdBy,
                PunchCardOptionEnabled = isPunchCardOptionEnabled
            });
        }
    }
}