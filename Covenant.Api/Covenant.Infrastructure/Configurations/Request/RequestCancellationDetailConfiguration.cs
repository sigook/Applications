using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestCancellationDetailConfiguration : IEntityTypeConfiguration<RequestCancellationDetail>
    {
        public void Configure(EntityTypeBuilder<RequestCancellationDetail> builder)
        {
            builder.ToTable("RequestCancellationDetail");
            builder.HasKey(d => d.RequestId);
        }
    }
} 