using System.Collections.Generic;
using System.Linq;

namespace IOL.Helpers;

public static class EnumerableExtension
{
	public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable != null && !enumerable.Any<T>();
}
