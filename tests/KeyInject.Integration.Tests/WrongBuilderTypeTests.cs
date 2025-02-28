using Microsoft.Extensions.Configuration;

namespace KeyInject.Integration.Tests;

[TestFixture]
public class WrongBuilderTypeTests
{
    [Test]
    public void ConfigBuilderTypeTest()
    {
        var builder = new ConfigurationBuilder();
        try {
            builder.AddKeyInject();
            builder.Build();
        }
        catch (TypeAccessException e) {
            Assert.Pass($"Success exceptions type: {e.Message}");
        }
        catch (Exception e) {
           Assert.Fail("Wrong error type");
        }
    }
}