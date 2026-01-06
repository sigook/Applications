using AutoMapper;

namespace Covenant.Api.Shared.WorkerComment.Models
{
    public class AgencyWorkerCommentMapperProfile : Profile
    {
        public AgencyWorkerCommentMapperProfile()
        {
            CreateMap<CreateCommentModel, Covenant.Common.Entities.Worker.WorkerComment>()
                .ForMember(e => e.Worker, exp => exp.Ignore());
        }
    }
}