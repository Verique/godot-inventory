using System;
using System.Collections.Generic;
using Godot;
using Grate.Inventory.Utils;
using Grate.Types;

namespace Grate.Inventory
{
    public interface IInventoryItem
    {
        int Id { get; }
        IReadOnlyCollection<(InventoryModule module, Vector2Int offset)> Layout { get; }
        Color Color { get; }
        Vector2Int? Position { get; }
        bool IsPicked { get; }
    }

    public class InventoryItem : Reference, IInventoryItem
    {
        public event Action<Vector2Int>? ItemPicked;
        public event Action<Vector2Int>? ItemPut;
        public event Action? ItemDeleting;

        public int Id => GetHashCode();
        public Color Color { get; private set; }
        public bool IsPicked => Position != null;

        public IReadOnlyCollection<(InventoryModule module, Vector2Int offset)> Layout { get; set; } = new List<(InventoryModule, Vector2Int)>();
        public Vector2Int? Position { get; set; }

        public InventoryItem()
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            Color = Color.Color8((byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255));

            Layout = InventoryUtils.GenerateItemLayout(rng.RandiRange(2, 5), this);
        }
    }
}
