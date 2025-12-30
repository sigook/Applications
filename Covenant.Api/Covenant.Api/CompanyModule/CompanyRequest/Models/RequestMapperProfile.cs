using AutoMapper;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;

namespace Covenant.Api.CompanyModule.CompanyRequest.Models
{
    public class RequestMapperProfile : Profile
    {
        public RequestMapperProfile()
        {
            CreateMap<CompanyProfileJobPositionRate, JobPositionDetailModel>()
                .ForMember(m => m.Id, exp => exp.MapFrom(e => e.Id))
                .ForMember(m => m.Value, exp => exp.MapFrom(e => e.JobPosition != null ? e.JobPosition.Value : e.OtherJobPosition));

            CreateMap<RequestUpdateRequirementsModel, Request>();
        }
    }
}