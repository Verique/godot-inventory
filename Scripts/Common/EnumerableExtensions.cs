using System;
using System.Collections.Generic;
using System.Linq;

namespace Grate.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T1> DistinctBy<T1, T2>(this IEnumerable<T1> that, Func<T1, T2> selector) 
        {
            return that.GroupBy(selector).Select(group => group.First());
        }

        public static IEnumerable<T> FilterOutNulls<T>(this IEnumerable<T?> that) where T : class
        {
            return that
                .Where(x => x != null)
                .Select(x => x!);
        }

        public static IEnumerable<T> FilterOutNulls<T>(this IEnumerable<T?> that) where T : struct
        {
            return that
                .Where(x => x != null)
                .Select(x => x!)
                .Cast<T>();
        }

        public static IEnumerable<T1> FilterOutNullsBy<T1, T2>(this IEnumerable<T1> that, Func<T1, T2?> nullableValueSelector) where T2 : class
        {
            return that
                .Where(x => nullableValueSelector(x) != null);
        }
    }
}
