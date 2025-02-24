using FluentAssertions;
using KeyInject.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KeyInject.Integration.Tests;

[TestFixture]
public sealed class SimpleInjectionTests
{
	private const string EnvVariableName = "VAR_NAME";
	private const string EnvVariableValue = "VALUE";
	private const string ConfigTestKey = "SomeKey";
	private string _stringForReplaceOn = InjectDefaults.DefaultRegexPatternRaw
		.Replace(InjectDefaults.RegexPatternPlaceholder, EnvVariableName);

	private IConfiguration Configuration { get; set; }
	
	[OneTimeSetUp]
	public void Setup()
	{
		Environment.SetEnvironmentVariable(EnvVariableName, EnvVariableValue);
		Environment.SetEnvironmentVariable("replace_me", "some replace text");
		Configuration = new ConfigurationManager()
			.AddInMemoryCollection(new Dictionary<string, string?> {
				{ ConfigTestKey, _stringForReplaceOn }
			})
			.AddEnvironmentVariables()
			.AddJsonFile("appsettings.json")
			.AddKeyInject(LoggerFactory.Create(builder =>
			{
				builder
					.SetMinimumLevel(LogLevel.Debug)
					.AddConsole()
					.AddDebug();
			}))
			.Build();
	}

	[Test]
	public void SimpleReplaceTest()
	{
		Configuration[ConfigTestKey].Should().NotBeNullOrEmpty();
		Configuration[ConfigTestKey].Should().Be(EnvVariableValue);
		Configuration["AppSettingsKey"].Should().Be("Value some replace text");
		Configuration["Key-1"].Should().Be("Value is Value2");
	}
}