using System;
using KeyInject.Configuration.Models;

namespace KeyInject.Common
{
	internal sealed class InjectStringCompareConfiguration
	{
		private readonly KeyInjectConfiguration _injectConfig;
		public InjectStringCompareConfiguration(KeyInjectConfiguration injectConfig)
		{
			_injectConfig = injectConfig;
		}

		public StringComparison StringComparison => _injectConfig.IgnoreCase
			? StringComparison.InvariantCultureIgnoreCase
			: StringComparison.Ordinal;
	
		public StringComparer StringComparer => _injectConfig.IgnoreCase
			? StringComparer.InvariantCultureIgnoreCase
			: StringComparer.Ordinal;
	
		public bool Compare(string left, string right)
		{
			if (string.IsNullOrEmpty(left) && string.IsNullOrEmpty(right)) return true;
			return left.Equals(right, StringComparison);
		}
	}
}