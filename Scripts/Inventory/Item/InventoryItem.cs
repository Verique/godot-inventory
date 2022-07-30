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
        event Action<Vector2Int> ItemPicked;
        event Action<Vector2Int> ItemPut;
        event Action ItemDeleting;

        int Id { get; }
        IReadOnlyCollection<Vector2Int> Layout { get; }
        Color Color { get; }
        Vector2Int GridPos { get; }
        Vector2Int PickOffset { get; }
        bool IsPicked { get; }
    }

    public class InventoryItem : Reference, IInventoryItem
    {
        public event Action<Vector2Int> ItemPicked;
        public event Action<Vector2Int> ItemPut;
        public event Action ItemDeleting;

        public int Id => GetHashCode();
        public IReadOnlyCollection<Vector2Int> Layout { get; }
        public Color Color { get; private set; }
        public Vector2Int GridPos { get; private set; }
        public Vector2Int PickOffset { get; private set; }
        public bool IsPicked => PickOffset != null;

        public InventoryItem()
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            Color = Color.Color8((byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255));

            Layout = InventoryUtils.GenerateLayout(rng.RandiRange(2, 5));
        }

        public IReadOnlyCollection<Vector2Int> GetModuleCoordinates()
        {
            return Layout.Select(x => x + GridPos).ToList();
        }

        public void Pick(Vector2Int pickPos)
        {
            PickOffset = GridPos - pickPos;
            ItemPicked?.Invoke(PickOffset);
        }

        public void Put(Vector2Int putPos)
        {
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
}
