using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
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
}
