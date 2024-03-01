using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RS.Identity.API.Data;
using RS.Identity.API.Extensions;
using System.Text;

namespace RS.Identity.API.Configurations;

public static class IdentityConfig
{
	public static WebApplicationBuilder AddIdentityConfiguration(this WebApplicationBuilder builder)
	{
		builder.Services.AddDefaultIdentity<IdentityUser>(options =>
		{
			options.SignIn.RequireConfirmedAccount = true;
		})
			.AddRoles<IdentityRole>()
			.AddErrorDescriber<IdentityMensagensPortugues>()
			.AddEntityFrameworkStores<RSIdentityDbContext>()
			.AddDefaultTokenProviders();

		//JWT
		var appSettingsSection = builder.Configuration.GetSection("AppSettings");
		builder.Services.Configure<AppSettings>(appSettingsSection);

		var appSettings = appSettingsSection.Get<AppSettings>();
		var key = Encoding.ASCII.GetBytes(appSettings.Secret);

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(bearerOptions =>
		{
			bearerOptions.RequireHttpsMetadata = true;
			bearerOptions.SaveToken = true;
			bearerOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = appSettings.Issuer,
				ValidateAudience = true,
				ValidAudience = appSettings.Audience
			};
		});

		return builder;
	}

	public static WebApplication UseIdentityConfiguration(this WebApplication app)
	{
		app.UseAuthentication();
		app.UseAuthorization();
		return app;
	}
}
