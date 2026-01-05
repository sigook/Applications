using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileCertificateConfiguration : IEntityTypeConfiguration<WorkerProfileCertificate>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileCertificate> builder)
        {
            builder.ToTable("WorkerProfileCertificate");
            builder.HasKey(k => k.Id);
        }
    }
} 