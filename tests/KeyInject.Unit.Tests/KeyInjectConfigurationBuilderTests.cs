using System.Text.RegularExpressions;
using FluentAssertions;
using KeyInject.Common;
using KeyInject.Configuration.Builder;
using KeyInject.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace KeyInject.Unit.Tests
{
	[TestFixture]
	public sealed class KeyInjectConfigurationBuilderTests
	{
		private readonly KeyInjectConfiguration _defaultConfig = KeyInjectConfiguration.Default;
		private KeyInjectConfigurationBuilder Builder { get; set; }
		[SetUp]
		public void SetUp() => Builder = new(new ConfigurationManager());
	
		[Test]
		public void CallBuildOnly_OutDefaultValue_Test()
		{
			var config = Builder.Build();
		
			config.Should().NotBeNull();
			config.IgnoreCase.Should().Be(_defaultConfig.IgnoreCase);
			config.ReplaceRepeatCount.Should().Be(_defaultConfig.ReplaceRepeatCount);
			config.RegexPatterns.Should().ContainSingle(x => x == InjectDefaults.DefaultRegex);
			config.KeyPrefixes.Should().BeEmpty();
		}
	
		[Test]
		public void AddRawCorrectRegex_OutParsedRegex_Test()
		{
			var rawRegex = @"\$\{([^\{\}]+)\}";
			Builder.AddRegexPattern(rawRegex);
			var config = Builder.Build();
		
			config.Should().NotBeNull();
			config.RegexPatterns.Should().HaveCount(1);
			config.RegexPatterns.Should().ContainSingle(x => x.ToString() == rawRegex);
		}
	
		[Test]
		public void AddBuiltRegex_OutParsedRegex_Test()
		{
			var rawRegex = new Regex(@"\$\{([^\{\}]+)\}");
			Builder.AddRegexPattern(rawRegex);
			var config = Builder.Build();
		
			config.Should().NotBeNull();
			config.RegexPatterns.Should().ContainSingle(x => x == rawRegex);
		}
	
		[Test]
		public void AddEmptyRegex_OutEmptyRegexs_Test()
		{
			var regex = "    ";
			Builder.AddRegexPattern(regex);
			var config = Builder.Build();

			config.Should().NotBeNull();
			config.RegexPatterns.Should().ContainSingle(x => x == InjectDefaults.DefaultRegex);
		}
	
		[Test]
		public void AddNotEmptyPrefix_OutPrefix_Test()
		{
			var prefix = "pre_";
			Builder.AddKeyPrefix(prefix);
			var config = Builder.Build();
		
			config.Should().NotBeNull();
			config.KeyPrefixes.Should().HaveCount(1);
			config.KeyPrefixes.Should().ContainSingle(x => x == prefix);
		}
	
		[Test]
		public void AddEmptyPrefix_OutEmptyPrefixes_Test()
		{
			var prefix = "    ";
			Builder.AddKeyPrefix(prefix);
			var config = Builder.Build();
		
			config.Should().NotBeNull();
			config.KeyPrefixes.Should().BeEmpty();
		}
	}
}