using Covenant.Common.Entities.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Candidate;

public class CandidateDocumentConfiguration : IEntityTypeConfiguration<CandidateDocument>
{
    public void Configure(EntityTypeBuilder<CandidateDocument> builder)
    {
        builder.ToTable("CandidateDocuments");
        builder.HasKey(k => new { k.CandidateId, k.DocumentId });
    }
}
