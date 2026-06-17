using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class WorkerDispatchConfiguration : IEntityTypeConfiguration<WorkerDispatch>
    {
        public void Configure(EntityTypeBuilder<WorkerDispatch> builder)
        {
            builder.ToTable("WorkerDispatch");
            builder.HasKey(k => k.Id);
            builder.HasOne(d => d.RequestRecruiter)
                .WithMany(r => r.Dispatches)
                .HasForeignKey(d => d.RequestRecruiterId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(d => d.WorkerProfile)
                .WithMany()
                .HasForeignKey(d => d.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(k => new { k.RequestRecruiterId, k.WorkerProfileId }).IsUnique();
        }
    }
}