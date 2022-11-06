using System.Collections.Generic;
using System.Linq;

namespace Grate.Utils
{
    public static class ArrayExtensions
    {
        public static List<T> ConvertToCSharp<T>(this Godot.Collections.Array array)
        {
            var result = new List<T>();

            foreach (var item in array)
            {
                result.Add((T)item);
            }

            return result;
        }

        public static IEnumerable<T> FilterOutNulls<T>(this IEnumerable<T?> that) where T: class
        {
            return that
                .Where(x => x != null)
                .Select(x => x!);
        }
    }
}
