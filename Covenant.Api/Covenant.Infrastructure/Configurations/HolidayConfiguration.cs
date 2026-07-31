using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations
{
    public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
    {
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
            builder.ToTable("Holidays");
            builder.HasKey(k => k.Id);
            builder.HasIndex(h => h.Date).IsUnique();
        }
    }
} 