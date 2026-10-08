using Covenant.Common.Entities.Candidate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Candidates;

public class CandidatePhoneConfiguration : IEntityTypeConfiguration<CandidatePhone>
{
    public void Configure(EntityTypeBuilder<CandidatePhone> builder)
    {
        builder.ToTable("CandidatePhones");
        builder.HasKey(x => x.Id);
    }
}
