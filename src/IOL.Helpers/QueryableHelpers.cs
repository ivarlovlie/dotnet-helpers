using System;
using System.Linq;
using System.Linq.Expressions;

namespace IOL.Helpers;

public static class QueryableHelpers
{
	public static IQueryable<T> ConditionalWhere<T>(
			this IQueryable<T> source,
			Func<bool> condition,
			Expression<Func<T, bool>> predicate
	) {
		return condition() ? source.Where(predicate) : source;
	}

	public static IQueryable<T> ConditionalWhere<T>(
			this IQueryable<T> source,
			bool condition,
			Expression<Func<T, bool>> predicate
	) {
		return condition ? source.Where(predicate) : source;
	}
}
