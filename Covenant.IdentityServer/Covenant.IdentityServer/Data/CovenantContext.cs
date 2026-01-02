using Covenant.Common.Entities;
using Covenant.IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.Data
{
    public class CovenantContext : IdentityDbContext<CovenantUser, CovenantRole, Guid>
    {
        public CovenantContext(DbContextOptions<CovenantContext> options) : base(options) { }

        public DbSet<InactiveUser> InactiveUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CovenantUser>().ToTable("User");
            builder.Entity<CovenantRole>().ToTable("Rol");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRole").HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogin").HasKey(ul => ul.UserId);
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken").HasKey(ut => ut.UserId);
            builder.Entity<InactiveUser>().HasKey(iu => iu.InactiveUserId);
        }
    }
}