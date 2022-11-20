using System;
using Godot;
using Grate.Services;
using Grate.Types;

namespace Grate.Inventory
{
    public interface IInventoryService : IService { }

    public class InventoryService : Reference, IInventoryService
    {
        private Inventory _inventory;
        private InventoryNode _inventoryNode;

        private PickedItemInfo? _pickedItem { get; set; }
        private Vector2Int _gridSize;

        public InventoryService(CanvasLayer canvasLayer)
        {
            var testButton = canvasLayer.GetNode<Button>("Button");
            _inventoryNode = canvasLayer.GetNode<InventoryNode>("Inventory");

            _gridSize = _inventoryNode.GridSize.ToVector2Int();
            _inventory = new Inventory(_gridSize);

            testButton.Connect("button_up", this, nameof(Add));
            _inventoryNode.LeftMouseButtonUp += OnLeftMouseButtonUp;
            _inventoryNode.MouseMoved += OnMouseMoved;
        }

        private void OnMouseMoved(Vector2 mousePos)
        {
            if (_pickedItem != null)
            {
                _inventoryNode.MoveItem(_pickedItem.Item.Id, mousePos - _pickedItem.PixelOffset);
            }
        }

        private void OnLeftMouseButtonUp(Vector2Int? gridPos)
        {
            if (_pickedItem != null)
            {
                if (gridPos == null)
                    DeletePickedItem(_pickedItem);
                else
                {
                    var putPos = gridPos - _pickedItem.GridOffset;

                    if (!TryPutPickedItem(putPos, _pickedItem)) {
                        ReplacePickedItem(putPos, _pickedItem);
                    }
                }
            }
            else if (gridPos != null)
                PickItem(gridPos);
        }

        private void ReplacePickedItem(Vector2Int putPos, PickedItemInfo pickedItem)
        {
            var replacementItem = _inventory.TryReplace(pickedItem.Item, putPos);
            if (replacementItem == null) return;

            _inventoryNode.PutItem(pickedItem.Item.Id, putPos);
            _inventoryNode.PickItem(replacementItem.Id);
            _pickedItem = BuildPickedItem(replacementItem);
        }

        private bool TryPutPickedItem(Vector2Int putPos, PickedItemInfo pickedItem)
        {
            var result = _inventory.TryPlace(pickedItem.Item, putPos);
            if (!result) return false;

            _inventoryNode.PutItem(pickedItem.Item.Id, putPos);
            _pickedItem = null;

            return true;
        }

        private void Add()
        {
            var item = new InventoryItem();

            for (int x = 0; x < _gridSize.x; x++)
                for (int y = 0; y < _gridSize.y; y++)
                {
                    if (_inventory.TryPlace(item, new Vector2Int(x, y)))
                    {
                        _inventoryNode.CreateItem(item);
                        return;
                    }
                }
        }

        private void PickItem(Vector2Int gridPos)
        {
            var result = _inventory.TryPopByPosition(gridPos);

            if (result == null) return;

            var (item, lastPos) = result.Value;

            _inventoryNode.PickItem(item.Id);
            _pickedItem = BuildPickedItem(item, gridPos - lastPos);
        }

        private void DeletePickedItem(PickedItemInfo pickedItem)
        {
            _inventoryNode.DeleteItem(pickedItem.Item.Id);
            _pickedItem = null;
        }

        private PickedItemInfo BuildPickedItem(InventoryItem item, Vector2Int? gridOffset = null) =>
            new PickedItemInfo(item, gridOffset ?? Vector2Int.Zero, _inventoryNode.CellSize);

        private class PickedItemInfo
        {
            public InventoryItem Item { get; private set; }
            public Vector2Int GridOffset { get; private set; }
            public Vector2 PixelOffset { get; private set; }

            public PickedItemInfo(InventoryItem item, Vector2Int gridOffset, int cellSize)
            {
                if (item.Position != null) throw new Exception("Item isn't picked. Its position is set.");
                Item = item;
                GridOffset = gridOffset;
                PixelOffset = (gridOffset.ToVector2() + (Vector2.One / 2)) * cellSize;
            }
        }
    }
}

