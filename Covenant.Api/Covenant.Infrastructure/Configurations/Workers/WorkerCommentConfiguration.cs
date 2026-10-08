using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerCommentConfiguration : IEntityTypeConfiguration<WorkerComment>
{
    public void Configure(EntityTypeBuilder<WorkerComment> builder)
    {
        builder.ToTable("WorkerComments");
        builder.HasKey(k => k.Id);
        builder.Property(c => c.NumberId).ValueGeneratedOnAdd();

        builder.HasOne(c => c.WorkerProfile)
            .WithMany()
            .HasForeignKey(c => c.WorkerProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CompanyProfile)
            .WithMany()
            .HasForeignKey(c => c.CompanyProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
