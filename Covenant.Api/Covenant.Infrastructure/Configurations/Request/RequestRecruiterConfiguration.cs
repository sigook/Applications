using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestRecruiterConfiguration : IEntityTypeConfiguration<RequestRecruiter>
    {
        public void Configure(EntityTypeBuilder<RequestRecruiter> builder)
        {
            builder.ToTable("RequestRecruiter");
            builder.HasKey(k => new { k.RequestId, k.RecruiterId });
        }
    }
} 