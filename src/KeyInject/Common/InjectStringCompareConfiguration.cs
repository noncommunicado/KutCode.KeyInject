using KeyInject.Configuration.Models;

namespace KeyInject.Common;

internal sealed class InjectStringCompareConfiguration(KeyInjectConfiguration injectConfig)
{
	public StringComparison StringComparison => injectConfig.IgnoreCase
		? StringComparison.InvariantCultureIgnoreCase
		: StringComparison.Ordinal;
	
	public StringComparer StringComparer => injectConfig.IgnoreCase
		? StringComparer.InvariantCultureIgnoreCase
		: StringComparer.Ordinal;
	
	public bool Compare(string left, string right)
	{
		if (string.IsNullOrEmpty(left) && string.IsNullOrEmpty(right)) return true;
		return left.Equals(right, StringComparison);
	}
}