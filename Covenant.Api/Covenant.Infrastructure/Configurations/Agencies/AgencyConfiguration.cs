using Covenant.Common.Entities.Agencies;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Agencies;

public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
{
    public void Configure(EntityTypeBuilder<Agency> builder)
    {
        builder.ToTable("Agencies");
        builder.Property(a => a.NumberId).ValueGeneratedOnAdd();
        
        // Configure self-referencing relationship for Agency Parent
        builder.HasOne(a => a.AgencyParent)
               .WithMany()
               .HasForeignKey(a => a.AgencyParentId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
    }
}