using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RS.Identity.API.Security.EntityFrameworkCore;
using RS.Identity.API.Security.Model;
using RS.Identity.API.Models;

namespace RS.Identity.API.Data;

public class RSIdentityDbContext : IdentityDbContext, ISecurityKeyContext
{
	public RSIdentityDbContext(DbContextOptions<RSIdentityDbContext> options) : base(options) { }

	public DbSet<KeyMaterial> SecurityKeys { get; set; }

	public DbSet<RefreshToken> RefreshTokens { get; set; }
}
