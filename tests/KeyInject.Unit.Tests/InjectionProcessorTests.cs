using FluentAssertions;
using KeyInject.Configuration;
using KeyInject.Configuration.Models;
using KeyInject.Injection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace KeyInject.Unit.Tests;

[TestFixture]
public sealed class InjectionProcessorTests
{
	private IConfigurationRoot TestConfigurationRoot { get; set; }
	
	[SetUp]
	public void SetUp()
	{
		var json = @"{
            ""section_1"": {
                ""CHILD_1"": ""value_1"",
                ""child_2"": ""value_2"",
				""child_3"": {
					""CHILD_4"": ""value_4""
				}
            }
        }";

		TestConfigurationRoot = new ConfigurationBuilder()
			.AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
			.Build();
	}
	
	[Test]
	public void FlattenConfigurationSection_MatchCase_Success()
	{
		var resultSequence = new InjectionProcessor(
				KeyInjectConfiguration.Default,
				new MemoryConfigurationProvider(new MemoryConfigurationSource())
			)
			.FlattenConfiguration(TestConfigurationRoot);

		resultSequence.Should().NotBeNull();
		resultSequence.Should().NotBeEmpty();
		resultSequence.Should().Contain(x => x.Key == "section_1:CHILD_1");
		resultSequence.Should().Contain(x => x.Key == "section_1:child_2");
		resultSequence.Should().Contain(x => x.Key == "section_1:child_3:CHILD_4");
	}
	
	[Test]
	public void FlattenConfigurationSection_IgnoreCase_Success()
	{
		var configProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource());
		var resultSequence = new InjectionProcessor(KeyInjectConfiguration.Default with {
				IgnoreCase = true
			}, configProvider)
			.FlattenConfiguration(TestConfigurationRoot);
		
		resultSequence.Should().NotBeNull();
		resultSequence.Should().NotBeEmpty();

		resultSequence.Should().NotBeNull();
		resultSequence.Should().NotBeEmpty();
		resultSequence.Should().Contain(x => x.Key == "section_1:CHILD_1");
		resultSequence.Should().Contain(x => x.Key == "section_1:child_2");
		resultSequence.Should().Contain(x => x.Key == "section_1:child_3:CHILD_4");
	}
}