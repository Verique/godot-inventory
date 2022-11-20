using System.Collections.Generic;
using Godot;
using Grate.Inventory.Utils;

namespace Grate.Inventory
{
    public class InventoryItem : Reference
    {
        public int Id => GetHashCode();
        public Color Color { get; private set; }

        public IReadOnlyCollection<InventoryModule> Layout { get; private set; } = new List<InventoryModule>();

        public InventoryItem()
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            Color = Color.Color8((byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255));
            Layout = InventoryUtils.GenerateItemLayout(rng.RandiRange(2, 5), this);
        }
    }
}
