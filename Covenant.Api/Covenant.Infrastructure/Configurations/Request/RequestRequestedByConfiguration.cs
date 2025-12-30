using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestRequestedByConfiguration : IEntityTypeConfiguration<RequestRequestedBy>
    {
        public void Configure(EntityTypeBuilder<RequestRequestedBy> builder)
        {
            builder.ToTable("RequestRequestedBy");
            builder.HasKey(k => new { k.RequestId, RequestedById = k.ContactPersonId });
        }
    }
} 