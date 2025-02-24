using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using KeyInject.Common;

namespace KeyInject.Configuration.Models;

public sealed record KeyInjectConfiguration
{
	/// <summary>
	/// Does key injection enabled. Simple switch. True by default.
	/// </summary>
	[DefaultValue(true)]
	public bool Enabled { get; set; } = true;
	
	private int _replaceRepeatCount = InjectDefaults.DefaultReplacementIterationsCount;
	public int ReplaceRepeatCount {
		get => _replaceRepeatCount;
		set => _replaceRepeatCount = value > 0 ? value : 1;
	}
	public bool IgnoreCase { get; set; } = false;
	public List<string> KeyPrefixes { get; set; } = new();
	public List<Regex> RegexPatterns { get; set; } = new();

	public static KeyInjectConfiguration Default => new();

	public override string ToString() => JsonSerializer.Serialize(new {
		Enabled, ReplaceRepeatCount, IgnoreCase, KeyPrefixes,
		RegexPatterns = RegexPatterns.Select(x => x.ToString()).ToArray()
	});
}