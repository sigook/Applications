using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations;

public class CovenantNoteConfiguration : IEntityTypeConfiguration<CovenantNote>
{
    public void Configure(EntityTypeBuilder<CovenantNote> builder)
    {
        builder.ToTable("CovenantNotes");
        builder.HasKey(x => x.Id);
    }
}
