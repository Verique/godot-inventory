using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryModel : Reference
    {
        public readonly Vector2Int Size;
        public event Action<IReadOnlyCollection<IInventoryItemInfo>, IInventoryItemInfo> ModelChanged;
        
        private Dictionary<Vector2Int, InventoryItem> _itemsByPosition = new Dictionary<Vector2Int, InventoryItem>();
        private InventoryItem _pickedItem = null;

        public InventoryModel(Vector2Int size)
        {
            Size = size;
        }

        public void InvokeModelChange()
        {
            ModelChanged?.Invoke(_itemsByPosition.Values.Distinct().ToList(), _pickedItem);
        }

        public void Add(InventoryItem item, Vector2Int pos)
        {
            if (!CanPlace(item, pos)) throw new Exception($"Place {pos} isn't valid");

            item.MoveTo(pos);
            var cellsToOccupy = item.GetModuleCoordinates();

            foreach (var cell in cellsToOccupy)
            {
                _itemsByPosition.Add(cell, item);
            }

            InvokeModelChange();
        }

        public void Pick(Vector2Int coord)
        {
            if (!_itemsByPosition.TryGetValue(coord, out var item)) throw new Exception($"No item at {coord}");

            foreach (var moduleCoord in item.GetModuleCoordinates())
            {
                if (_itemsByPosition.ContainsKey(moduleCoord)) _itemsByPosition.Remove(moduleCoord);
                else throw new Exception($"Inventory doesn't contain anything at {moduleCoord}, but the item is telling otherwise");
            }

            _pickedItem = item;
            _pickedItem.Pick(coord);
            InvokeModelChange();
        }

        public void Put(Vector2Int putPos)
        {
            if (_pickedItem == null) throw new Exception("Nothing's picked");

            if (!CanPlace(_pickedItem, putPos + _pickedItem.PickOffset)) return;

            _pickedItem.Put(putPos);
            foreach (var coord in _pickedItem.GetModuleCoordinates())
            {
                _itemsByPosition.Add(coord, _pickedItem);
            }

            _pickedItem = null;
            InvokeModelChange();
        }

        public bool CanPlace(InventoryItem item, Vector2Int placePos)
        {
            foreach (var layoutItem in item.Layout)
            {
                var cell = layoutItem + placePos;

                if (!CheckCoordinatesValid(cell))
                    return false;
                if (_itemsByPosition.ContainsKey(cell))
                    return false;
            }
            return true;
        }

        public bool CheckCoordinatesValid(Vector2Int coords)
        {
            var (x, y) = coords.Unpack();
            return !(x >= Size.x || x < 0 || y >= Size.y || y < 0);
        }
        
        public bool CanPut(Vector2Int putPos) => CanPlace(_pickedItem, putPos + _pickedItem.PickOffset);
        public bool HasItemAt(Vector2Int pos) => _itemsByPosition.ContainsKey(pos);
    };
}
