using FluentAssertions;
using KeyInject.Common;

namespace KeyInject.Unit.Tests;

[TestFixture]
public sealed class RegexParserTests
{
	[TestCase(@"\$\{([^\{\}]+)\}", true)]
	[TestCase(@"\{\{([^\{\}]+)\}\}", true)]
	[TestCase(@"<([^<>]+)>", true)]
	[TestCase(@"<<([^<>]+)>>", true)]
	[TestCase(@"!\{([^{}]+)\}!", true)]
	[TestCase("", false)]
	[TestCase("\\id=(.*?)", false)]
	public void TryParseRegex_Test(string regex, bool isSuccessExpected)
	{
		RegexParser.TryParse(regex, false, out var parsed)
			.Should().Be(isSuccessExpected);

		if (isSuccessExpected)
			parsed.Should().NotBeNull();
		else
			parsed.Should().BeNull();
	}
}