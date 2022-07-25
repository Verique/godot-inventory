using System;
using System.Collections.Generic;
using Godot;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryModel : Reference
    {
        public event Action<IReadOnlyCollection<InventoryItem>, InventoryItem> ModelChanged;

        public readonly Vector2Int Size;

        private InventoryModule[,] _cells;
        private InventoryItem _pickedItem;
        private List<InventoryItem> _items;

        public InventoryModel(Vector2Int size)
        {
            Size = size;
            _items = new List<InventoryItem>();
            _cells = new InventoryModule[size.x, size.y];
        }

        public void InvokeModelChange()
        {
            ModelChanged?.Invoke(_items, _pickedItem);
        }

        public void Add(InventoryItem item)
        {
            foreach (var cell in item.GetModuleCoordinates())
            {
                if (!CheckCoordinatesValid(cell))
                    throw new Exception($"Place [{cell.x},{cell.y}] isn't valid");
                if (this[cell] != null)
                    throw new Exception($"Place [{cell.x},{cell.y}] is occupied");
                this[cell] = new InventoryModule(item);
            }

            _items.Add(item);
            InvokeModelChange();
        }

        public void Pick(Vector2Int coord)
        {
            var item = this[coord].Item;
            foreach (var moduleCoord in item.GetModuleCoordinates())
            {
                this[moduleCoord] = null;
            }
            _pickedItem = item;
            _pickedItem.PickPos = item.MainPos - coord;
            InvokeModelChange();
        }

        //TODO: put problems with MainPos
        public void Put(Vector2Int pos)
        {
            if (_pickedItem == null) throw new Exception("Nothing's picked");

            _pickedItem.MainPos = pos + _pickedItem.PickPos;
            if (!CanPlace(_pickedItem)) return; //throw new Exception("Can't place item here");

            foreach (var coord in _pickedItem.GetModuleCoordinates())
            {
                this[coord] = new InventoryModule(_pickedItem);
            }

            _pickedItem = null;
            InvokeModelChange();
        }

        public bool CanPlace(InventoryItem item)
        {
            foreach (var cell in item.GetModuleCoordinates())
            {
                if (!CheckCoordinatesValid(cell))
                    return false;
                if (this[cell] != null)
                    return false;
            }
            return true;
        }

        public bool CanPut => CanPlace(_pickedItem);

        public bool CheckCoordinatesValid(Vector2Int coords)
        {
            var (x, y) = coords.Unpack();
            return !(x >= Size.x || x < 0 || y >= Size.y || y < 0);
        }

        public InventoryModule this[Vector2Int v] { get => _cells[v.x, v.y]; private set => _cells[v.x, v.y] = value; }
    };
}
