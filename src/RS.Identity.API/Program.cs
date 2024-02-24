using RS.Identity.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder
	.AddApiConfig()
	.AddSwaggerConfig();

var app = builder.Build();

app.UseApiConfigure()
	.UseSwaggerConfigure();

app.Run();