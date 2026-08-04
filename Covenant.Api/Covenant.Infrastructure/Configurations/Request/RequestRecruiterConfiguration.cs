using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestRecruiterConfiguration : IEntityTypeConfiguration<RequestRecruiter>
    {
        public void Configure(EntityTypeBuilder<RequestRecruiter> builder)
        {
            builder.ToTable("RequestRecruiters");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.WorkDate).HasColumnType("date");
            builder.HasIndex(k => new { k.RequestId, k.RecruiterId, k.WorkDate }).IsUnique();
        }
    }
}