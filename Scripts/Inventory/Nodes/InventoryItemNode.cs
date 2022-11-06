using Godot;
using Grate.Types;
using System.Collections.Generic;
using System.Linq;

namespace Grate.Inventory
{
    public class InventoryItemNode : Control
    {
        public int Id { get; private set; }
        private bool _isPicked = false;
        private Dictionary<Vector2Int, InventoryItemModuleNode> _modulesByLayout = new Dictionary<Vector2Int, InventoryItemModuleNode>();

        private Vector2? _pickOffset;
        private Grid _grid;

        public InventoryItemNode(IInventoryItem item, Grid grid)
        {
            _grid = grid;
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

        public override bool HasPoint(Vector2 point)
        {
            return _modulesByLayout.Any(pair => pair.Value.HasPoint(point));
        }

        public override void _Input(InputEvent @event)
        {
            var e = MakeInputLocal(@event);

            if (e is InputEventMouseMotion m && _pickOffset is Vector2 offset)
                RectPosition += m.Position - offset;
        }

        public void Pick(Vector2Int pickOffset)
        {
            _pickOffset = _grid.CellSize * (pickOffset.ToVector2() + Vector2.One / 2);

            this.Modulate = Color.ColorN("white", 0.2f);
        }

        public void Put(Vector2Int gridPos)
        {
            _pickOffset = null;
            RectPosition = _grid.LeftTopPointOfCell(gridPos);
            this.Modulate = Color.ColorN("white", 1);
        }
    }
}

