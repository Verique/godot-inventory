using System.Collections.Generic;
using System.Linq;
using Godot;
using Grate.Inventory.Utils;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryItem
    {
        public IReadOnlyCollection<Vector2Int> Layout { get; }
        public string Message { get; }
        public Color color;

        public Vector2Int MainPos { get; set; }
        public Vector2Int PickPos { get; set; }

        public InventoryItem(string message, Vector2Int mainPos = null)
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            color = Color.Color8((byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255));

            Layout = InventoryUtils.GenerateLayout(3);
            Message = message;
            MainPos = mainPos;
        }

        public IReadOnlyCollection<Vector2Int> GetModuleCoordinates()
        {
            return Layout.Select(x => x + MainPos).ToList();
        }

    }

    public class InventoryModule
    {
        public InventoryItem Item { get; private set; }
        public InventoryModule(InventoryItem item) { Item = item; }
    }
}
