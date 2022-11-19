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
        private (InventoryItem Item, Vector2Int Offset)? _pickedItem { get; set; }
        private Inventory _inventory;
        private InventoryNode _inventoryNode;
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
            if (_pickedItem != null) {
                var (item, offset) = _pickedItem.Value;
                var pickOffset = (offset.ToVector2() + (Vector2.One / 2)) * _inventoryNode.Grid.CellSize;
                _inventoryNode.MoveItem(item.Id, mousePos - pickOffset);
            }
        }

        private void OnLeftMouseButtonUp(Vector2Int? gridPos)
        {
            if (_pickedItem != null)
            {
                var (item, offset) = _pickedItem.Value;
                if (gridPos == null)
                {
                    _inventoryNode.DeleteItem(item.Id);
                    _pickedItem = null;
                }
                else
                {
                    Put(gridPos - offset);
                }
            }
            else
            {
                var item = gridPos != null ? _inventory.ItemAt(gridPos) : null;
                var itemPos = item != null ? item.Position : null;
                if (itemPos != null)
                {
                    _pickedItem = (Pick(itemPos), gridPos! - itemPos);
                    _inventoryNode.PickItem(item!.Id);
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

        private InventoryItem Pick(Vector2Int coord)
        {
            if (_inventory[coord] is null) throw new Exception($"Nothing in {coord}");

            return _inventory.DeleteByCoord(coord);
        }

        private void Put(Vector2Int putPos)
        {
            if (_pickedItem is null) throw new Exception("Nothing's picked");
            var pickedItem = _pickedItem.Value.Item;
            if (pickedItem.Position != null) throw new Exception("Picked item isn't picked");
            var id = pickedItem.Id;

            if (!_inventory.CanPlace(pickedItem, putPos, true)) return;

            var itemsAtPutPos = pickedItem.Layout
                .Select(x => _inventory[putPos + x.offset]?.Item)
                .FilterOutNulls()
                .Distinct();

            // Cant replace more than 1 item
            if (itemsAtPutPos.Count() > 1) return;

            if (itemsAtPutPos.Any())
            {
                var nextItem = itemsAtPutPos.First();
                _inventory.Delete(nextItem);
                _inventory.Add(pickedItem, putPos);
                _inventoryNode.PickItem(nextItem.Id);
                _pickedItem = (nextItem, Vector2Int.Zero);
            }
            else
            {
                _inventory.Add(pickedItem, putPos);
                _pickedItem = null;
            }
            _inventoryNode.PutItem(id, putPos);
        }
    }
}
