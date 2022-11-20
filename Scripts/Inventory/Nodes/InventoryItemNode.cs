using Godot;
using Grate.Types;
using System.Collections.Generic;

namespace Grate.Inventory
{
    public class InventoryItemNode : Control
    {
        public int Id { get; private set; }
        private Dictionary<Vector2Int, InventoryItemModuleNode> _modulesByLayout = new Dictionary<Vector2Int, InventoryItemModuleNode>();

        public InventoryItemNode(InventoryItem item, Grid grid, Vector2Int position)
        {
            Id = item.Id;

            foreach (var module in item.Layout)
            {
                var pos = (module.Offset) * grid.CellSize;
                var moduleNode = new InventoryItemModuleNode(item.Color, pos.ToVector2(), module);
                this.AddChild(moduleNode);
                _modulesByLayout.Add(module.Offset + position, moduleNode);
            }
            RectPosition = grid.LeftTopPointOfCell(position);
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

