using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestRequestedByConfiguration : IEntityTypeConfiguration<RequestRequestedBy>
    {
        public void Configure(EntityTypeBuilder<RequestRequestedBy> builder)
        {
            builder.ToTable("RequestRequestedBys");
            builder.HasKey(k => new { k.RequestId, RequestedById = k.ContactPersonId });
        }
    }
} 