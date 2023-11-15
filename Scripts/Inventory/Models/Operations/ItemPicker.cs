using System.Linq;
using Godot;
using Grate.Inventory.Nodes;
using Grate.Types;

namespace Grate.Inventory.Models; 
internal class ItemPicker {
    Grid<Item?> _itemGrid;

    public ItemPicker(Grid<Item?> itemGrid) {
        _itemGrid = itemGrid;
    }

    public PickedItem? TryPick(Vector2I v) {
        if (!CanPick(v)) return null;
        return Pick(v);
    }

    public PickedItem Pick(Vector2I v) {
        var item = _itemGrid[v]!;
        var basePos = SizeUtils.ToGrid(item.Position);
        var occupiedPositions = item.Layout.Select(x => basePos + x);
        foreach (var pos in occupiedPositions) _itemGrid[pos] = null;
        return new(item, basePos - v);
    }

    private bool CanPick(Vector2I v) {
        return _itemGrid.CheckCoordinatesValid(v) && _itemGrid[v] != null;
    }
}
