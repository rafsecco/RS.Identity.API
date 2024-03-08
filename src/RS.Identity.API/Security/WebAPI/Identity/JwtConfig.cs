using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RS.Identity.API.Security.JwtExtensions;

namespace RS.Identity.API.Security.WebAPI.Identity;

public static class JwtConfig
{
	public static void AddJwtConfiguration(this WebApplicationBuilder builder)
	{
		var appSettingsSection = builder.Configuration.GetSection("AppSettings");

		builder.Services.Configure<AppSettings>(appSettingsSection);

		var appSettings = appSettingsSection.Get<AppSettings>();

		builder.Services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(x =>
		{
			x.RequireHttpsMetadata = false; // deixar como false para ambiente sem HTTPS
			x.SaveToken = true;
			x.SetJwksOptions(new JwkOptions(appSettings.AutenticacaoJwksUrl));
		});
	}

	public static void UseAuthConfiguration(this IApplicationBuilder app)
	{
		app.UseAuthentication();
		app.UseAuthorization();
	}
}
