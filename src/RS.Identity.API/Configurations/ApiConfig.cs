namespace RS.Identity.API.Configurations;

public static class ApiConfig
{
	public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
	{
		//builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
		//	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

		builder.Services.AddControllers();
		//.ConfigureApiBehaviorOptions(options =>
		//{
		//	options.SuppressModelStateInvalidFilter = true;
		//});
		return builder;
	}

	public static WebApplication UseApiConfigure(this WebApplication app)
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
