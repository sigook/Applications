using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request;

public class RequestApplicantConfiguration : IEntityTypeConfiguration<RequestApplicant>
{
    public void Configure(EntityTypeBuilder<RequestApplicant> builder)
    {
        builder.ToTable("RequestApplicants");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Request)
            .WithMany()
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkerProfile)
            .WithMany()
            .HasForeignKey(x => x.WorkerProfileId)
            .IsRequired(false);

        builder.HasOne(x => x.Candidate)
            .WithMany(x => x.RequestApplicants)
            .HasForeignKey(x => x.CandidateId)
            .IsRequired(false);
    }
}
