using System.Collections.Generic;
using System.Linq;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryModule
    {
        public InventoryItem Item { get; private set; } = default!;
        public bool Right { get; private set; }
        public bool Left { get; private set; }
        public bool Up { get; private set; }
        public bool Down { get; private set; }

        public InventoryModule(Vector2Int pos, IEnumerable<Vector2Int> freeNeighbourCells, InventoryItem parent)
        {
            Item = parent;
            // ups and downs are shuffled since inventory is top to bottom
            if (freeNeighbourCells.Contains(Vector2Int.Up + pos)) Down = true;
            if (freeNeighbourCells.Contains(Vector2Int.Down + pos)) Up = true;
            if (freeNeighbourCells.Contains(Vector2Int.Left + pos)) Left = true;
            if (freeNeighbourCells.Contains(Vector2Int.Right + pos)) Right = true;
        }
    }
}
