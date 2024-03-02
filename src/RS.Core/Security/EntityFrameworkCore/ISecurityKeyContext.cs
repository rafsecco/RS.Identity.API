using Microsoft.EntityFrameworkCore;
using RS.Core.Security.Model;

namespace RS.Core.Security.EntityFrameworkCore;

public interface ISecurityKeyContext
{
	/// <summary>
	/// A collection of <see cref="T:NetDevPack.Security.Jwt.Core.Model.KeyMaterial" />
	/// </summary>
	DbSet<KeyMaterial> SecurityKeys { get; set; }
}

