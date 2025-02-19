using System.Text.RegularExpressions;
using KeyInject.Configuration.Models;

namespace KeyInject.Configuration.Builder;

public interface IKeyInjectConfigurationBuilder
{
	IKeyInjectConfigurationBuilder EnrichFromAppSettings(IConfiguration section);
	IKeyInjectConfigurationBuilder EnrichFromAppSettings(Func<IConfiguration, IConfiguration> configurationFunc);
	IKeyInjectConfigurationBuilder SetEnabled(bool isEnabled);
	IKeyInjectConfigurationBuilder SetReplaceRepeatCount(int maxReplaceRepeatCount);
	IKeyInjectConfigurationBuilder SetIgnoreCase(bool ignoreCase);
	IKeyInjectConfigurationBuilder AddKeyPrefix(string prefix);
	IKeyInjectConfigurationBuilder AddRegexPattern(Regex pattern);
	IKeyInjectConfigurationBuilder AddRegexPattern(string pattern);
	KeyInjectConfiguration Build();
}