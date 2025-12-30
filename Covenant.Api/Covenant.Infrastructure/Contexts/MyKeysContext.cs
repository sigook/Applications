using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Context
{
	public class MyKeysContext : DbContext, IDataProtectionKeyContext
	{
		public MyKeysContext(DbContextOptions<MyKeysContext> options) 
			: base(options) 
		{ 
		}

		public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
	}
}