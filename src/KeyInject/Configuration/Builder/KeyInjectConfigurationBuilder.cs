using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KeyInject.Common;
using KeyInject.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace KeyInject.Configuration.Builder
{
	/// <summary>
	/// Simple builder for <see cref="KeyInjectConfiguration"/>  
	/// </summary>
	public sealed class KeyInjectConfigurationBuilder : IKeyInjectConfigurationBuilder
	{
		private readonly List<string> _pickedPresetPatterns = new();
		private readonly List<string> _rawRegexPatterns = new();
		private IConfiguration? _configurationSection = null;
		private KeyInjectConfiguration _configurationResult = KeyInjectConfiguration.Default;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Simple builder for <see cref="KeyInjectConfiguration"/>  
		/// </summary>
		public KeyInjectConfigurationBuilder(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Enrich <see cref="KeyInjectConfiguration"/> with <see cref="IConfigurationSection"/> if provided.<br/>
		/// If <see cref="IConfigurationSection"/> not provided, using <b>"KeyInject"</b> name.<br/><br/>
		/// Warning! This method will overwrite <see cref="KeyInjectConfiguration"/> properties, and extend internal colletions.  
		/// </summary>
		public IKeyInjectConfigurationBuilder EnrichFromAppSettings(IConfiguration? section)
		{
			if (section is null) return this;
			_configurationSection = section;
			return this;
		}
		/// <inheritdoc cref="EnrichFromAppSettings(Microsoft.Extensions.Configuration.IConfiguration?)"/>
		public IKeyInjectConfigurationBuilder EnrichFromAppSettings(Func<IConfiguration, IConfiguration> configurationFunc)
		{
			EnrichFromAppSettings(configurationFunc.Invoke(_configuration));
			return this;
		}

		public IKeyInjectConfigurationBuilder SetEnabled(bool isEnabled)
		{
			_configurationResult.Enabled = isEnabled;
			return this;
		}

		public IKeyInjectConfigurationBuilder SetReloadEnabled(bool isReloadEnabled)
		{
			_configurationResult.ReloadEnabled = isReloadEnabled;
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

		public IKeyInjectConfigurationBuilder AddPresetPattern(string presetPattern)
		{
			if (string.IsNullOrWhiteSpace(presetPattern)) return this;
			if (InjectDefaults.PresetPatterns.TryGetValue(presetPattern.Trim(), out var regex))
				_configurationResult.RegexPatterns.Add(regex);
			return this;
		}

		public KeyInjectConfiguration Build()
		{
			EnrichFromAppSettings();

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
	
		private void EnrichFromAppSettings()
		{
			if (_configurationSection is null)
				_configurationSection = _configuration.GetSection(InjectDefaults.DefaultConfigurationSectionName);
			var settings = _configurationSection.Get<KeyInjectConfigurationAppSettings>();
			if (settings is null) return;

			_configurationResult.IgnoreCase = settings.IgnoreCase.HasValue
				? settings.IgnoreCase.Value : _configurationResult.IgnoreCase;
			_configurationResult.ReplaceRepeatCount = settings.ReplaceRepeatCount.HasValue
				? settings.ReplaceRepeatCount.Value : _configurationResult.ReplaceRepeatCount;
			_configurationResult.KeyPrefixes.AddRange(
				settings.KeyPrefixes?.Where(x => string.IsNullOrEmpty(x) is false) ?? new List<string> { }.AsReadOnly()
			);
			_rawRegexPatterns.AddRange(
				settings.RegexPatterns?.Where(x => string.IsNullOrEmpty(x) is false) ?? new List<string> { }.AsReadOnly()
			);
			_pickedPresetPatterns.AddRange(
				settings.Patterns?.Where(x => string.IsNullOrEmpty(x) is false) ?? new List<string> { }.AsReadOnly()
			);
		}

		public override string ToString() => _configurationResult.ToString();
	}
}