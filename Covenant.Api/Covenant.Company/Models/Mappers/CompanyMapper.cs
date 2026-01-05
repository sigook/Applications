using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Company;

namespace Covenant.Company.Models.Mappers
{
    public static class CompanyMapper
    {
        public static Result<CompanyProfileJobPositionRate> ToJobPosition(this CompanyProfileJobPositionRateModel model, Guid profileId, string createdBy)
        {
            Result<CompanyProfileJobPositionRate> result;
            if (model.JobPosition != null && model.JobPosition.Id != Guid.Empty)
            {
                result = CompanyProfileJobPositionRate.Create(profileId, model.JobPosition.Id,
                    model.Rate, model.WorkerRate, model.Description, createdBy);
            }
            else
            {
                result = CompanyProfileJobPositionRate.Create(profileId, model.OtherJobPosition,
                    model.Rate, model.WorkerRate, model.Description, createdBy);
            }
            if (!result) return result;

            Result rMin = result.Value.UpdateWorkerRateMin(model.WorkerRateMin);
            if (!rMin) return Result.Fail<CompanyProfileJobPositionRate>(rMin.Errors);

            Result rMax = result.Value.UpdateWorkerRateMax(model.WorkerRateMax);
            if (!rMax) return Result.Fail<CompanyProfileJobPositionRate>(rMax.Errors);

            if (model.Shift != null) result.Value.AddShift(model.Shift.ToShift());
            return result;
        }
    }
}