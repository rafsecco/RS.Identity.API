using Microsoft.EntityFrameworkCore;
using RS.Identity.API.Security.AspNetCore;
using RS.Identity.API.Security.WebAPI.User;
using RS.Identity.API.Data;
using RS.Identity.API.Services;
using System.Reflection;

namespace RS.Identity.API.Configurations;

public static class ApiConfig
{
	public static WebApplicationBuilder AddApiConfiguration(this WebApplicationBuilder builder)
	{
		builder.Configuration
			.SetBasePath(builder.Environment.ContentRootPath)
			.AddJsonFile("appsettings.json", true, true)
			.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
			.AddEnvironmentVariables()
			.AddUserSecrets(Assembly.GetExecutingAssembly(), true);

		builder.Services.AddControllers();

		builder.Services.AddScoped<AuthenticationService>();
		builder.Services.AddScoped<IAspNetUser, AspNetUser>();

		string? strConn = builder.Configuration.GetConnectionString("DefaultConnection");

		builder.Services.AddDbContext<RSIdentityDbContext>(options =>
			options.UseSqlServer(strConn, e =>
			{
				e.EnableRetryOnFailure(
					maxRetryCount: 3,
					maxRetryDelay: TimeSpan.FromSeconds(6),
					errorNumbersToAdd: null);
			})
			.LogTo(Console.WriteLine)
			.EnableSensitiveDataLogging()
			.EnableDetailedErrors());

		return builder;
	}

	public static WebApplication UseApiConfiguration(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseIdentityConfiguration();
		app.MapControllers();

		// Endpoint personalizado: localhost:porta/minha-chave
		//app.UseJwksDiscovery("/minha-chave");
		// Endpoint default: localhost:porta/jwks
		app.UseJwksDiscovery(); //Responsavel pelo EndPoint da chave publica
		return app;
	}
}
