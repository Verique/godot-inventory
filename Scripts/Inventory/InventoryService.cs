using System;
using System.Linq;
using Godot;
using Grate.Services;
using Grate.Types;
using Grate.Utils;

namespace Grate.Inventory
{
    public interface IInventoryService : IService { }

    public class InventoryService : Reference, IInventoryService
    {
        public InventoryItem? _pickedItem { get; private set; }
        private Inventory _inventory;
        private InventoryNode _inventoryNode;
        private Vector2Int _gridSize;
        private Vector2Int _pickOffset = Vector2Int.Zero;

        public InventoryService(CanvasLayer canvasLayer)
        {
            var testButton = canvasLayer.GetNode<Button>("Button");
            _inventoryNode = canvasLayer.GetNode<InventoryNode>("Inventory");

            _gridSize = _inventoryNode.GridSize.ToVector2Int();
            _inventory = new Inventory(_gridSize);

            testButton.Connect("button_up", this, nameof(Add));
            _inventoryNode.LeftMouseButtonUp += OnLeftMouseButtonUp;
        }

        private void OnLeftMouseButtonUp(Vector2Int? gridPos)
        {
            var item = gridPos != null ? _inventory.ItemAt(gridPos) : null;
            var itemPos = item != null ? item.Position : null;

            if (_pickedItem != null)
            {
                if (gridPos == null) {
                    _inventoryNode.DeleteItem(_pickedItem.Id);
                    _pickedItem = null;
                }
                else
                {
                    Put(gridPos - _pickOffset);
                }
            }
            else
            {
                if (itemPos != null)
                {
                    Pick(itemPos);
                    _pickOffset = gridPos! - itemPos;
                    _inventoryNode.PickItem(item!.Id, _pickOffset);
                }
            }
        }

        private void Add()
        {
            var item = new InventoryItem();

            for (int x = 0; x < _gridSize.x; x++)
                for (int y = 0; y < _gridSize.y; y++)
                {
                    var newPos = new Vector2Int(x, y);
                    if (_inventory.CanPlace(item, newPos))
                    {
                        _inventory.Add(item, newPos);
                        _inventoryNode.CreateItem(item);
                        return;
                    }
                }
        }

        private void Pick(Vector2Int coord)
        {
            if (_inventory[coord] is null) throw new Exception($"Nothing in {coord}");

            _pickedItem = _inventory.DeleteByCoord(coord);
        }

        private void Put(Vector2Int putPos)
        {
            if (_pickedItem is null) throw new Exception("Nothing's picked");
            if (_pickedItem.Position != null) throw new Exception("Picked item isn't picked");
            var id = _pickedItem.Id;

            if (!_inventory.CanPlace(_pickedItem, putPos, true)) return;

            var itemsAtPutPos = _pickedItem.Layout
                .Select(x => _inventory[putPos + x.offset]?.Item)
                .FilterOutNulls()
                .Distinct();

            // Cant replace more than 1 item
            if (itemsAtPutPos.Count() > 1) return;

            if (itemsAtPutPos.Any())
            {
                var nextItem = itemsAtPutPos.First();
                _inventory.Delete(nextItem);
                _inventory.Add(_pickedItem, putPos);
                _pickedItem = nextItem;
                _inventoryNode.PickItem(_pickedItem.Id, Vector2Int.Zero);
                _pickOffset = Vector2Int.Zero;
            }
            else
            {
                _inventory.Add(_pickedItem, putPos);
                _pickedItem = null;
            }
            _inventoryNode.PutItem(id, putPos);
        }
    }
}
