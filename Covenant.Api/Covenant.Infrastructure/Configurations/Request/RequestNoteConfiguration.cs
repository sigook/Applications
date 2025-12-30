using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestNoteConfiguration : IEntityTypeConfiguration<RequestNote>
    {
        public void Configure(EntityTypeBuilder<RequestNote> builder)
        {
            builder.ToTable("RequestNote");
            builder.HasKey(k => new { k.RequestId, k.NoteId });
        }
    }
} 