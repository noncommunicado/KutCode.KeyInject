using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KeyInject.Common;
using KeyInject.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KeyInject.Injection
{
    internal sealed class InjectionProcessor
    {
        private readonly ILogger? _logger;
        private readonly KeyInjectConfiguration _injectConfig;
        private readonly ConfigurationProvider _provider;
        private readonly InjectStringCompareConfiguration _stringCompareConfig;

        public InjectionProcessor(
            KeyInjectConfiguration injectConfig,
            ConfigurationProvider provider,
            ILogger? logger = null) 
        {
            _injectConfig = injectConfig;
            _provider = provider;
            _stringCompareConfig = new InjectStringCompareConfiguration(injectConfig);
            _logger = logger;
        }

        /// <summary>
        /// Process key injection, main part of this library.
        /// </summary>
        public void Process(IConfigurationRoot root)
        {
            if (_injectConfig.Enabled is false) {
                _logger?.LogInformation("KeyInject is disabled globally, check configuration if this behaviour is incorrect");
                return;
            }

            _logger?.LogInformation("KeyInject starting process configuration");
            _logger?.LogInformation("KeyInject configuration: {0}", _injectConfig);

            _logger?.LogInformation("Found configuration providers:{0}{1}", Environment.NewLine,
                string.Join(Environment.NewLine, root.Providers
                    .Select(x => "\t" + x)
                    .Distinct())
            );

            Handle(root);
        }

        private void Handle(IConfigurationRoot root)
        {
            var configuration = FlattenConfiguration(root);
            var counter = 0;
            
            while (counter < _injectConfig.ReplaceRepeatCount) {
                foreach (var configPair in configuration)
                foreach (var regex in _injectConfig.RegexPatterns) {
                    int replaceCount = HandleRegex(regex, configPair, configuration);
                    if (replaceCount > 0)
                        _logger?.LogInformation("For key [{0}] made [{1}] replacements ", configPair.Key, replaceCount);
                }
                counter++;
            }
        }

        /// <summary>
        /// Find all parent and children (recursively) config sections with value.  
        /// Section without value ignored (usually, it's parent sections).  
        /// </summary>
        internal Dictionary<string, string> FlattenConfiguration(IConfigurationRoot root)
        {
            Dictionary<string, string> providersData = new(_stringCompareConfig.StringComparer);
            
            foreach (var configProvider in root.Providers.Where(x => x is not KeyInjectConfigurationProvider))
            {
                FlattenConfigurationProviderIterative(providersData, configProvider);
            }

            return providersData;
        }

        private void FlattenConfigurationProviderIterative(
            Dictionary<string, string> providersData,
            IConfigurationProvider provider)
        {
            var keysToProcess = new Queue<string>();
            var processedKeys = new HashSet<string>(_stringCompareConfig.StringComparer);
            
            // add root level keys
            foreach (var key in provider.GetChildKeys(new List<string> { }.AsReadOnly(), null))
            {
                if (!string.IsNullOrEmpty(key) && processedKeys.Add(key))
                {
                    keysToProcess.Enqueue(key);
                }
            }
            
            // iterative processing instead of recursive
            while (keysToProcess.Count > 0)
            {
                var currentKey = keysToProcess.Dequeue();
                
                // check if current key has a value
                if (provider.TryGet(currentKey, out var value) && !string.IsNullOrEmpty(value))
                {
                    providersData[currentKey] = value;
                }
                
                // add child keys to queue
                var childKeys = provider.GetChildKeys(new List<string> { }.AsReadOnly(), currentKey);
                foreach (var childKey in childKeys)
                {
                    var fullKey = $"{currentKey}:{childKey}";
                    if (processedKeys.Add(fullKey))
                    {
                        keysToProcess.Enqueue(fullKey);
                    }
                }
            }
        }

        /// <summary>
        /// Replace patterns in key's value
        /// </summary>
        /// <returns>How many replacements was in key's value</returns>
        private int HandleRegex(
            Regex regex,
            KeyValuePair<string, string> configPair,
            Dictionary<string, string> configuration)
        {
            if (string.IsNullOrEmpty(configPair.Value)) return 0;
            string value = configPair.Value;
            int replacementsCounter = 0;

            foreach (Match match in regex.Matches(value)) {
                var presentedInMatch = match.Groups
                    .TryGetValue(InjectDefaults.RegexInjectionGroupKey, out var keyGroup);
                
                if (presentedInMatch is false) continue;
                
                // cleared key: "${Some_Key}" --> "Some_Key"
                var key = _injectConfig.IgnoreCase
                    ? keyGroup!.Value.ToLowerInvariant()
                    : keyGroup!.Value;
                
                // check for prefix satisfy if presented
                var prefixFilter = _injectConfig.KeyPrefixes.IsNullOrEmpty() ||
                                   _injectConfig.KeyPrefixes.Any(x
                                       => key.StartsWith(x, _stringCompareConfig.StringComparison)
                                   );
                
                if (prefixFilter is false) continue;
                // finally, string replacement
                if (!configuration.TryGetValue(key, out var sourceValue)) continue;
                if (string.IsNullOrEmpty(sourceValue)) continue;
                value = value.Replace(match.Value, sourceValue);
                replacementsCounter++;
            }

            if (replacementsCounter <= 0) return replacementsCounter;

            _provider.Set(configPair.Key, value);
            configuration[configPair.Key] = value;
            return replacementsCounter;
        }
    }
}
