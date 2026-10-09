using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class RequestComissionsConfiguration : IEntityTypeConfiguration<RequestComission>
{
    public void Configure(EntityTypeBuilder<RequestComission> builder)
    {
        builder.ToTable("RequestComissions");
        builder.HasKey(rc => rc.RequestId);
        builder.HasOne(rc => rc.Request)
            .WithOne(r => r.RequestComission)
            .HasForeignKey<RequestComission>(rc => rc.RequestId);
    }
}
