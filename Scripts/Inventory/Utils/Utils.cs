using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grate.Inventory
{
    public static class Utils
    {
        public static List<Vector2I> GenerateLayout(int count)
        {
            if (count < 1) throw new ArgumentException($"Invalid item size: {count}");

            var rng = new Random();
            List<Vector2I> layout = new List<Vector2I>(new[] { Vector2I.Zero });
            HashSet<Vector2I> vacantCells = new HashSet<Vector2I>(GetNeighbours(Vector2I.Zero));

            for (int i = 0; i < count - 1; i++)
            {
                var rngIndex = rng.Next() % vacantCells.Count();
                var place = vacantCells.ElementAt(rngIndex);
                vacantCells.Remove(place);
                layout.Add(place);
                vacantCells.UnionWith(GetNeighbours(place).Where(x => !layout.Contains(x)));
            }

            return NormalizeLayout(layout);
        }

        private static List<Vector2I> NormalizeLayout(List<Vector2I> layout)
        {
            var middle = layout.OrderBy(m => GetModuleAverageDiff(m, layout)).ThenBy(m => m.X + m.Y).First();

            return layout.Select(cell => cell - middle).ToList();
        }

        private static double GetModuleAverageDiff(Vector2I module, List<Vector2I> layout)
        {
            return layout.Select(v2 => v2 - module).Select(v2 => Math.Abs(v2.X) + Math.Abs(v2.Y)).Average();
        }

        private static Vector2I[] GetNeighbours(Vector2I start)
        {
            return Enumerable
                .Range(-1, 3)
                .SelectMany(a => Enumerable.Range(-1, 3).Select(b => new Vector2I(b, a)))
                .Where(v => Math.Abs(v.X) != Math.Abs(v.Y))
                .Select(v => v + start)
                .ToArray();
        }
    }
}
