using System;
using KeyInject.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KeyInject
{
    public sealed class KeyInjectConfigurationSource : IConfigurationSource
    {
        private readonly KeyInjectConfiguration _configuration;
        private readonly ILoggerFactory? _loggerFactory;

        public KeyInjectConfigurationSource(KeyInjectConfiguration configuration, ILoggerFactory? loggerFactory = null)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (builder.GetType().IsAssignableTo(typeof(IConfigurationManager)) is false)
                throw new TypeAccessException("Builder must be ConfigurationManager");
            return new KeyInjectConfigurationProvider(
                _configuration, 
                (builder as IConfigurationManager)!,
                _loggerFactory
            );
        }
    }
}