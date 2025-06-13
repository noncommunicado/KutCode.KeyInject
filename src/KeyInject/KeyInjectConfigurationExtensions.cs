using System;
using KeyInject.Configuration.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KeyInject
{
	public static class KeyInjectConfigurationExtensions
	{
		/// <summary>
		/// Add KeyInject with default configuraiton. <br/>
		/// Default configuration includes ${_} pattern and 5 cycles.
		/// </summary>
		public static IConfigurationBuilder AddKeyInject(
			this IConfigurationBuilder manager,
			ILoggerFactory? loggerFactory = null)
		{
			return AddKeyInject(manager, x => x, loggerFactory);
		}
	
		/// <summary>
		/// Add KeyInject with configuraiton builder. <br/>
		/// Default configuration includes ${_} pattern and 5 cycles.
		/// </summary>
		public static IConfigurationBuilder AddKeyInject(
			this IConfigurationBuilder manager,
			Func<IKeyInjectConfigurationBuilder, IKeyInjectConfigurationBuilder> builder,
			ILoggerFactory? loggerFactory = null)
		{
			var configuration = manager.Build();
			var injectBuilder = new KeyInjectConfigurationBuilder(configuration);
			return manager.Add(new KeyInjectConfigurationSource(
					builder.Invoke(injectBuilder).Build(),
					loggerFactory
				)
			);
		}
	}
}