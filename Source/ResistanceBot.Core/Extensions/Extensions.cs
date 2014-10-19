using System;
using System.Collections.Generic;
using System.Linq;

namespace ResistanceBot.Core.Extensions
{
	public static class LinqExtensions
	{
		public static IEnumerable<TSource> Duplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			var grouped = source.GroupBy(selector);
			var moreThen1 = grouped.Where(i => i.IsMultiple());
			return moreThen1.SelectMany(i => i);
		}

		public static IEnumerable<TSource> Duplicates<TSource>(this IEnumerable<TSource> source)
		{
			return source.Duplicates(i => i);
		}

		public static bool IsMultiple<T>(this IEnumerable<T> source)
		{
			var enumerator = source.GetEnumerator();
			return enumerator.MoveNext() && enumerator.MoveNext();
		}
	}
}