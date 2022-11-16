using System.Collections.Generic;

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
    }
}
