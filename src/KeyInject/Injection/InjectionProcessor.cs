using System.Collections.Frozen;
using System.Text.RegularExpressions;
using KeyInject.Common;
using KeyInject.Configuration.Models;

namespace KeyInject.Injection;

internal sealed class InjectionProcessor(KeyInjectConfiguration injectConfig)
{
	private readonly InjectStringComparer _stringComparer = new(injectConfig);

	public static InjectionProcessor Create(KeyInjectConfiguration injectConfig) => new(injectConfig);

	public void Process(IConfigurationBuilder builder)
	{
		if (injectConfig.Enabled is false) return;

		if (builder is not ConfigurationManager manager)
			throw new ArgumentOutOfRangeException();

		int counter = 0;
		while (counter < injectConfig.ReplaceRepeatCount) {
			counter++;
			Process(manager);
		}
	}

	/// <summary>
	/// Find all parent and children (recursively) config sections with value.  
	/// Section without value ignored (usually, it's parent sections).  
	/// </summary>
	internal Dictionary<string, IConfigurationSection> FlattenConfigurationSection(
		IEnumerable<IConfigurationSection> sections,
		Dictionary<string, IConfigurationSection>? collection = null)
	{
		Dictionary<string, IConfigurationSection> newCollection = collection ?? new();
		foreach (var section in sections) {
			var children = section.GetChildren() as IConfigurationSection[] 
			               ?? section.GetChildren().ToArray();
			if (children.NotEmptyOrNull())
				FlattenConfigurationSection(children, newCollection);
			if (section.Value is null) continue;
			var key = injectConfig.IgnoreCase
				? section.Path.ToLowerInvariant() : section.Path;
			newCollection.Add(key, section);
		}
		return newCollection;
	}
	
	private void Process(ConfigurationManager configurationManager)
	{
		// key -- config section
		var sectionsByKeys =
			FlattenConfigurationSection(configurationManager.GetChildren())
				.ToFrozenDictionary();
		foreach (var section in sectionsByKeys)
			foreach (var regex in injectConfig.RegexPatterns)
				HandleRegex(regex, section, sectionsByKeys);
	}

	private void HandleRegex(
		Regex regex,
		KeyValuePair<string, IConfigurationSection> section,
		FrozenDictionary<string, IConfigurationSection> sectionsByKeys)
	{
		if (string.IsNullOrEmpty(section.Value.Value)) return;
		var matches = regex.Matches(section.Value.Value);
		foreach (Match match in matches) {
			var presentedInMatch = match.Groups
				.TryGetValue(InjectDefaults.RegexInjectionGroupKey, out var keyGroup);
			if (presentedInMatch is false) continue;
			// cleared key: "${Some_Key}" --> "Some_Key"
			var key = injectConfig.IgnoreCase 
				? keyGroup!.Value.ToLowerInvariant() : keyGroup!.Value;
			// check for prefixe satisfy if presented
			var prefixFilter =  injectConfig.KeyPrefixes.IsNullOrEmpty() || 
				injectConfig.KeyPrefixes.Any(x 
						=> key.StartsWith(x, _stringComparer.StringComparison)
					);
			if (prefixFilter is false) continue;
			// finally, string replacement
			if (sectionsByKeys.TryGetValue(key, out var configSection)) {
				if (string.IsNullOrEmpty(configSection.Value)) continue;
				section.Value.Value = section.Value.Value
					.Replace(match.Value, configSection.Value);
			}
		}
	}
}