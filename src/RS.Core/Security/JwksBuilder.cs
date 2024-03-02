using Microsoft.Extensions.DependencyInjection;
using RS.Core.Security.Interfaces;

namespace RS.Core.Security;

public class JwksBuilder : IJwksBuilder
{
	public JwksBuilder(IServiceCollection services)
	{
		Services = services ?? throw new ArgumentNullException(nameof(services));
	}

	public IServiceCollection Services { get; }
}
