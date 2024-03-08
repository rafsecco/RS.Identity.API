using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RS.Identity.API.Security.Interfaces;

namespace RS.Identity.API.Security.EntityFrameworkCore;

/// <summary>
/// Builder extension methods for registering crypto services
/// </summary>
public static class EFCoreServiceExtensions
{
	/// <summary>
	/// Sets the signing credential.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <param name="credential">The credential.</param>
	/// <returns></returns>
	public static IJwksBuilder PersistKeysToDatabaseStore<TContext>(this IJwksBuilder builder) where TContext : DbContext, ISecurityKeyContext
	{
		builder.Services.AddScoped<IJsonWebKeyStore, DatabaseJsonWebKeyStore<TContext>>();

		return builder;
	}
}
