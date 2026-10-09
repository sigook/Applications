using Covenant.Common.Entities.Candidate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Candidates;

public class CandidateNoteConfiguration : IEntityTypeConfiguration<CandidateNote>
{
    public void Configure(EntityTypeBuilder<CandidateNote> builder)
    {
        builder.ToTable("CandidateNotes");
        builder.HasKey(k => new { k.CandidateId, k.NoteId });
    }
}
