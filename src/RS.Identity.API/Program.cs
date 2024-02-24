using RS.Identity.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder
	.AddApiConfiguration()
	.AddSwaggerConfiguration();

var app = builder.Build();

app.UseApiConfiguration()
	.UseSwaggerConfiguration();

app.Run();