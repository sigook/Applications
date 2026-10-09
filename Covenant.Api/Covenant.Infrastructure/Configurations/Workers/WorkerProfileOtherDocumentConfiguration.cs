using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileOtherDocumentConfiguration : IEntityTypeConfiguration<WorkerProfileOtherDocument>
{
    public void Configure(EntityTypeBuilder<WorkerProfileOtherDocument> builder)
    {
        builder.ToTable("WorkerProfileOtherDocuments");
        builder.HasKey(k => k.Id);
    }
}
