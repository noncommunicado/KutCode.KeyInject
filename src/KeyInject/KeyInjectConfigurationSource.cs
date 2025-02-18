using KeyInject.Configuration.Models;

namespace KeyInject;

public sealed class KeyInjectConfigurationSource(KeyInjectConfiguration configuration) : IConfigurationSource
{
	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new KeyInjectConfigurationProvider(configuration, builder);
	}
}