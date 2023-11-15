using Godot;
using Grate.CustomInput;
using Grate.Inventory.Models;

namespace Grate.Inventory.Nodes {
    public partial class Inventory : Node2D {
        [Export] Button testAddButton = default!;

        public InventoryGrid ItemGrid { get; private set; } = new(SizeUtils.SizeX, SizeUtils.SizeY);
        public PickedItem? PickedItem { get; private set; }

        public override void _Ready() {
            testAddButton.ButtonUp += TryAdd;
        }

        public override void _Input(InputEvent @event) {
            @event.Process(click => ProcessClick(click.Position), move => MovePicked(move.Position));
        }

        private void TryAdd() {
            var item = ItemGrid.TryAdd();
            if (item is not null) this.AddChild(item);
        }

        private void ProcessClick(Vector2 v) {
            var gridPos = SizeUtils.ToGrid(v - Position);
            if (ItemGrid.CheckCoordinatesValid(gridPos))
                if (PickedItem is null) PickedItem = ItemGrid.TryPickItem(gridPos);
                else {
                    var placeResult = ItemGrid.TryPlaceItem(PickedItem.Item, gridPos + PickedItem.Offset);
                    if (placeResult) PickedItem = null;
                    else TryReplaceItem();
                }
            else
                DeletePickedItem();

            void TryReplaceItem() {
                var replaceItem = ItemGrid.TryReplaceItem(PickedItem.Item, gridPos + PickedItem.Offset);
                if (replaceItem is not null) {
                    Input.WarpMouse(replaceItem.Item.Position + Position);
                    PickedItem = replaceItem;
                }
            }
            void DeletePickedItem() {
                PickedItem?.Item.QueueFree(); PickedItem = null;
            }
        }

        private void MovePicked(Vector2 v) {
            if (PickedItem is null) return;
            PickedItem.Item.Position = v - Position + SizeUtils.ToPixels(PickedItem.Offset);
        }
    }
}
