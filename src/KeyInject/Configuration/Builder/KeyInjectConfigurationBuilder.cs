using System.Text.RegularExpressions;
using KeyInject.Common;
using KeyInject.Configuration.Models;

namespace KeyInject.Configuration.Builder;

/// <summary>
/// Simple builder for <see cref="KeyInjectConfiguration"/>  
/// </summary>
public sealed class KeyInjectConfigurationBuilder : IKeyInjectConfigurationBuilder
{
	private readonly List<string> _pickedPresetPatterns = new();
	private readonly List<string> _rawRegexPatterns = new();
	private KeyInjectConfiguration _configurationResult = KeyInjectConfiguration.Default;

	/// <summary>
	/// Enrich <see cref="KeyInjectConfiguration"/> with <see cref="IConfigurationSection"/> if provided.<br/>
	/// If <see cref="IConfigurationSection"/> not provided, using <b>"KeyInject"</b> name.<br/><br/>
	/// Warning! This method will overwrite <see cref="KeyInjectConfiguration"/> properties, and extend internal colletions.  
	/// </summary>
	internal IKeyInjectConfigurationBuilder EnrichFromAppSettings(IConfiguration section)
	{
		var settings = section.Get<KeyInjectConfigurationAppSettings>();
		if (settings is null) return this;

		_configurationResult.IgnoreCase = settings.IgnoreCase.HasValue
			? settings.IgnoreCase.Value : _configurationResult.IgnoreCase;
		_configurationResult.ReplaceRepeatCount = settings.ReplaceRepeatCount.HasValue
			? settings.ReplaceRepeatCount.Value : _configurationResult.ReplaceRepeatCount;
		_configurationResult.KeyPrefixes.AddRange(
			settings.KeyPrefixes?.Where(x => string.IsNullOrEmpty(x) is false) ?? []
		);
		_rawRegexPatterns.AddRange(
			settings.RegexPatterns?.Where(x => string.IsNullOrEmpty(x) is false) ?? []
		);
		_pickedPresetPatterns.AddRange(
			settings.Patterns?.Where(x => string.IsNullOrEmpty(x) is false) ?? []
		);
		return this;
	}
	
	public IKeyInjectConfigurationBuilder SetEnabled(bool isEnabled)
	{
		_configurationResult.Enabled = isEnabled;
		return this;
	}
	public IKeyInjectConfigurationBuilder SetReplaceRepeatCount(int maxReplaceRepeatCount)
	{
		_configurationResult.ReplaceRepeatCount = maxReplaceRepeatCount;
		return this;
	}
	public IKeyInjectConfigurationBuilder SetIgnoreCase(bool ignoreCase)
	{
		_configurationResult.IgnoreCase = ignoreCase;
		return this;
	}
	public IKeyInjectConfigurationBuilder AddKeyPrefix(string prefix)
	{
		if (string.IsNullOrWhiteSpace(prefix)) return this;
		_configurationResult.KeyPrefixes.Add(prefix.Trim());
		return this;
	}
	public IKeyInjectConfigurationBuilder AddRegexPattern(Regex pattern)
	{
		_configurationResult.RegexPatterns.Add(pattern);
		return this;
	}
	public IKeyInjectConfigurationBuilder AddRegexPattern(string pattern)
	{
		if (string.IsNullOrWhiteSpace(pattern)) return this;
		_rawRegexPatterns.Add(pattern);
		return this;
	}
	
	public KeyInjectConfiguration Build()
	{
		// adding preset regex patterns from static internal dictionary 
		foreach (var pattern in _pickedPresetPatterns)
			if (InjectDefaults.PresetPatterns.TryGetValue(pattern.Trim(), out var presetRegex))
				_configurationResult.RegexPatterns.Add(presetRegex);

		foreach (var pattern in _rawRegexPatterns) 
			if (RegexParser.TryParse(pattern, _configurationResult.IgnoreCase, out var regex))
				_configurationResult.RegexPatterns.Add(regex!);

		// adding default regex - is default behavior of this package
		if(_configurationResult.RegexPatterns.Count == 0)
			_configurationResult.RegexPatterns.Add(InjectDefaults.DefaultRegex);
		
		return _configurationResult;
	}

	public override string ToString() => _configurationResult.ToString();
}