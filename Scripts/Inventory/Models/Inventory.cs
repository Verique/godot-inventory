using System;
using System.Linq;
using Godot;
using Grate.Types;

namespace Grate.Inventory
{
    public class Inventory : Reference
    {
        public InventoryModule?[,] Grid { get; private set; }
        private Vector2Int _size;

        public Inventory(Vector2Int size)
        {
            _size = size;
            Grid = new InventoryModule[size.x, size.y];
        }

        public void Add(InventoryItem item, Vector2Int pos)
        {
            foreach (var (module, cell) in item.Layout.Select(x => (x.module, x.offset + pos)))
            {
                this[cell] = module;
            }

            item.Position = pos;
        }

        public void Delete(InventoryItem item)
        {
            if (item.Position is null) throw new Exception($"Item not placed");

            foreach (var (module, cell) in item.Layout.Select(x => (x.module, x.offset + item.Position)))
            {
                this[cell] = null;
            }

            item.Position = null;
        }

        public InventoryItem DeleteByCoord(Vector2Int coords)
        {
            if (this[coords] is null) throw new Exception($"No items at {coords}");

            var item = this[coords]!.Item;
            Delete(item);
            return item;
        }

        //TODO list of highlighted cells with color
        public bool CanPlace(InventoryItem item, Vector2Int placePos, bool replace = false)
        {
            return item.Layout
                .Select(x => x.offset + placePos)
                .All(x =>
                   CheckCoordinatesValid(x)
                   && (replace || this[x] is null));
        }

        public InventoryModule? this[Vector2Int v]
        {
            get => Grid[v.x, v.y];
            private set => Grid[v.x, v.y] = value;
        }

        public InventoryItem? ItemAt(Vector2Int v) => this[v]?.Item;

        private bool CheckCoordinatesValid(Vector2Int coords)
        {
            var (x, y) = coords;
            return !(x >= _size.x || x < 0 || y >= _size.y || y < 0);
        }
    }
}
