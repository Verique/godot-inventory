using System;
using System.Linq;
using Godot;
using Grate.Services;
using Grate.Types;
using Grate.Utils;

namespace Grate.Inventory
{
    public interface IInventoryService : IService
    {
        event Action<IInventoryItem>? ItemAdded;
        event Action<int, Vector2Int>? ItemPicked;
        event Action<int, Vector2Int>? ItemPut;
        event Action<int>? ItemDeleted;
    }

    public class InventoryService : Reference, IInventoryService
    {
        public event Action<IInventoryItem>? ItemAdded;
        public event Action<int>? ItemDeleted;
        public event Action<int, Vector2Int>? ItemPicked;
        public event Action<int, Vector2Int>? ItemPut;

        public InventoryItem? _pickedItem { get; private set; }
        private Inventory _inventory;
        private Vector2Int _gridSize;

        public InventoryService(CanvasLayer canvasLayer)
        {
            var testButton = canvasLayer.GetNode<Button>("Button");
            var node = canvasLayer.GetNode<InventoryNode>("Inventory");

            _gridSize = node.GridSize.ToVector2Int();
            _inventory = new Inventory(_gridSize);

            testButton.Connect("button_up", this, nameof(Add));
            node.LeftMouseButtonUp += OnLeftMouseButtonUp;
            node.Initialize(this);
        }

        private void OnLeftMouseButtonUp(Vector2Int? gridPos, Vector2Int offset)
        {
            var item = gridPos != null ? _inventory.ItemAt(gridPos) : null;
            var itemPos = item != null ? item.Position : null;

            if (_pickedItem != null)
            {
                if (gridPos == null) {
                    ItemDeleted?.Invoke(_pickedItem.Id);
                    _pickedItem = null;
                }
                else
                {
                    Put(gridPos - offset);
                }
            }
            else
            {
                if (itemPos != null)
                {
                    Pick(itemPos);
                    ItemPicked?.Invoke(item!.Id, gridPos! - itemPos);
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
                        foreach (var (module, offset) in item.Layout){
                            GD.Print($"Service coords {item.Position! + offset}");
                        }
                        ItemAdded?.Invoke(item);
                        return;
                    }
                }
        }

        public void Pick(Vector2Int coord)
        {
            if (_inventory[coord] is null) throw new Exception($"Nothing in {coord}");

            _pickedItem = _inventory.DeleteByCoord(coord);
        }

        public void DeletePickedItem()
        {
            if (_pickedItem == null) return;
            //PickedItem.InvokeItemDeletion();
            _pickedItem = null;
        }

        public void Put(Vector2Int putPos)
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
                ItemPicked?.Invoke(_pickedItem.Id, Vector2Int.Zero);
            }
            else
            {
                _inventory.Add(_pickedItem, putPos);
                _pickedItem = null;
            }
            ItemPut?.Invoke(id, putPos);
        }
    }
}
