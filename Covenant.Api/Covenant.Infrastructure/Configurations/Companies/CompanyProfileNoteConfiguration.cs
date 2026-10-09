using Covenant.Common.Entities.Company;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Companies;

public class CompanyProfileNoteConfiguration : IEntityTypeConfiguration<CompanyProfileNote>
{
    public void Configure(EntityTypeBuilder<CompanyProfileNote> builder)
    {
        builder.ToTable("CompanyProfileNotes");
        builder.HasKey(k => new { k.CompanyProfileId, k.NoteId });
    }
}
