using Covenant.Common.Functionals;
using Covenant.Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace Covenant.Common.Entities.Company
{
    public class CompanyProfileJobPositionRate
    {
        public const decimal MinAgencyRate = 0.01M;
        public const decimal MaxAgencyRate = 1000M;
        public const decimal MinWorkerRate = 0.01M;
        public const decimal MaxWorkerRate = 1000M;

        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? JobPositionId { get; set; }
        public JobPosition JobPosition { get; set; }
        public string OtherJobPosition { get; set; }

        public Guid CompanyProfileId { get; set; }
        public CompanyProfile CompanyProfile { get; set; }

        public Guid? ShiftId { get; private set; }
        public Shift Shift { get; private set; }

        [Range(0.1, 1000)]
        public decimal Rate { get; set; } //AgencyRate
        public decimal WorkerRate { get; set; }
        public decimal? WorkerRateMin { get; private set; }
        public decimal? WorkerRateMax { get; private set; }
        public decimal? AsapRate { get; set; }
        public string Description { get; set; }

        public bool IsDeleted { get; private set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public event EventHandler<Shift> OnShiftUpdated;

        /// <summary>
        /// When the company creates an account the company doesn't have the rates
        /// that's why by default are in zero
        /// </summary>
        /// <param name="companyProfileId"></param>
        /// <param name="jobPositionId"></param>
        /// <param name="otherJobPosition"></param>
        /// <returns></returns>
        public static Result<CompanyProfileJobPositionRate> CompanyCreate(Guid companyProfileId, Guid? jobPositionId, string otherJobPosition)
        {
            return Result.Ok(new CompanyProfileJobPositionRate
            {
                CompanyProfileId = companyProfileId,
                JobPositionId = jobPositionId,
                OtherJobPosition = otherJobPosition,
                Rate = default,
                WorkerRate = default,
                Description = "Created by company",
                CreatedAt = DateTime.Now,
                CreatedBy = default
            });
        }

        public static Result<CompanyProfileJobPositionRate> Create(Guid companyProfileId, Guid jobPositionId,
            decimal agencyRate, decimal workerRate, string description = null, string createdBy = null) =>
            jobPositionId == Guid.Empty
                ? Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.RequiredMsg(ApiResources.JobPosition))
                : PrivateCreate(companyProfileId, agencyRate, workerRate, description, createdBy, jobPositionId: jobPositionId);

        public static Result<CompanyProfileJobPositionRate> Create(Guid companyProfileId, string otherJobPosition,
            decimal agencyRate, decimal workerRate, string description = null, string createdBy = null) =>
            string.IsNullOrEmpty(otherJobPosition)
                ? Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.RequiredMsg(ApiResources.JobPosition))
                : PrivateCreate(companyProfileId, agencyRate, workerRate, description, createdBy, otherJobPosition);

        private static Result<CompanyProfileJobPositionRate> PrivateCreate(Guid companyProfileId, decimal agencyRate,
            decimal workerRate, string description, string createdBy, string otherJobPosition = null, Guid? jobPositionId = null)
        {
            if (agencyRate < MinAgencyRate || agencyRate > MaxAgencyRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.BetweenMsg(ApiResources.Rate, MinAgencyRate, MaxAgencyRate));

            if (workerRate < MinWorkerRate || workerRate > MaxWorkerRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.BetweenMsg(ApiResources.WorkerRate, MinWorkerRate, MaxWorkerRate));

            if (workerRate > agencyRate)
                return Result.Fail<CompanyProfileJobPositionRate>(ValidationMessages.LessThanOrEqualMsg(ApiResources.WorkerRate, agencyRate));

            return Result.Ok(new CompanyProfileJobPositionRate
            {
                CompanyProfileId = companyProfileId,
                JobPositionId = jobPositionId,
                OtherJobPosition = otherJobPosition,
                Rate = agencyRate,
                WorkerRate = workerRate,
                Description = description,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            });
        }

        public void Update(CompanyProfileJobPositionRate value)
        {
            JobPositionId = value.JobPositionId;
            OtherJobPosition = value.OtherJobPosition;
            Rate = value.Rate;
            WorkerRate = value.WorkerRate;
            Description = value.Description;
            UpdatedAt = DateTime.Now;
            UpdatedBy = value.UpdatedBy ?? value.CreatedBy;
            WorkerRateMin = value.WorkerRateMin;
            WorkerRateMax = value.WorkerRateMax;
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
    }
}