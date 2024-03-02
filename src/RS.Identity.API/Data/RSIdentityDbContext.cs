using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RS.Core.Security.EntityFrameworkCore;
using RS.Core.Security.Model;

namespace RS.Identity.API.Data;

public class RSIdentityDbContext : IdentityDbContext, ISecurityKeyContext
{
	public RSIdentityDbContext(DbContextOptions<RSIdentityDbContext> options) : base(options) { }

	public DbSet<KeyMaterial> SecurityKeys { get; set; }
}
