using Microsoft.EntityFrameworkCore;
using RS.Identity.API.Security.Model;

namespace RS.Identity.API.Security.EntityFrameworkCore;

public interface ISecurityKeyContext
{
	/// <summary>
	/// A collection of <see cref="T:NetDevPack.Security.Jwt.Core.Model.KeyMaterial" />
	/// </summary>
	DbSet<KeyMaterial> SecurityKeys { get; set; }
}

