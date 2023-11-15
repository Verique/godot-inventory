using Godot;
using Grate.Inventory.Nodes;
using Grate.Types;

namespace Grate.Inventory.Models; 
public class InventoryGrid : Grid<Item?> {
    private ItemPlacer ItemPlacer;
    private ItemReplacer ItemReplacer;
    private ItemSpawner ItemSpawner;
    private ItemPicker ItemPicker;

    public InventoryGrid(int x, int y) : base(x, y) {
        ItemPlacer = new(this);
        ItemPicker = new(this);
        ItemSpawner = new(this, ItemPlacer);
        ItemReplacer = new(this, ItemPlacer, ItemPicker);
    }

    public Item? TryAdd() => ItemSpawner.TryAddNewItem();
    public PickedItem? TryPickItem(Vector2I v) => ItemPicker.TryPick(v);
    public bool TryPlaceItem(Item item, Vector2I v) => ItemPlacer.TryPlace(item, v);
    public PickedItem? TryReplaceItem(Item item, Vector2I v) => ItemReplacer.TryReplace(item, v);
}
