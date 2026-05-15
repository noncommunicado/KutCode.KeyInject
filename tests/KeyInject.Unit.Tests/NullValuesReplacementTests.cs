using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace KeyInject.Unit.Tests;

[TestFixture]
public sealed class NullValuesReplacementTests
{
    private const string JsonMissingKey = """
        {
            "section_1": {
                "CHILD_1": "${PLACEHOLDER}"
            }
        }
        """;

    private const string JsonWithResolvedKey = """
        {
            "PLACEHOLDER": "resolved",
            "section_1": {
                "CHILD_1": "${PLACEHOLDER}"
            }
        }
        """;

    private const string JsonMixedPlaceholders = """
        {
            "A": "first",
            "section_1": {
                "CHILD_1": "${A}|${B}"
            }
        }
        """;

    [Test]
    public void Default_MissingKey_ReplacesWithEmptyString()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonMissingKey)))
            .AddKeyInject(x => x)
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be(string.Empty);
    }

    [Test]
    public void ReplaceNullTrue_MissingKey_ReplacesWithEmptyString()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonMissingKey)))
            .AddKeyInject(x => x.SetReplaceNullValue(true))
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be(string.Empty);
    }

    [Test]
    public void ReplaceNullFalse_MissingKey_PreservesPlaceholder()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonMissingKey)))
            .AddKeyInject(x => x.SetReplaceNullValue(false))
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be("${PLACEHOLDER}");
    }

    [Test]
    public void ReplaceNullTrue_PresentKey_ReplacesWithConfiguredValue()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonWithResolvedKey)))
            .AddKeyInject(x => x.SetReplaceNullValue(true))
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be("resolved");
    }

    [Test]
    public void ReplaceNullFalse_PresentKey_ReplacesWithConfiguredValue()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonWithResolvedKey)))
            .AddKeyInject(x => x.SetReplaceNullValue(false))
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be("resolved");
    }

    [Test]
    public void ReplaceNullFalse_MixedPresentAndMissing_ReplacesOnlyExistingKeys()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonMixedPlaceholders)))
            .AddKeyInject(x => x.SetReplaceNullValue(false))
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be("first|${B}");
    }

    [Test]
    public void ReplaceNullTrue_MixedPresentAndMissing_ReplacesAllPlaceholders()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonMixedPlaceholders)))
            .AddKeyInject(x => x.SetReplaceNullValue(true))
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be("first|");
    }

    [Test]
    public void Builder_LastSetReplaceNullValue_Wins()
    {
        var resultConfiguration = new ConfigurationManager()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonMissingKey)))
            .AddKeyInject(x => x.SetReplaceNullValue(true).SetReplaceNullValue(false))
            .Build();

        resultConfiguration.GetValue<string>("section_1:CHILD_1").Should().Be("${PLACEHOLDER}");
    }
}
