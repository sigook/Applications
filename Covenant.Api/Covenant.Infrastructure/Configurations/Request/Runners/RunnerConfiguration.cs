using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covenant.Infrastructure.Configurations.Request.Runners;

public class RunnerConfiguration : IEntityTypeConfiguration<Runner>
{
    public void Configure(EntityTypeBuilder<Runner> builder)
    {
        builder.Property(e => e.NumberId).ValueGeneratedOnAdd();
        builder.Property(e => e.Type).HasConversion(new EnumToStringConverter<RunnerType>());
        builder.Property(e => e.Status).HasConversion(new EnumToStringConverter<RunnerStatus>());

        builder.HasOne(r => r.Request)
            .WithMany()
            .HasForeignKey(r => r.RequestId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.WorkerProfile)
            .WithMany()
            .HasForeignKey(r => r.WorkerProfileId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.Candidate)
            .WithMany()
            .HasForeignKey(r => r.CandidateId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(m => m.StatusHistory)
            .WithOne(o => o.Runner)
            .HasForeignKey(f => f.RunnerId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(m => m.Interviews)
            .WithOne(o => o.Runner)
            .HasForeignKey(f => f.RunnerId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(r => r.RequestId);
    }
}
