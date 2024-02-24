using Microsoft.OpenApi.Models;

namespace RS.Identity.API.Configurations;

public static class SwaggerConfig
{
	public static WebApplicationBuilder AddSwaggerConfiguration(this WebApplicationBuilder builder)
	{
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "RS.Identity.API", Version = "v1" });
		});
		return builder;
	}

	public static WebApplication UseSwaggerConfiguration(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RS.Identity.API v1"));
		}
		return app;
	}
}
