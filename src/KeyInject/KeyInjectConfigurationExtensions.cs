using KeyInject.Configuration.Builder;
using KeyInject.Configuration.Models;

namespace KeyInject;

public static class KeyInjectConfigurationExtensions
{
	/// <summary>
	/// Add KeyInject with default configuraiton. <br/>
	/// Default configuration includes ${} pattern and 5 cycles.
	/// </summary>
	public static IConfigurationBuilder AddKeyInject(
		this IConfigurationBuilder manager)
	{
		var configuration = manager.Build();
		var injectBuilder = new KeyInjectConfigurationBuilder(configuration);
		return manager.Add(new KeyInjectConfigurationSource(injectBuilder.Build()));
	}
	
	/// <summary>
	/// Add KeyInject with configuraiton builder. <br/>
	/// Default configuration includes ${} pattern and 5 cycles.
	/// </summary>
	public static IConfigurationBuilder AddKeyInject(
		this IConfigurationBuilder manager,
		Func<IKeyInjectConfigurationBuilder, IKeyInjectConfigurationBuilder> builder)
	{
		var configuration = manager.Build();
		var injectBuilder = new KeyInjectConfigurationBuilder(configuration);
		return manager.Add(new KeyInjectConfigurationSource(
			builder.Invoke(injectBuilder).Build())
		);
	}
}