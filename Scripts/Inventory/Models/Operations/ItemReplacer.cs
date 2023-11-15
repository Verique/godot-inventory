using System.Linq;
using Godot;
using Grate.Inventory.Nodes;

namespace Grate.Inventory.Models {
    internal class ItemReplacer {
        private InventoryGrid _inventoryGrid;
        private ItemPlacer _itemPlacer;
        private ItemPicker _itemPicker;

        public ItemReplacer(InventoryGrid inventoryGrid, ItemPlacer itemPlacer, ItemPicker itemPicker) {
            _inventoryGrid = inventoryGrid;
            _itemPlacer = itemPlacer;
            _itemPicker = itemPicker;
        }

        public PickedItem? TryReplace(Item item, Vector2I v) {
            var newPositions = item.Layout.Select(x => x + v);
            if (!_inventoryGrid.AreValidPositions(newPositions)) return null;
            var replaces = newPositions.Select(x => _inventoryGrid[x]).Where(x => x != null).Select(x => x!).Distinct();
            if (replaces.Count() != 1) return null;
            var replaceItem = _itemPicker.Pick(SizeUtils.ToGrid(replaces.First().Position));
            _itemPlacer.Place(item, v);
            return replaceItem;
        }
    }
}
