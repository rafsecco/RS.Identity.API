using Microsoft.Extensions.DependencyInjection;

namespace RS.Core.Security.Interfaces;

public interface IJwksBuilder
{
	IServiceCollection Services { get; }
}
