using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileLanguageConfiguration : IEntityTypeConfiguration<WorkerProfileLanguage>
{
    public void Configure(EntityTypeBuilder<WorkerProfileLanguage> builder)
    {
        builder.ToTable("WorkerProfileLanguages");
        builder.HasKey(a => new { a.WorkerProfileId, a.LanguageId });
    }
}
