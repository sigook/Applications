using Covenant.Common.Entities.Candidate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Candidate
{
    public class CandidatePhoneConfiguration : IEntityTypeConfiguration<CandidatePhone>
    {
        public void Configure(EntityTypeBuilder<CandidatePhone> builder)
        {
            builder.ToTable("CandidatePhones");
            builder.HasKey(x => x.Id);
        }
    }
}
