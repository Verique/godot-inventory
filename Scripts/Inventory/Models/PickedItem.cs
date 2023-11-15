using Godot;
using Grate.Inventory.Nodes;

namespace Grate.Inventory.Models {
    public class PickedItem {
        public Item Item { get; } = default!;
        public Vector2I Offset { get; } = default!;

        public PickedItem(Item item, Vector2I offset) {
            Item = item;
            Offset = offset;
        }
    }
}
