using System.ComponentModel;

namespace KeyInject.Configuration.Models;

/// <summary>
/// Model for appsetting.json mapping
/// </summary>
public sealed record KeyInjectConfigurationAppSettings
{
	/// <summary>
	/// Does key injection enabled. Simple switch. True by default.
	/// </summary>
	[DefaultValue(true)]
	public bool Enabled { get; init; } = true;
	/// <summary>
	/// Allows to reload configuration if other Configuration Provides triggers OnReload()
	/// </summary>
	[DefaultValue(true)]
	public bool ReloadEnabled { get; set; } = true;
	public int? ReplaceRepeatCount { get; init; }
	public bool? IgnoreCase { get; init; }
	public string[]? KeyPrefixes { get; init; }
	public string[]? Patterns { get; init; }
	public string[]? RegexPatterns { get; init; }
}