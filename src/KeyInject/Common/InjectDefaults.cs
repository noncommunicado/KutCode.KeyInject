using System.Text.RegularExpressions;

namespace KeyInject.Common;

/// <summary>
/// Default values of many part of package
/// </summary>
public sealed class InjectDefaults
{
	public const string RegexInjectionGroupKey = "key";
	public const string RegexPatternPlaceholder = "_";
	public const string DefaultRegexPatternRaw = "${_}";
	public static Dictionary<string, Regex> PresetPatterns = new() {
		{DefaultRegexPatternRaw, new Regex(@"\$\{(?<key>[^\{\}]+)\}")},
		{"{{_}}", new Regex(@"\{\{(?<key>[^\{\}]+)\}\}")},
		{"$<_>", new Regex(@"\$<(?<key>[^<>]+)>")},
		{"<<_>>", new Regex(@"<<(?<key>[^<>]+)>>")},
		{"!{_}!", new Regex(@"!\{(?<key>[^{}]+)\}!")},
		{"%_%", new Regex(@"%(?<key>[^%]+)%")},
	};
	public static Regex DefaultRegex => PresetPatterns[DefaultRegexPatternRaw];
	public const string DefaultConfigurationSectionName = "KeyInject";

	/// <summary>
	/// Enough to cover greater part of nested keys.
	/// </summary>
	public const int DefaultReplacementIterationsCount = 5;
}