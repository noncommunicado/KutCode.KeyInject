using System.Text.RegularExpressions;
using KeyInject.Configuration.Models;

namespace KeyInject.Configuration.Builder;

/// <summary>
/// KeyInject simple configuration builder
/// </summary>
public interface IKeyInjectConfigurationBuilder
{
	IKeyInjectConfigurationBuilder EnrichFromAppSettings(IConfiguration section);
	IKeyInjectConfigurationBuilder EnrichFromAppSettings(Func<IConfiguration, IConfiguration> configurationFunc);
	IKeyInjectConfigurationBuilder SetEnabled(bool isEnabled);
	IKeyInjectConfigurationBuilder SetReloadEnabled(bool isReloadEnabled);
	IKeyInjectConfigurationBuilder SetReplaceRepeatCount(int maxReplaceRepeatCount);
	IKeyInjectConfigurationBuilder SetIgnoreCase(bool ignoreCase);
	IKeyInjectConfigurationBuilder AddKeyPrefix(string prefix);
	IKeyInjectConfigurationBuilder AddRegexPattern(Regex pattern);
	IKeyInjectConfigurationBuilder AddRegexPattern(string pattern);
	IKeyInjectConfigurationBuilder AddPresetPattern(string presetPattern);
	KeyInjectConfiguration Build();
}