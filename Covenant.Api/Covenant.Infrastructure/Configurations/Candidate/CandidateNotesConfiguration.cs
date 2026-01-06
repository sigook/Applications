using Covenant.Common.Entities.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Candidate
{
    public class CandidateNotesConfiguration : IEntityTypeConfiguration<CandidateNote>
    {
        public void Configure(EntityTypeBuilder<CandidateNote> builder)
        {
            builder.ToTable("CandidateNote");
            builder.HasKey(k => new { k.CandidateId, k.NoteId });
        }
    }
}
