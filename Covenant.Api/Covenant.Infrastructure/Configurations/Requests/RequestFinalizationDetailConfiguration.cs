using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class RequestFinalizationDetailConfiguration : IEntityTypeConfiguration<RequestFinalizationDetail>
{
    public void Configure(EntityTypeBuilder<RequestFinalizationDetail> builder)
    {
        builder.ToTable("RequestFinalizationDetails");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Request)
            .WithMany()
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
