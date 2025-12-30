using AutoMapper;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;

namespace Covenant.Api.Common
{
    public class CommonMapperProfile : Profile
    {
        public CommonMapperProfile()
        {
            CreateMap<Country, CountryModel>().ReverseMap();
            CreateMap<Province, ProvinceModel>().ReverseMap();
            CreateMap<City, CityModel>();

            CreateMap<WsibGroup, BaseModel<Guid>>()
                .ForMember(c => c.Value, exp => exp.MapFrom(e => $"{e.Value} {e.RateGroup} "))
                .ReverseMap();
            CreateMap<Availability, BaseModel<Guid>>().ReverseMap();
            CreateMap<AvailabilityTime, BaseModel<Guid>>().ReverseMap();
            CreateMap<Day, BaseModel<Guid>>().ReverseMap();
            CreateMap<Gender, BaseModel<Guid>>().ReverseMap();
            CreateMap<IdentificationType, BaseModel<Guid>>().ReverseMap();
            CreateMap<Language, BaseModel<Guid>>();
            CreateMap<Lift, BaseModel<Guid>>().ReverseMap();

            CreateMap<Industry, BaseModel<Guid>>();
            CreateMap<Industry, IndustryDetailModel>();

            CreateMap<BaseModel<Guid>, Industry>();

            CreateMap<BaseModel<Guid>, Industry>()
                .ForMember(e => e.JobPositions, exp => exp.Ignore())
                .ForMember(e => e.Value, exp => exp.Ignore());

            CreateMap<JobPosition, JobPositionDetailModel>();
            CreateMap<JobPositionDetailModel, JobPosition>()
                .ForMember(e => e.IndustryId, exp => exp.Ignore())
                .ForMember(e => e.Value, exp => exp.Ignore());

            CreateMap<CovenantFileModel, CovenantFile>()
                .ConvertUsing((m, e) => m?.ToCovenantFile(e));

            CreateMap<CovenantFile, CovenantFileModel>().ConvertUsing<CovenantFileToFileDetailModelConverter>();

            CreateMap<LocationModel, Location>()
                .ConvertUsing((m, e) =>
                {
                    Result<Location> rLocation = Location.Create(m.City.Id, m.Address, m.PostalCode);
                    if (!rLocation) return e;
                    if (e is null) e = rLocation.Value;
                    else e.Update(rLocation.Value);
                    return e;
                });
        }
    }
}
