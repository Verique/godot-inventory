using System;
using System.Collections.Generic;
using Godot;
using Grate.Input;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryNode : Control
    {
        [Export] public Vector2 GridSize = new Vector2(10, 5);
        [Export] public int CellSize = 80;

        public event Action<Vector2Int?>? LeftMouseButtonUp;
        public event Action<Vector2>? MouseMoved;

        public Grid Grid { get; private set; }

        private Vector2? _cursorPos;
        private Dictionary<int, InventoryItemNode> _itemViewById;

        public InventoryNode()
        {
            Grid = new Grid(GridSize.ToVector2Int(), CellSize);
            _itemViewById = new Dictionary<int, InventoryItemNode>();
        }

        public override void _Draw()
        {
            for (int x = 0; x <= Grid.GridSize.x; x++)
            {
                var startPoint = new Vector2(x * Grid.CellSize, 0);
                DrawLine(startPoint, startPoint + new Vector2(0, Grid.GridSize.y * Grid.CellSize), Color.ColorN("blue", 1));
            }
            for (int y = 0; y <= Grid.GridSize.y; y++)
            {
                var startPoint = new Vector2(0, y * Grid.CellSize);
                DrawLine(startPoint, startPoint + new Vector2(Grid.GridSize.x * Grid.CellSize, 0), Color.ColorN("blue", 1));
            }
            if (_cursorPos is Vector2 validPos) DrawCircle(validPos, Grid.CellSize / 2, Color.ColorN("white", 0.3f));
        }

        public override void _Input(InputEvent @event)
        {
            var e = MakeInputLocal(@event);
            switch (e)
            {
                case InputEventMouseMotion mm:
                    MouseMoved?.Invoke(mm.Position);
                    if (Grid.HasPoint(mm.Position))
                        _cursorPos = Grid.CenterPointOfCell(Grid.LocalToGrid(mm.Position));
                    else
                        _cursorPos = null;
                    Update();
                    return;
                case InputEventMouseButton mb:
                    if (mb.IsLeftMouseUp())
                    {
                        var gridPos = (Grid.HasPoint(mb.Position)) ? Grid.LocalToGrid(mb.Position) : null;
                        LeftMouseButtonUp?.Invoke(gridPos);
                    }
                    break;
                default:
                    return;
            }
        }

        public void CreateItem(InventoryItem item, Vector2Int pos)
        {
            var node = new InventoryItemNode(item, Grid, pos);
            AddChild(node);
            _itemViewById.Add(item.Id, node);
        }

        public void DeleteItem(int id)
        {
            var item = GetItemNodeById(id);
            item.QueueFree();
            _itemViewById.Remove(id);
        }

        public void PutItem(int id, Vector2Int gridPos)
        {
            GetItemNodeById(id).Put(Grid.LeftTopPointOfCell(gridPos));
        }

        public void PickItem(int id)
        {
            GetItemNodeById(id).Pick();
        }

        public void MoveItem(int id, Vector2 newPos)
        {
            GetItemNodeById(id).RectPosition = newPos;
        }

        private InventoryItemNode GetItemNodeById(int id)
        {
            if (!_itemViewById.TryGetValue(id, out var result)) throw new Exception($"No item with id {id}");
            return result;
        }
    }
}
