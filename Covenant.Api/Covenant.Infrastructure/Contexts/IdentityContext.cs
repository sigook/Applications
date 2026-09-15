using Covenant.Common.Entities;
using Covenant.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Contexts;

public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext<CovenantUser, CovenantRole, Guid>(options)
{
    public const string ConfigurationsNamespace = "Covenant.Infrastructure.Configurations.Identity";

    public DbSet<InactiveUser> InactiveUsers { get; set; }

    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Ignore<IdentityUserPasskey<Guid>>();
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly, type => type.Namespace == ConfigurationsNamespace);
        builder.UseOpenIddict();
    }
}
