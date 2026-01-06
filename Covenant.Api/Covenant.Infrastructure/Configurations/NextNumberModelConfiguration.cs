using Covenant.Common.Models.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations
{
    public class NextNumberModelConfiguration : IEntityTypeConfiguration<NextNumberModel>
    {
        public void Configure(EntityTypeBuilder<NextNumberModel> builder)
        {
            builder.HasNoKey().ToView("view_doesnt_exitst");
        }
    }
} 