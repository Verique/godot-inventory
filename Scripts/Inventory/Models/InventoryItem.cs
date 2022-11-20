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
    }

    public class InventoryItem : Reference, IInventoryItem
    {
        public int Id => GetHashCode();
        public Color Color { get; private set; }

        public IReadOnlyCollection<(InventoryModule module, Vector2Int offset)> Layout { get; set; } = new List<(InventoryModule, Vector2Int)>();
        // Position is kinda strange since everyone can set it
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
