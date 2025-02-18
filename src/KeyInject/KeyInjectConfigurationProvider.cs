
using KeyInject.Configuration.Builder;
using KeyInject.Configuration.Models;
using KeyInject.Injection;

namespace KeyInject;

public sealed class KeyInjectConfigurationProvider(KeyInjectConfiguration configuration, IConfigurationBuilder builder) : ConfigurationProvider
{
	/// <summary>
	/// Main entry for Key Injection
	/// </summary>
	public override void Load()
	{
		//var manager = builder as ConfigurationManager;
		new InjectionProcessor(new KeyInjectConfigurationBuilder().Build())
			.Process(builder);
		return;
		// var envVars = Environment.GetEnvironmentVariables();
		//
		//
		// List<IConfigurationSection> envs = new List<IConfigurationSection>();
		// List<IConfigurationSection> others = new List<IConfigurationSection>();
		// foreach (var configurationSection in manager?.GetChildren() ?? []) {
		// 	//if (configurationSection.GetType() == typeof(EnvironmentVariablesCon))
		// }
		//
		// ConfigurationSection a = null;
		// List<IConfigurationSection> configurationSections = (manager?.GetChildren() ?? []).ToList();
		//
		// foreach (var sec in configurationSections) {
		// 	//sec.Value = "2222";
		// }

		//${N}
		//N
	}
}