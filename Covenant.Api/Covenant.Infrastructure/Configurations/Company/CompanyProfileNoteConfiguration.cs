using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Company
{
    public class CompanyProfileNoteConfiguration : IEntityTypeConfiguration<CompanyProfileNote>
    {
        public void Configure(EntityTypeBuilder<CompanyProfileNote> builder)
        {
            builder.ToTable("CompanyProfileNote");
            builder.HasKey(k => new { k.CompanyProfileId, k.NoteId });
        }
    }
} 