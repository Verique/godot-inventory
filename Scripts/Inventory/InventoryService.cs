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

                    if (!TryPutPickedItem(putPos, _pickedItem))
                    {
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
            _inventoryNode.PickItem(replacementItem.Item.Id);
            _pickedItem = new PickedItemInfo(replacementItem, _inventoryNode.CellSize);
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
            var itemWithOffset = _inventory.TryPopByPosition(gridPos);

            if (itemWithOffset == null) return;

            _inventoryNode.PickItem(itemWithOffset.Item.Id);
            _pickedItem = new PickedItemInfo(itemWithOffset, _inventoryNode.CellSize);
        }

        private void DeletePickedItem(PickedItemInfo pickedItem)
        {
            _inventoryNode.DeleteItem(pickedItem.Item.Id);
            _pickedItem = null;
        }

        private class PickedItemInfo : ItemWithOffset
        {
            public Vector2 PixelOffset { get; private set; }

            public PickedItemInfo(ItemWithOffset itemWithOffset, int cellSize) : base(itemWithOffset.Item, itemWithOffset.GridOffset)
            {
                PixelOffset = (GridOffset.ToVector2() + (Vector2.One / 2)) * cellSize;
            }
        }
    }
}

