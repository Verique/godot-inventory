using Godot;
using Grate.Types;
using System.Collections.Generic;

namespace Grate.Inventory
{
    public class InventoryItemNode : Control
    {
        public int Id { get; private set; }
        private Dictionary<Vector2Int, InventoryItemModuleNode> _modulesByLayout = new Dictionary<Vector2Int, InventoryItemModuleNode>();

        public InventoryItemNode(IInventoryItem item, Grid grid)
        {
            Id = item.Id;
            if (item.Position == null) return;

            foreach (var (module, offset) in item.Layout)
            {
                var pos = (offset) * grid.CellSize;
                var moduleNode = new InventoryItemModuleNode(item.Color, pos.ToVector2(), module);
                this.AddChild(moduleNode);
                _modulesByLayout.Add(offset + item.Position, moduleNode);
            }
            RectPosition = grid.LeftTopPointOfCell(item.Position);
        }

        public void Pick()
        {
            // TODO: REMOVE _GRID & OTHER THINGS, MAKE NODES STUPID
            this.Modulate = Color.ColorN("white", 0.2f);
        }

        public void Put(Vector2 newPos)
        {
            RectPosition = newPos;
            this.Modulate = Color.ColorN("white", 1);
        }
    }
}

