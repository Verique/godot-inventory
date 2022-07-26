using System;
using System.Collections.Generic;
using Godot;
using Grate.Input;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryNode : Control
    {
        [Export] public readonly Vector2 TopLeft = Vector2.Zero;
        [Export] public readonly Vector2 GridSize = Vector2.Zero;
        [Export] public readonly int CellSize = 0;

        public event Action<Vector2Int> ItemPicked;
        public event Action<Vector2Int> ItemPut;

        private Dictionary<int, InventoryItemNode> _items;
        private InventoryItemNode _pickedItem;
        private Vector2Int _pickOffset;

        private Vector2? _cursorPos;

        public void HandleModelChange(IReadOnlyCollection<IInventoryItemInfo> items, IInventoryItemInfo pickedItem)
        {
            _pickedItem = null;
            foreach (var item in items)
            {
                var node = FindOrCreateItemNode(item);
                node.Put(LeftTopPointOfCell(item.GridPos));
            }

            if (pickedItem != null)
            {
                var node = FindOrCreateItemNode(pickedItem);
                _pickOffset = pickedItem.PickOffset;
                node.Pick();
                _pickedItem = node;
            }
        }

        public override void _Ready()
        {
            Connect("mouse_exited", this, nameof(LoseFocus));
            _items = new Dictionary<int, InventoryItemNode>();
            RectSize = new Vector2(GridSize.x * CellSize, GridSize.y * CellSize);
            RectPosition = TopLeft;
        }

        public override void _Draw()
        {
            for (int x = 0; x <= GridSize.x; x++)
            {
                var startPoint = new Vector2(x * CellSize, 0);
                DrawLine(startPoint, startPoint + new Vector2(0, GridSize.y * CellSize), Color.ColorN("blue", 1));
            }
            for (int y = 0; y <= GridSize.y; y++)
            {
                var startPoint = new Vector2(0, y * CellSize);
                DrawLine(startPoint, startPoint + new Vector2(GridSize.x * CellSize, 0), Color.ColorN("blue", 1));
            }
            if (_cursorPos is Vector2 validPos) DrawCircle(validPos, CellSize / 2, Color.ColorN("white", 0.3f));
        }

        public override void _Process(float delta)
        {
            Update();
        }

        public override void _GuiInput(InputEvent _e)
        {
            switch (_e)
            {
                case InputEventMouseMotion e:
                    _cursorPos = CenterPointOfCell(LocalToGrid(e.Position));
                    return;
                case InputEventMouseButton mb:
                    if (mb.IsLeftMouseUp())
                        HandleLeftMouseUp(mb.Position);
                    break;
                default:
                    return;
            }
        }

        public override void _Input(InputEvent e)
        {
            if (e is InputEventMouseMotion m && _pickedItem != null)
            {
                _pickedItem.RectPosition = GetLocalMousePosition() + (_pickOffset.ToVector2() - Vector2.One / 2) * CellSize;
            }
        }

        private void HandleLeftMouseUp(Vector2 position)
        {
            if (_pickedItem == null)
            {
                ItemPicked?.Invoke(LocalToGrid(position));
            }
            else
            {
                ItemPut?.Invoke(LocalToGrid(position));
            }
        }

        private InventoryItemNode FindOrCreateItemNode(IInventoryItemInfo item)
        {
            var success = _items.TryGetValue(item.GetHashCode(), out var node);

            if (success)
            {
                return node;
            }
            else
            {
                var newNode = new InventoryItemNode(item, this, item.GridPos);
                _items.Add(item.GetHashCode(), newNode);
                AddChild(newNode);
                return newNode;
            }
        }

        private void LoseFocus() => _cursorPos = null;

        private Vector2Int LocalToGrid(Vector2 local)
        {
            var x = Mathf.FloorToInt(local.x / CellSize);
            var y = Mathf.FloorToInt(local.y / CellSize);

            return new Vector2Int(x, y);
        }

        private Vector2 CenterPointOfCell(Vector2Int cellCoordinates)
        {
            return LeftTopPointOfCell(cellCoordinates) + Vector2.One * CellSize / 2;
        }

        private Vector2 LeftTopPointOfCell(Vector2Int cellCoordinates)
        {
            return cellCoordinates.ToVector2() * CellSize;
        }
    }
}
