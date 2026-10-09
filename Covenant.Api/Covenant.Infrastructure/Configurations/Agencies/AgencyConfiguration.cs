using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Agencies;

public class AgencyConfiguration : IEntityTypeConfiguration<Common.Entities.Agency.Agency>
{
    public void Configure(EntityTypeBuilder<Common.Entities.Agency.Agency> builder)
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