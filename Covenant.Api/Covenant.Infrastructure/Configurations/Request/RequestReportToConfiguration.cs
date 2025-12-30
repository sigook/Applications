using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestReportToConfiguration : IEntityTypeConfiguration<RequestReportTo>
    {
        public void Configure(EntityTypeBuilder<RequestReportTo> builder)
        {
            builder.ToTable("RequestReportTo");
            builder.HasKey(k => new { k.RequestId, RequestedById = k.ContactPersonId });
        }
    }
} 