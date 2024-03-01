using Microsoft.OpenApi.Models;

namespace RS.Identity.API.Configurations;

public static class SwaggerConfig
{
	public static WebApplicationBuilder AddSwaggerConfiguration(this WebApplicationBuilder builder)
	{
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "RS.Identity.API",
				Version = "v1",
				Description = "Api to manage user authentication and authorization",
				Contact = new OpenApiContact() { Name = "Rafael Secco", Email = "rafaelsecco.dev@google.com" },
				License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/license/MIT") }
			});
		});
		return builder;
	}

	public static WebApplication UseSwaggerConfiguration(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
		}
		return app;
	}
}
