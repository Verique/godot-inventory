using System.Collections.Generic;
using System.Linq;
using Godot;
using Grate.Inventory.Utils;
using Grate.Types;

namespace Grate.Inventory
{
    public interface IInventoryItemInfo
    {
        IReadOnlyCollection<Vector2Int> Layout { get; }
        Color Color { get; }
        Vector2Int GridPos { get; }
        Vector2Int PickOffset { get; }
        bool IsPicked { get; }
    }

    public class InventoryItem : IInventoryItemInfo
    {
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

            Layout = InventoryUtils.GenerateLayout(3);
        }

        public IReadOnlyCollection<Vector2Int> GetModuleCoordinates()
        {
            return Layout.Select(x => x + GridPos).ToList();
        }

        public void Pick(Vector2Int pickPos)
        {
            PickOffset = GridPos - pickPos;
        }

        public void Put(Vector2Int putPos)
        {
            GridPos = putPos + PickOffset;
            PickOffset = null;
        }

        public void MoveTo(Vector2Int newPos)
        {
            GridPos = newPos;
        }
    }
}
