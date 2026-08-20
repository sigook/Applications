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
        public const int DaysToWaitToResendInvitation = 7;

        private int _workersQuantityWorking;
        private int _workersQuantity = MinimumWorkersQuantity;
        private readonly List<WorkerRequest> _workers = new List<WorkerRequest>();
        private readonly List<RequestRecruiter> _recruiters = new List<RequestRecruiter>();

        private Request()
        {
        }

        public Request(CompanyProfile companyProfile, CompanyProfileJobPositionRate jobPositionRate, int workersQuantity = MinimumWorkersQuantity)
        {
            CompanyProfile = companyProfile;
            JobPositionRate = jobPositionRate;
            CompanyProfileId = companyProfile.Id;
            JobPositionRateId = jobPositionRate.Id;
            WorkersQuantity = workersQuantity;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public int NumberId { get; set; }
        public string JobTitle { get; set; }
        public string BillingTitle { get; set; }
        public string JobCosting { get; set; }
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
        public CompanyProfile CompanyProfile { get; set; }
        public Guid CompanyProfileId { get; set; }
        public bool IsAsap { get; set; }
        public bool UsesRunners { get; set; } = true;
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
        public RequestComission RequestComission { get; set; }
        public IEnumerable<RequestCompanyUser> RequestCompanyUser { get; set; }
        public IEnumerable<RequestComplianceItem> ComplianceItems { get; set; }
        public IEnumerable<RequestNote> Notes { get; set; } = new List<RequestNote>();
        public ICollection<RequestSource> Sources { get; set; } = new List<RequestSource>();
        public IReadOnlyCollection<RequestRecruiter> Recruiters => _recruiters;
        public IReadOnlyCollection<WorkerRequest> Workers => _workers;
        private string TheRequestCanNotBeChanged => $"The request can't be changed because is: {Status}";
        private int CountWorkersWorking => _workers.Count(c => c.WorkerRequestStatus == WorkerRequestStatus.Booked);
        public bool CanBeUpdated => Status != RequestStatus.Cancelled;
        public bool IsAvailableToApply => Status == RequestStatus.Open;
        public RequestStatus Status { get; private set; } = RequestStatus.Open;
        public int WorkersQuantity
        {
            get => _workersQuantity;
            set => _workersQuantity = value < MinimumWorkersQuantity ? MinimumWorkersQuantity : value;
        }
        public int WorkersQuantityWorking
        {
            get => _workersQuantityWorking;
            private set => _workersQuantityWorking = value;
        }

        public event EventHandler OnNewShift;

        public Result<Guid> AddWorker(Guid workerProfileId, DateTime startWorking, string createdBy = null)
        {
            if (CountWorkersWorking >= WorkersQuantity) return Result.Fail<Guid>("The Request is complete");
            if (!IsAvailableToApply) return Result.Fail<Guid>(TheRequestCanNotBeChanged);
            WorkerRequest workerRequest = _workers.SingleOrDefault(s => s.WorkerProfileId == workerProfileId);
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
                        workerRequest = WorkerRequest.AgencyBook(workerProfileId, Id, createdBy);
                        _workers.Add(workerRequest);
                        break;
                    }
            }

            workerRequest.UpdateStartWorking(startWorking);
            WorkersQuantityWorking = CountWorkersWorking;

            // Automatic state transitions
            if (WorkersQuantityWorking >= WorkersQuantity)
                Status = RequestStatus.Filled;

            return Result.Ok(workerRequest.Id);
        }

        public Result RejectWorker(Guid workerProfileId, string detail, string rejectedBy = null)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            WorkerRequest workerRequest = _workers.SingleOrDefault(a => a.WorkerProfileId == workerProfileId);
            if (workerRequest is null) return Result.Fail("Worker isn't in the request");
            if (workerRequest.IsRejected) return Result.Fail("Worker is already rejected");
            workerRequest.Reject(detail, null, rejectedBy);
            WorkersQuantityWorking = CountWorkersWorking;

            // Automatic state transitions
            if (Status == RequestStatus.Filled && WorkersQuantityWorking < WorkersQuantity)
                Status = RequestStatus.Open;

            return Result.Ok();
        }

        public Result Cancel(DateTime now)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);

            // Only requests in Open status can be cancelled
            if (Status != RequestStatus.Open)
                return Result.Fail("Only requests in Open status can be cancelled");

            // Cannot cancel requests with workers assigned
            if (WorkersQuantityWorking > 0)
                return Result.Fail("Cannot cancel requests with workers assigned. Please remove all workers first.");

            foreach (WorkerRequest worker in Workers)
            {
                Result rReject = worker.Reject("Request canceled", now);
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
                case RequestStatus.Open:
                case RequestStatus.Filled:
                    return Result.Ok();
                case RequestStatus.Cancelled:
                    {
                        // Reopen to appropriate state based on capacity
                        Status = WorkersQuantityWorking >= WorkersQuantity
                            ? RequestStatus.Filled
                            : RequestStatus.Open;
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
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            Requirements = requirements;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateDescription(string description)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            Description = description;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateDurationBreak(TimeSpan durationBreak)
        {
            DurationBreak = durationBreak;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateJobTitle(string jobTitle)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            JobTitle = jobTitle;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateBillingTitle(string title)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            BillingTitle = title;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateJobCosting(string jobCosting)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            JobCosting = jobCosting;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateUsesRunners(bool usesRunners)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            UsesRunners = usesRunners;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result UpdateIsAsap(bool? isAsap = null)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
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
            Incentive = newIncentive;
            if (string.IsNullOrEmpty(description))
            {
                IncentiveDescription = null;
                return Result.Ok();
            }
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
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            if (JobLocation is null)
            {
                JobLocation = location;
            }
            JobLocationId = location.Id;
            UpdatedAt = DateTime.Now;
            JobIsOnBranchOffice = jobIsOnBranchOffice;
            return Result.Ok();
        }

        public Result UpdatePunchCardVisibilityStatusInApp(bool visibleInApp)
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            PunchCardOptionEnabled = visibleInApp;
            UpdatedAt = DateTime.Now;
            return Result.Ok();
        }

        public Result IncreaseWorkersQuantityByOne()
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            WorkersQuantity++;
            UpdatedAt = DateTime.Now;

            // If it was filled and we increase capacity, it goes back to Open
            if (Status == RequestStatus.Filled)
                Status = RequestStatus.Open;

            return Result.Ok();
        }

        public Result DecreaseWorkersQuantityByOne()
        {
            if (!CanBeUpdated) return Result.Fail(TheRequestCanNotBeChanged);
            if (WorkersQuantity <= 1 || WorkersQuantity <= WorkersQuantityWorking) return Result.Fail($"The request has to have at least {WorkersQuantity} worker");
            WorkersQuantity--;
            UpdatedAt = DateTime.Now;

            // Update status if reducing capacity causes the request to become filled
            if (WorkersQuantityWorking >= WorkersQuantity && Status == RequestStatus.Open)
                Status = RequestStatus.Filled;

            return Result.Ok();
        }

        public Result AddRecruiter(AgencyPersonnel recruiter, DateTime now, DateTime? workDate = null)
        {
            if (recruiter is null) throw new ArgumentNullException(nameof(recruiter));
            DateTime? day = workDate?.Date;
            if (_recruiters.Count(a => a.WorkDate == day) >= MaximumNumberOfRecruiters) return Result.Fail($"The maximum number of recruiters is {MaximumNumberOfRecruiters}");
            if (_recruiters.Any(a => a.RecruiterId == recruiter.Id && a.WorkDate == day)) return Result.Fail();
            _recruiters.Add(new RequestRecruiter(Id, recruiter.Id, now, day));
            UpdatedAt = now;
            return Result.Ok();
        }

        public Result RemoveRecruiter(Guid id, DateTime now, DateTime? workDate = null)
        {
            DateTime? day = workDate?.Date;
            var entity = _recruiters.FirstOrDefault(c => c.RecruiterId == id && c.WorkDate == day);
            if (entity != null)
            {
                _recruiters.Remove(entity);
            }
            UpdatedAt = now;
            return Result.Ok();
        }

        public Result MoveRecruiterAssignment(Guid fromRecruiterId, DateTime fromWorkDate, Guid toRecruiterId, DateTime toWorkDate, DateTime now)
        {
            DateTime from = fromWorkDate.Date;
            DateTime to = toWorkDate.Date;
            var entity = _recruiters.FirstOrDefault(a => a.RecruiterId == fromRecruiterId && a.WorkDate == from);
            if (entity is null) return Result.Fail("Assignment not found");
            if (fromRecruiterId == toRecruiterId && from == to) return Result.Ok();
            if (_recruiters.Any(a => a.RecruiterId == toRecruiterId && a.WorkDate == to)) return Result.Fail("The recruiter is already assigned to this order on that day");
            entity.MoveTo(toRecruiterId, to);
            UpdatedAt = now;
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
            Guid companyProfileId,
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
            bool isPunchCardOptionEnabled = true)
        {
            return Result.Ok(new Request
            {
                CompanyProfileId = companyProfileId,
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
            Guid companyProfileId,
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
            bool isPunchCardOptionEnabled = true)
        {
            return Result.Ok(new Request
            {
                CompanyProfileId = companyProfileId,
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