using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RS.Core.Security.Interfaces;
using RS.Core.Security.Jwt;
using RS.Core.Security.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RS.Core.Security.AspNetCore;

public class JwtServiceDiscoveryMiddleware
{
	private readonly RequestDelegate _next;

	public JwtServiceDiscoveryMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext httpContext, IJwtService keyService, IOptions<JwtOptions> options)
	{
		var storedKeys = await keyService.GetLastKeys(options.Value.AlgorithmsToKeep);
		var keys = new
		{
			keys = storedKeys.Select(s => s.GetSecurityKey()).Select(PublicJsonWebKey.FromJwk)
		};
		httpContext.Response.ContentType = "application/json";
		await httpContext.Response.WriteAsync(JsonSerializer.Serialize(keys, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
	}
}
