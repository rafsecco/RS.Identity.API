using RS.Identity.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder
	.AddApiConfiguration()
	.AddIdentityConfiguration()
	.AddSwaggerConfiguration();

var app = builder.Build();

app.UseApiConfiguration()
	.UseSwaggerConfiguration();

app.Run();