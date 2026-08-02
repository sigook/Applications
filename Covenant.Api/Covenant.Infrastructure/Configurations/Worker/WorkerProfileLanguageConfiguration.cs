using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileLanguageConfiguration : IEntityTypeConfiguration<WorkerProfileLanguage>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileLanguage> builder)
        {
            builder.ToTable("WorkerProfileLanguages");
            builder.HasKey(a => new { a.WorkerProfileId, a.LanguageId });
        }
    }
} 