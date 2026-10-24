using Covenant.Common.Entities.Requests;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class RequestCancellationDetailConfiguration : IEntityTypeConfiguration<RequestCancellationDetail>
{
    public void Configure(EntityTypeBuilder<RequestCancellationDetail> builder)
    {
        builder.ToTable("RequestCancellationDetails");
        builder.HasKey(d => d.RequestId);
    }
}
