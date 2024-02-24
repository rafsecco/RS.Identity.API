using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RS.Identity.API.Data;

public class RSIdentityDbContext : IdentityDbContext
{
	public RSIdentityDbContext(DbContextOptions<RSIdentityDbContext> options) : base(options) { }
}
