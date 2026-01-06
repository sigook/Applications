using Covenant.Common.Entities.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Candidate
{
    public class CandidateDocumentsConfiguration : IEntityTypeConfiguration<CandidateDocument>
    {
        public void Configure(EntityTypeBuilder<CandidateDocument> builder)
        {
            builder.ToTable("CandidateDocument");
            builder.HasKey(k => new { k.CandidateId, k.DocumentId });
        }
    }
}
