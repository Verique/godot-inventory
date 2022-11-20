using System.Collections.Generic;
using System.Linq;
using Godot;
using Grate.Types;
using Grate.Utils;

namespace Grate.Inventory
{
    public class Inventory : Reference
    {
        private InventoryModule?[,] _grid;
        private Dictionary<int, Vector2Int> _itemPosById;
        private Vector2Int _size;

        public Inventory(Vector2Int size)
        {
            _size = size;
            _grid = new InventoryModule[size.x, size.y];
            _itemPosById = new Dictionary<int, Vector2Int>();
        }

        public ItemWithOffset? TryPopByPosition(Vector2Int pos) {
            var itemWithPos = TryGetItemAt(pos);
            if (itemWithPos == null) return null;
            
            if (!TryPop(itemWithPos.Item)) return null;
            else return itemWithPos;
        }

        public bool TryPlace(InventoryItem item, Vector2Int gridPos) {
            if (!CanPlace(item, gridPos)) return false;

            Add(item, gridPos);
            return true;
        }

        public ItemWithOffset? TryReplace(InventoryItem item, Vector2Int gridPos) {
            var replacementItem = TryGetReplacementItem(item, gridPos);
            if (replacementItem == null) return null;

            if (!TryPop(replacementItem)) return null;

            Add(item, gridPos);

            return new ItemWithOffset(replacementItem, Vector2Int.Zero);
        }

        private bool CanPlace(InventoryItem item, Vector2Int placePos) {
            return item.Layout
                .Select(x => x.Offset + placePos)
                .All(x => CheckCoordinatesValid(x) && this[x] == null);
        }

        private InventoryItem? TryGetReplacementItem(InventoryItem item, Vector2Int placePos) {
            var modulePoses = item.Layout
                .Select(x => x.Offset + placePos);

            if (!modulePoses.All(CheckCoordinatesValid)) return null;

            var items = modulePoses
                .Select(pos => this[pos]?.Item)
                .FilterOutNulls()
                .DistinctBy(item => item.Id);

            return items.Count() == 1 ? items.First() : null;
        }

        private InventoryModule? this[Vector2Int v]
        {
            get => _grid[v.x, v.y];
            set => _grid[v.x, v.y] = value;
        }

        private ItemWithOffset? TryGetItemAt(Vector2Int? v) {
            if (v == null) return null;
            var item = CheckCoordinatesValid(v) ? this[v]?.Item : null;

            if (item == null) return null;

            var itemPos = TryGetPos(item);
            if (itemPos == null) return null;

            return new ItemWithOffset(item, v - itemPos);
        } 

        private void Add(InventoryItem item, Vector2Int pos)
        {
            foreach (var (module, cell) in item.Layout.Select(x => (x, x.Offset + pos)))
            {
                this[cell] = module;
            }

            _itemPosById.Add(item.Id, pos);
        }

        private bool TryPop(InventoryItem item)
        {
            var itemPos = TryGetPos(item);
            if (itemPos == null) return false;

            foreach (var (module, cell) in item.Layout.Select(x => (x, x.Offset + itemPos)))
            {
                this[cell] = null;
            }

            _itemPosById.Remove(item.Id);
            return true;
        }

        private bool CheckCoordinatesValid(Vector2Int coords)
        {
            var (x, y) = coords;
            return !(x >= _size.x || x < 0 || y >= _size.y || y < 0);
        }

        private Vector2Int? TryGetPos(InventoryItem item) {
            return _itemPosById.ContainsKey(item.Id) 
                ? _itemPosById[item.Id]
                : null;
        }
    }

    public class ItemWithOffset
    {
        public InventoryItem Item { get; private set; }
        public Vector2Int GridOffset { get; private set; }

        public ItemWithOffset(InventoryItem item, Vector2Int gridOffset)
        {
            Item = item;
            GridOffset = gridOffset;
        }
    }
}
