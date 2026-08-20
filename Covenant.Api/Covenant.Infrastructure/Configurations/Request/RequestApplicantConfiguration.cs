using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covenant.Infrastructure.Configurations.Request;

public class RequestApplicantConfiguration : IEntityTypeConfiguration<RequestApplicant>
{
    private const int MaximumLengthStatus = 20;

    public void Configure(EntityTypeBuilder<RequestApplicant> builder)
    {
        builder.ToTable("RequestApplicants");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion(new EnumToStringConverter<RequestApplicantStatus>())
            .HasMaxLength(MaximumLengthStatus)
            .HasDefaultValue(RequestApplicantStatus.Pending);

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
