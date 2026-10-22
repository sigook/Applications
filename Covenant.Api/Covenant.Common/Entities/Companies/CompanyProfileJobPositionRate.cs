using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Company
{
    public class CompanyProfileJobPositionRate
    {
        public const decimal MinAgencyRate = 0.01M;
        public const decimal MaxAgencyRate = 1000M;
        public const decimal MinWorkerRate = 0.01M;
        public const decimal MaxWorkerRate = 1000M;

        public Guid Id { get; private set; } = Guid.NewGuid();

        public string JobPosition { get; internal set; }

        public Guid CompanyProfileId { get; internal set; }
        public CompanyProfile CompanyProfile { get; internal set; }

        public Guid? ShiftId { get; private set; }
        public Shift Shift { get; private set; }

        public decimal Rate { get; internal set; } //AgencyRate
        public decimal WorkerRate { get; internal set; }
        public decimal? WorkerRateMin { get; private set; }
        public decimal? WorkerRateMax { get; private set; }
        public TimeSpan? OvertimeStartsAfter { get; private set; }
        public string Description { get; private set; }

        public bool IsDeleted { get; private set; }
        public string CreatedBy { get; private set; }
        public string UpdatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public event EventHandler<Shift> OnShiftUpdated;

        /// <summary>
        /// When the company creates an account the company doesn't have the rates
        /// that's why by default are in zero
        /// </summary>
        public static Result<CompanyProfileJobPositionRate> CompanyCreate(Guid companyProfileId, string jobPosition)
        {
            if (string.IsNullOrWhiteSpace(jobPosition))
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.RequiredMsg(ApiResources.JobPosition));

            return Result.Ok(new CompanyProfileJobPositionRate
            {
                CompanyProfileId = companyProfileId,
                JobPosition = jobPosition,
                Rate = default,
                WorkerRate = default,
                Description = "Created by company",
                CreatedAt = DateTime.Now,
                CreatedBy = default
            });
        }

        public static Result<CompanyProfileJobPositionRate> Create(Guid companyProfileId, string jobPosition,
            decimal agencyRate, decimal workerRate, string description = null, string createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(jobPosition))
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.RequiredMsg(ApiResources.JobPosition));

            if (agencyRate < MinAgencyRate || agencyRate > MaxAgencyRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.BetweenMsg(ApiResources.Rate, MinAgencyRate, MaxAgencyRate));

            if (workerRate < MinWorkerRate || workerRate > MaxWorkerRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.BetweenMsg(ApiResources.WorkerRate, MinWorkerRate, MaxWorkerRate));

            if (workerRate > agencyRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.LessThanOrEqualMsg(ApiResources.WorkerRate, agencyRate));

            return Result.Ok(new CompanyProfileJobPositionRate
            {
                CompanyProfileId = companyProfileId,
                JobPosition = jobPosition,
                Rate = agencyRate,
                WorkerRate = workerRate,
                Description = description,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            });
        }

        public void Update(CompanyProfileJobPositionRate value)
        {
            JobPosition = value.JobPosition;
            Rate = value.Rate;
            WorkerRate = value.WorkerRate;
            Description = value.Description;
            UpdatedAt = DateTime.Now;
            UpdatedBy = value.UpdatedBy ?? value.CreatedBy;
            WorkerRateMin = value.WorkerRateMin;
            WorkerRateMax = value.WorkerRateMax;
            OvertimeStartsAfter = value.OvertimeStartsAfter;
            UpdateShift(value.Shift);
        }

        public Result UpdateWorkerRateMin(decimal? min)
        {
            if (min is null)
            {
                WorkerRateMin = null;
                return Result.Ok();
            }
            if (min < MinWorkerRate || min > MaxWorkerRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.BetweenMsg(ApiResources.WorkerRate, MinWorkerRate, MaxWorkerRate));
            WorkerRateMin = min;
            return Result.Ok();
        }

        public Result UpdateWorkerRateMax(decimal? max)
        {
            if (max is null)
            {
                WorkerRateMax = null;
                return Result.Ok();
            }
            if (max < MinWorkerRate || max > MaxWorkerRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.BetweenMsg(ApiResources.WorkerRate, MinWorkerRate, MaxWorkerRate));
            WorkerRateMax = max;
            return Result.Ok();
        }

        public void AddShift(Shift shift)
        {
            Shift = shift ?? throw new ArgumentNullException(nameof(shift));
            ShiftId = shift.Id;
        }

        public void UpdateShift(Shift newShift)
        {
            if (newShift is null) return;
            if (ShiftId is null || Shift is null)
            {
                OnShiftUpdated?.Invoke(this, newShift);
                Shift = newShift;
                ShiftId = Shift.Id;
            }
            else
            {
                Shift.Update(newShift);
            }
        }

        public void Delete(string deletedBy)
        {
            IsDeleted = true;
            UpdatedBy = deletedBy;
            UpdatedAt = DateTime.Now;
        }

        public void UpdateOvertimeStartsAfter(TimeSpan? overtimeStartsAfter) => OvertimeStartsAfter = overtimeStartsAfter;
    }
}
