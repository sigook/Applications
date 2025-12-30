using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class PayStubHistoryConfiguration : IEntityTypeConfiguration<PayStubHistory>
{
    public void Configure(EntityTypeBuilder<PayStubHistory> builder)
    {
        builder.HasNoKey();
        builder.ToView("PayStubHistory");
    }
}
