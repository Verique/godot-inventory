using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryModel : Reference
    {
        public event Action<IInventoryItem> ItemAdded;

        public readonly Vector2Int Size;
        public InventoryItem PickedItem { get; private set; }

        private Dictionary<Vector2Int, InventoryItem> _itemsByPosition = new Dictionary<Vector2Int, InventoryItem>();
        private List<InventoryItem> Items => _itemsByPosition.Select(x => x.Value).Distinct().ToList();

        public InventoryModel(Vector2Int size)
        {
            Size = size;
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

            ItemAdded?.Invoke(item);
        }

        public void Pick(Vector2Int coord)
        {
            if (!_itemsByPosition.TryGetValue(coord, out var item)) throw new Exception($"No item at {coord}");
            PickedItem = PickItem(item, coord);
        }

        public void DeletePickedItem()
        {
            if (PickedItem == null) return;
            PickedItem.InvokeItemDeletion();
            PickedItem = null;
        }

        private InventoryItem PickItem(InventoryItem item, Vector2Int coord = null)
        {
            foreach (var moduleCoord in item.GetModuleCoordinates())
            {
                if (_itemsByPosition.ContainsKey(moduleCoord)) _itemsByPosition.Remove(moduleCoord);
                else throw new Exception($"Inventory doesn't contain anything at {moduleCoord}, but the item is telling otherwise");
            }

            item.Pick(coord ?? item.GridPos);
            return item;
        }

        private void PutItem(Vector2Int coord)
        {
            PickedItem.Put(coord);
            foreach (var moduleCoord in PickedItem.GetModuleCoordinates())
            {
                _itemsByPosition.Add(moduleCoord, PickedItem);
            }
            PickedItem = null;
        }

        public void Put(Vector2Int putPos)
        {
            if (PickedItem == null) throw new Exception("Nothing's picked");

            var itemsAtPutPos = ItemsAt(PickedItem.Layout.Select(x => putPos + PickedItem.PickOffset + x.LayoutPos).ToList());
            if ((itemsAtPutPos.Count() > 1)
                    || (!CanPlace(PickedItem, putPos + PickedItem.PickOffset, itemsAtPutPos.Any()))) return;

            if (itemsAtPutPos.Any())
            {
                var nextItem = PickItem(itemsAtPutPos.First());
                PutItem(putPos);
                PickedItem = nextItem;
            }
            else
            {
                PutItem(putPos);
            }
        }

        //TODO list of highlighted cells with color
        public bool CanPlace(InventoryItem item, Vector2Int placePos, bool replace = false)
        {
            foreach (var layoutItem in item.Layout.Select(x => x.LayoutPos))
            {
                var cell = layoutItem + placePos;

                if (!CheckCoordinatesValid(cell))
                    return false;
                if (!replace && _itemsByPosition.ContainsKey(cell))
                    return false;
            }
            return true;
        }

        public IReadOnlyCollection<InventoryItem> ItemsAt(List<Vector2Int> positions)
        {
            return positions.Where(pos => _itemsByPosition.ContainsKey(pos)).Select(pos => _itemsByPosition[pos]).Distinct().ToList();
        }

        public bool CheckCoordinatesValid(Vector2Int coords)
        {
            var (x, y) = coords;
            return !(x >= Size.x || x < 0 || y >= Size.y || y < 0);
        }

        public bool CanPut(Vector2Int putPos) => CanPlace(PickedItem, putPos + PickedItem.PickOffset, true);
        public bool HasItemAt(Vector2Int pos) => _itemsByPosition.ContainsKey(pos);
    };
}
