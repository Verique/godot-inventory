using Godot;
using Grate.Types;
using System.Collections.Generic;
using System.Linq;

namespace Grate.Inventory
{
    public class InventoryItemModuleNode : TextureRect
    {
        List<Vector2> DrawPoses = new List<Vector2>();

        public InventoryItemModuleNode(Color color, Vector2 position, InventoryModule module)
        {
            RectPosition = position;
            this.Texture = ResourceLoader.Load<StreamTexture>("res://sprites/item1x1.png");
            this.Modulate = color;
            this.MouseFilter = MouseFilterEnum.Pass;
            if (module.Down) DrawPoses.Add(new Vector2(40, 80));
            if (module.Up) DrawPoses.Add(new Vector2(40, 0));
            if (module.Left) DrawPoses.Add(new Vector2(0, 40));
            if (module.Right) DrawPoses.Add(new Vector2(80, 40));
            Update();
        }

        public override void _Draw()
        {
            foreach (var point in DrawPoses)
            {
                DrawCircle(point, 10, Colors.Plum);
            }
        }
    }

    public class InventoryItemNode : Control
    {
        private bool _isPicked = false;
        private Dictionary<Vector2Int, InventoryItemModuleNode> _modulesByLayout = new Dictionary<Vector2Int, InventoryItemModuleNode>();

        private Vector2? _pickOffset;
        private Grid _grid;

        public InventoryItemNode(IInventoryItem item, Grid grid)
        {
            _grid = grid;

            RectPosition = grid.LeftTopPointOfCell(item.GridPos);

            item.ItemPicked += Pick;
            item.ItemPut += Put;
            item.ItemDeleting += () => QueueFree();

            foreach (var point in item.Layout)
            {
                var pos = (point.LayoutPos) * grid.CellSize;
                var module = new InventoryItemModuleNode(item.Color, pos.ToVector2(), point);
                this.AddChild(module);
                _modulesByLayout.Add(point.LayoutPos, module);
            }
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

        private void Pick(Vector2Int pickOffset)
        {
            _pickOffset = _grid.CellSize * (Vector2.One / 2 - pickOffset.ToVector2());
            this.Modulate = Color.ColorN("white", 0.2f);
        }

        private void Put(Vector2Int gridPos)
        {
            _pickOffset = null;
            RectPosition = _grid.LeftTopPointOfCell(gridPos);
            this.Modulate = Color.ColorN("white", 1);
        }
    }
}

