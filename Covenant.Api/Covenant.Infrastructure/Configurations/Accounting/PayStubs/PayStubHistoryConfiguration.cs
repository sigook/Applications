using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.PayStubs;

public class PayStubHistoryConfiguration : IEntityTypeConfiguration<PayStubHistory>
{
    public void Configure(EntityTypeBuilder<PayStubHistory> builder)
    {
        builder.HasNoKey();
        builder.ToView("PayStubHistory");
    }
}
