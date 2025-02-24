using KeyInject.Configuration.Models;
using KeyInject.Injection;
using Microsoft.Extensions.Logging;

namespace KeyInject;

public sealed class KeyInjectConfigurationProvider : ConfigurationProvider
{
	private readonly KeyInjectConfiguration _configuration;
	private readonly IConfigurationBuilder _builder;
	private readonly ILoggerFactory? _loggerFactory;

	public KeyInjectConfigurationProvider(
		KeyInjectConfiguration configuration, 
		IConfigurationBuilder builder, 
		ILoggerFactory? loggerFactory = null)
	{
		_configuration = configuration;
		_builder = builder;
		_loggerFactory = loggerFactory;
	}
	
	/// <summary>
	/// Main entry for Key Injection
	/// </summary>
	public override void Load()
	{
		InjectionProcessor.Create(_configuration, _loggerFactory).Process(_builder);
	}
}