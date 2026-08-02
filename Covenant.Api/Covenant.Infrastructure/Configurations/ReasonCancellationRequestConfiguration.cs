using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations;

public class ReasonCancellationRequestConfiguration : IEntityTypeConfiguration<ReasonCancellationRequest>
{
    public void Configure(EntityTypeBuilder<ReasonCancellationRequest> builder)
    {
        builder.ToTable("ReasonCancellationRequests");
        builder.HasKey(x => x.Id);
    }
}
