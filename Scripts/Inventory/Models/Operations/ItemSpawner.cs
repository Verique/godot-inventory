using Godot;
using Grate.Inventory.Nodes;
using Grate.Types;

namespace Grate.Inventory.Models {
    internal class ItemSpawner {
        Grid<Item?> _itemGrid;
        ItemPlacer _placer;
        PackedScene _itemScene;

        public ItemSpawner(Grid<Item?> itemGrid, ItemPlacer placer) {
            _itemGrid = itemGrid;
            _placer = placer;
            _itemScene = GD.Load<PackedScene>("res://Scenes/Item.tscn");
        }

        public Item? TryAddNewItem() {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            var layout = Utils.GenerateLayout(rng.RandiRange(2, 5));

            var item = _itemScene.Instantiate<Item>();
            item.Initialize(layout);
            for (int i = 0; i < _itemGrid.Size.X; i++)
                for (int j = 0; j < _itemGrid.Size.Y; j++) {
                    var coord = new Vector2I(i, j);
                    if (_placer.TryPlace(item, coord)) return item;
                }
            return null;
        }
    }
}
