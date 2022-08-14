using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Grate.Inventory.Utils;
using Grate.Types;

namespace Grate.Inventory
{
    public interface IInventoryItem
    {
        event Action<Vector2Int>? ItemPicked;
        event Action<Vector2Int>? ItemPut;
        event Action? ItemDeleting;

        int Id { get; }
        IReadOnlyCollection<InventoryModule> Layout { get; }
        Color Color { get; }
        Vector2Int GridPos { get; }
        Vector2Int? PickOffset { get; }
        bool IsPicked { get; }
    }

    public class InventoryItem : Reference, IInventoryItem
    {
        public event Action<Vector2Int>? ItemPicked;
        public event Action<Vector2Int>? ItemPut;
        public event Action? ItemDeleting;

        public int Id => GetHashCode();
        public IReadOnlyCollection<InventoryModule> Layout { get; }
        public Color Color { get; private set; }
        public Vector2Int GridPos { get; private set; }
        public Vector2Int? PickOffset { get; private set; }
        public bool IsPicked => PickOffset != null;

        public InventoryItem(Vector2Int gridPos)
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            Color = Color.Color8((byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255));

            Layout = InventoryUtils.GenerateItemLayout(rng.RandiRange(2, 5));
            GridPos = gridPos;
        }

        public IReadOnlyCollection<Vector2Int> GetModuleCoordinates()
        {
            return Layout.Select(x => x.LayoutPos + GridPos).ToList();
        }

        public void Pick(Vector2Int pickPos)
        {
            PickOffset = GridPos - pickPos;
            ItemPicked?.Invoke(PickOffset);
        }

        public void Put(Vector2Int putPos)
        {
            if (PickOffset is null) throw new Exception($"Item {Id} is not picked");

            GridPos = putPos + PickOffset;
            PickOffset = null;
            ItemPut?.Invoke(GridPos);
        }

        public void InvokeItemDeletion()
        {
            ItemDeleting?.Invoke();
        }

        public void MoveTo(Vector2Int newPos)
        {
            GridPos = newPos;
        }
    }

    public class InventoryModule
    {
        public Vector2Int LayoutPos { get; private set; }
        public bool Right { get; private set; }
        public bool Left { get; private set; }
        public bool Up { get; private set; }
        public bool Down { get; private set; }

        public InventoryModule(Vector2Int pos, IEnumerable<Vector2Int> freeNeighbourCells)
        {
            LayoutPos = pos;
            // ups and downs are shuffled since inventory is top to bottom
            if (freeNeighbourCells.Contains(Vector2Int.Up + LayoutPos)) Down = true;
            if (freeNeighbourCells.Contains(Vector2Int.Down + LayoutPos)) Up = true;
            if (freeNeighbourCells.Contains(Vector2Int.Left + LayoutPos)) Left = true;
            if (freeNeighbourCells.Contains(Vector2Int.Right + LayoutPos)) Right = true;
        }
    }
}
