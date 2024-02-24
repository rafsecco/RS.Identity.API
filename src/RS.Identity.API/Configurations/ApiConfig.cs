using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RS.Identity.API.Data;

namespace RS.Identity.API.Configurations;

public static class ApiConfig
{
	public static WebApplicationBuilder AddApiConfiguration(this WebApplicationBuilder builder)
	{
		builder.Services.AddControllers();

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

		builder.Services.AddDefaultIdentity<IdentityUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
			})
			//.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<RSIdentityDbContext>();

		return builder;
	}

	public static WebApplication UseApiConfiguration(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		return app;
	}
}
