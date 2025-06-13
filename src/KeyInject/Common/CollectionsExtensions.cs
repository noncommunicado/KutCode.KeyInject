using System.Collections.Generic;
using System.Linq;

namespace KeyInject.Common
{
	public static class CollectionsExtensions
	{
		public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable) {
			return enumerable is null || !enumerable.Any();
		}

		public static bool NotEmptyOrNull<T>(this IEnumerable<T>? enumerable)
			=> !IsNullOrEmpty(enumerable);
	}
}