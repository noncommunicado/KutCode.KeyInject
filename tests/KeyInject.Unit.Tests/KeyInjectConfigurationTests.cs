using FluentAssertions;
using KeyInject.Configuration.Models;

namespace KeyInject.Unit.Tests;

[TestFixture]
public class KeyInjectConfigurationTests
{
	[TestCase(-1)]
	[TestCase(1)]
	[TestCase(0)]
	[TestCase(-10000)]
	[TestCase(int.MaxValue)]
	[TestCase(int.MinValue)]
	public void RepeatCount_AlwaysMoreThanZero_Test(int input)
	{
		new KeyInjectConfiguration() {
				ReplaceRepeatCount = input
			}.ReplaceRepeatCount
			.Should()
			.BeGreaterThan(0);
	}
}