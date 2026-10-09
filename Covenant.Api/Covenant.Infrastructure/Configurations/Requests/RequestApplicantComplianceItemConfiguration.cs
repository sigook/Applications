using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class RequestApplicantComplianceItemConfiguration : IEntityTypeConfiguration<RequestApplicantComplianceItem>
{
    private const int MaximumLengthCompletedBy = 200;

    public void Configure(EntityTypeBuilder<RequestApplicantComplianceItem> builder)
    {
        builder.ToTable("RequestApplicantComplianceItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompletedBy)
            .HasMaxLength(MaximumLengthCompletedBy);

        builder.HasIndex(x => new { x.RequestApplicantId, x.RequestComplianceItemId })
            .IsUnique();

        builder.HasOne(x => x.RequestApplicant)
            .WithMany(x => x.ComplianceItems)
            .HasForeignKey(x => x.RequestApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RequestComplianceItem)
            .WithMany()
            .HasForeignKey(x => x.RequestComplianceItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
