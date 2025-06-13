using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace KeyInject.Integration.Tests
{
	[TestFixture]
	public sealed class NestingInjectionTests
	{
		private IConfiguration Configuration { get; set; }
	
		[OneTimeSetUp]
		public void Setup()
		{
			Configuration = new ConfigurationManager()
				.AddJsonFile("appsettings.json")
				.AddKeyInject(x => x)
				.Build();
		}

		[Test]
		public void SimpleReplaceTest()
		{
			Configuration["nest-key-1"].Should().Be("this is kind a magic");
			Configuration["nest-key-2"].Should().Be("is kind a magic");
			Configuration["nest-key-3"].Should().Be("kind a magic");
			Configuration["nest-key-4"].Should().Be("magic");
		}
	}
}