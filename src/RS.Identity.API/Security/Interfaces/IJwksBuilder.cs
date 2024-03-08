using Microsoft.Extensions.DependencyInjection;

namespace RS.Identity.API.Security.Interfaces;

public interface IJwksBuilder
{
	IServiceCollection Services { get; }
}
