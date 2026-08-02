using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations;

public class CovenantFileConfiguration : IEntityTypeConfiguration<CovenantFile>
{
    public void Configure(EntityTypeBuilder<CovenantFile> builder)
    {
        builder.ToTable("CovenantFiles");
        builder.HasKey(x => x.Id);
    }
}
