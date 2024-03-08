using RS.Identity.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder
	.AddIdentityConfiguration()
	.AddApiConfiguration()
	.AddSwaggerConfiguration();

var app = builder.Build();

app.UseApiConfiguration()
	.UseSwaggerConfiguration();

app.Run();