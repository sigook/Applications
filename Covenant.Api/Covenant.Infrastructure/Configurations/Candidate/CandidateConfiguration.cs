using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Candidate
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Common.Entities.Candidate.Candidate>
    {
        public void Configure(EntityTypeBuilder<Common.Entities.Candidate.Candidate> builder)
        {
            builder.ToTable("Candidates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NumberId).ValueGeneratedOnAdd();

            builder.Property(x => x.Email).IsRequired(false);

            builder
                .HasMany(x => x.PhoneNumbers)
                .WithOne(x => x.Candidate)
                .HasForeignKey(x => x.CandidateId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade).Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
