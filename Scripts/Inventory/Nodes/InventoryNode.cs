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

        public event Action<Vector2Int?, Vector2Int>? LeftMouseButtonUp;

        private Grid Grid { get; set; }

        private Vector2? _cursorPos;
        private Vector2Int _pickOffset;
        private Dictionary<int, InventoryItemNode> _itemViewById;

        public InventoryNode()
        {
            Grid = new Grid(GridSize.ToVector2Int(), CellSize);
            _itemViewById = new Dictionary<int, InventoryItemNode>();
            _pickOffset = Vector2Int.Zero;
        }

        public void Initialize(IInventoryService service)
        {
            service.ItemAdded += CreateItemNode;
            service.ItemDeleted += DeleteItemNode;
            service.ItemPicked += PickItem;
            service.ItemPut += PutItem;
        }

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore;
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
            var _e = MakeInputLocal(@event);
            switch (_e)
            {
                case InputEventMouseMotion e:
                    if (Grid.HasPoint(e.Position))
                        _cursorPos = Grid.CenterPointOfCell(Grid.LocalToGrid(e.Position));
                    else
                        _cursorPos = null;
                    Update();
                    return;
                case InputEventMouseButton mb:
                    if (mb.IsLeftMouseUp())
                    {
                        var gridPos = (Grid.HasPoint(mb.Position)) ? Grid.LocalToGrid(mb.Position) : null;
                        LeftMouseButtonUp?.Invoke(gridPos, _pickOffset);
                    }
                    break;
                default:
                    return;
            }
        }

        private void CreateItemNode(IInventoryItem item)
        {
            var node = new InventoryItemNode(item, Grid);
            AddChild(node);
            _itemViewById.Add(item.Id, node);
        }

        private void DeleteItemNode(int id)
        {
            var item = GetItemNodeById(id);
            item.QueueFree();
            _itemViewById.Remove(id);
        }

        private void PutItem(int id, Vector2Int gridPos)
        {
            var item = GetItemNodeById(id);
            item.Put(gridPos);
            _pickOffset = Vector2Int.Zero;
        }

        private void PickItem(int id, Vector2Int pickOffset)
        {
            var item = GetItemNodeById(id);
            item.Pick(pickOffset);
            _pickOffset = pickOffset;
        }

        private InventoryItemNode GetItemNodeById(int id)
        {
            if (!_itemViewById.ContainsKey(id)) throw new Exception($"No item with id {id}");
            return _itemViewById[id];
        }
    }
}
