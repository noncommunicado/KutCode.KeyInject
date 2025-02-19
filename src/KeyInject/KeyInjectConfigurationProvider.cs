using KeyInject.Configuration.Models;
using KeyInject.Injection;

namespace KeyInject;

public sealed class KeyInjectConfigurationProvider(
	KeyInjectConfiguration configuration, IConfigurationBuilder builder) : ConfigurationProvider
{
	/// <summary>
	/// Main entry for Key Injection
	/// </summary>
	public override void Load()
	{
		InjectionProcessor
			.Create(configuration)
			.Process(builder);
	}
}