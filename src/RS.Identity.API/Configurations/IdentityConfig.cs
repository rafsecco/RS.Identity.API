using Microsoft.AspNetCore.Identity;
using RS.Identity.API.Security;
using RS.Identity.API.Security.AspNetCore;
using RS.Identity.API.Security.EntityFrameworkCore;
using RS.Identity.API.Security.Jwa;
using RS.Identity.API.Data;
using RS.Identity.API.Extensions;

namespace RS.Identity.API.Configurations;

public static class IdentityConfig
{
	public static WebApplicationBuilder AddIdentityConfiguration(this WebApplicationBuilder builder)
	{
		var appSettingsSection = builder.Configuration.GetSection("AppTokenSettings");
		builder.Services.Configure<AppTokenSettings>(appSettingsSection);

		builder.Services
			.AddJwksManager(options => options.Jws = Algorithm.Create(DigitalSignaturesAlgorithm.EcdsaSha256))
			.PersistKeysToDatabaseStore<RSIdentityDbContext>()
			.UseJwtValidation();

		builder.Services.AddDefaultIdentity<IdentityUser>()
			.AddRoles<IdentityRole>()
			.AddErrorDescriber<IdentityMensagensPortugues>()
			.AddEntityFrameworkStores<RSIdentityDbContext>()
			.AddDefaultTokenProviders();

		return builder;
	}

	public static WebApplication UseIdentityConfiguration(this WebApplication app)
	{
		app.UseAuthentication();
		app.UseAuthorization();
		return app;
	}
}
