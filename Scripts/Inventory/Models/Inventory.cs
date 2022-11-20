using System.Linq;
using Godot;
using Grate.Types;
using Grate.Utils;

namespace Grate.Inventory
{
    public class Inventory : Reference
    {
        private InventoryModule?[,] _grid;
        private Vector2Int _size;

        public Inventory(Vector2Int size)
        {
            _size = size;
            _grid = new InventoryModule[size.x, size.y];
        }

        public ItemWithOffset? TryPopByPosition(Vector2Int pos) {
            var itemWithPos = TryGetItemAt(pos);
            if (itemWithPos == null) return null;
            
            if (TryPop(itemWithPos.Item) == null) return null;
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

            if (TryPop(replacementItem) == null) return null;

            Add(item, gridPos);

            return new ItemWithOffset(replacementItem, Vector2Int.Zero);
        }

        private bool CanPlace(InventoryItem item, Vector2Int placePos) {
            return item.Layout
                .Select(x => x.offset + placePos)
                .All(x => CheckCoordinatesValid(x) && this[x] == null);
        }

        private InventoryItem? TryGetReplacementItem(InventoryItem item, Vector2Int placePos) {
            var modulePoses = item.Layout
                .Select(x => x.offset + placePos);

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

            var itemPos = item.Position!;

            return new ItemWithOffset(item, v - itemPos);
        } 

        private void Add(InventoryItem item, Vector2Int pos)
        {
            foreach (var (module, cell) in item.Layout.Select(x => (x.module, x.offset + pos)))
            {
                this[cell] = module;
            }

            item.Position = pos;
        }

        private Vector2Int? TryPop(InventoryItem item)
        {
            if (item.Position is null) return null;

            foreach (var (module, cell) in item.Layout.Select(x => (x.module, x.offset + item.Position)))
            {
                this[cell] = null;
            }

            var lastPos = item.Position;
            item.Position = null;
            return lastPos;
        }

        private bool CheckCoordinatesValid(Vector2Int coords)
        {
            var (x, y) = coords;
            return !(x >= _size.x || x < 0 || y >= _size.y || y < 0);
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
