using System;
using System.Collections.Generic;
using System.Linq;
using Grate.Types;

namespace Grate.Inventory.Utils
{
    public static class InventoryUtils
    {
        public static List<Vector2Int> GenerateLayout(int count)
        {
            if (count < 1) throw new ArgumentException($"Invalid item size: {count}");

            var rng = new Random();
            List<Vector2Int> layout = new List<Vector2Int>(new[] {Vector2Int.Zero});
            HashSet<Vector2Int> vacantCells = new HashSet<Vector2Int>(GetNeighbours(Vector2Int.Zero));

            for (int i = 0; i < count - 1; i++)
            {
                var rngIndex = rng.Next() % vacantCells.Count();
                var place = vacantCells.ElementAt(rngIndex);
                vacantCells.Remove(place);
                layout.Add(place);
                vacantCells.UnionWith(GetNeighbours(place).Where(x => !layout.Contains(x)));
            }

            return layout;
        }

        private static Vector2Int[] GetNeighbours(Vector2Int start)
        {
            return Enumerable
                .Range(-1, 3)
                .SelectMany(a => Enumerable.Range(-1, 3).Select(b => new Vector2Int(b, a)))
                .Where(v => Math.Abs(v.x) != Math.Abs(v.y))
                .Select(v => v + start)
                .ToArray();
        }
    }
}
