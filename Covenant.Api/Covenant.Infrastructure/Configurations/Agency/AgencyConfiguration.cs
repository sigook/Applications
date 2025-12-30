using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Agency;

public class AgencyConfiguration : IEntityTypeConfiguration<Common.Entities.Agency.Agency>
{
    public void Configure(EntityTypeBuilder<Common.Entities.Agency.Agency> builder)
    {
        builder.ToTable("Agency");
        builder.Property(a => a.NumberId).ValueGeneratedOnAdd();
        
        // Configure self-referencing relationship for Agency Parent
        builder.HasOne(a => a.AgencyParent)
               .WithMany()
               .HasForeignKey(a => a.AgencyParentId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
    }
}