using System.Linq;
using Godot;
using Grate.Inventory.Nodes;
using Grate.Types;

namespace Grate.Inventory.Models; 
internal class ItemPlacer {
    Grid<Item?> _itemGrid;
    public ItemPlacer(Grid<Item?> itemGrid) { _itemGrid = itemGrid; }

    public bool TryPlace(Item item, Vector2I v) {
        if (!CanPlace(item, v)) return false;
        Place(item, v);
        return true;
    }

    public void Place(Item item, Vector2I v) {
        item.Position = SizeUtils.ToPixels(v);
        var newPoses = item.Layout.Select(x => x + v);
        foreach (var cell in newPoses) _itemGrid[cell] = item;
    }

    private bool CanPlace(Item item, Vector2I v) {
        var newPoses = item.Layout.Select(x => x + v);
        return (_itemGrid.AreValidEmptyPositions(newPoses));
    }
}
