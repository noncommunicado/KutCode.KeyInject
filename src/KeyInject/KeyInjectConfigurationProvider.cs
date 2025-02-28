using KeyInject.Configuration.Models;
using KeyInject.Injection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace KeyInject;

public sealed class KeyInjectConfigurationProvider : ConfigurationProvider, IDisposable
{
	private readonly object _locker = new();
	private readonly ILogger? _logger;
	private readonly KeyInjectConfiguration _configuration;
	private readonly IConfigurationBuilder _manager;
	private readonly InjectionProcessor _processor;
	private IConfigurationRoot _configurationRoot;
	private IDisposable? _changeTokenRegistration;
	private int _loadsCount;
	private bool _reloadCalled = false;

	public Guid CurrentConfigVersion { get; private set; } = Guid.NewGuid();

	public KeyInjectConfigurationProvider(
		KeyInjectConfiguration configuration,
		IConfigurationManager manager,
		ILoggerFactory? loggerFactory = null)
	{
		_manager = manager;
		_configurationRoot = manager.Build();
		_configuration = configuration;
		_logger = loggerFactory?.CreateLogger<InjectionProcessor>();
		_processor = new InjectionProcessor(configuration, this, _logger);
	}

	/// <summary>
	/// Main entry for Key Injection
	/// </summary>
	/// <exception cref="TypeAccessException">When injected IConfigurationBuilder is not IConfigurationRoot</exception>
	public override void Load()
	{
		lock (_locker) {
			_processor.Process(_configurationRoot);
			_loadsCount++;
			_logger?.LogInformation("Configuration loaded");
			_reloadCalled = true;
			OnReload();

			if (_configuration.ReloadEnabled is false) return;
			_changeTokenRegistration?.Dispose();
			_changeTokenRegistration = ChangeToken.OnChange(
				() => _configurationRoot.GetReloadToken(),
				() => Reload(_configurationRoot)
			);
		}
	}

	private void Reload(IConfigurationRoot root)
	{
		lock (_locker) {
			if (_reloadCalled) {
				_reloadCalled = false;
				return;
			}
			if (_loadsCount == 0) return;
			_logger?.LogInformation("Configuration reload starting");
			Load();
		}
	}

	public void Dispose() => _changeTokenRegistration?.Dispose();
}