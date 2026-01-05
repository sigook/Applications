using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker;

internal class WorkerProfileTaxCategoryConfiguration : IEntityTypeConfiguration<WorkerProfileTaxCategory>
{
    public void Configure(EntityTypeBuilder<WorkerProfileTaxCategory> builder)
    {
        builder.ToTable("WorkerProfileTaxCategories");
        builder.HasKey(wptc => wptc.WorkerProfileId);

        builder.HasOne(wptc => wptc.WorkerProfile)
            .WithOne(wptc => wptc.WorkerProfileTaxCategory)
            .HasForeignKey<WorkerProfileTaxCategory>(wptc => wptc.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
