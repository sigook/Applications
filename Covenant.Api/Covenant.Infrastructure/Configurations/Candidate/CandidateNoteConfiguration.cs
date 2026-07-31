using Covenant.Common.Entities.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Candidate;

public class CandidateNoteConfiguration : IEntityTypeConfiguration<CandidateNote>
{
    public void Configure(EntityTypeBuilder<CandidateNote> builder)
    {
        builder.ToTable("CandidateNotes");
        builder.HasKey(k => new { k.CandidateId, k.NoteId });
    }
}
