using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class RequestReportToConfiguration : IEntityTypeConfiguration<RequestReportTo>
{
    public void Configure(EntityTypeBuilder<RequestReportTo> builder)
    {
        builder.ToTable("RequestReportTos");
        builder.HasKey(k => new { k.RequestId, RequestedById = k.ContactPersonId });
        builder.HasOne(x => x.Request)
            .WithMany(r => r.ReportTo)
            .HasForeignKey(x => x.RequestId);
    }
}
