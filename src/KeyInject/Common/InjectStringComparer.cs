using KeyInject.Configuration.Models;

namespace KeyInject.Common;

internal sealed class InjectStringComparer(KeyInjectConfiguration injectConfig)
{
	public StringComparison StringComparison => injectConfig.IgnoreCase
		? StringComparison.InvariantCultureIgnoreCase
		: default;
	
	public bool Compare(string left, string right)
	{
		if (string.IsNullOrEmpty(left) && string.IsNullOrEmpty(right)) return true;
		return left.Equals(right, StringComparison);
	}
}