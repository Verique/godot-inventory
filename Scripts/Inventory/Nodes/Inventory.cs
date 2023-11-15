using System.Linq;
using Godot;
using Grate.CustomInput;
using Grate.Inventory.Models;

namespace Grate.Inventory.Nodes; 
public partial class Inventory : Node2D {
    [Export] Button testAddButton = default!;

    public InventoryGrid ItemGrid { get; private set; } = new(SizeUtils.SizeX, SizeUtils.SizeY);
    public PickedItem? PickedItem {
        get => _pickedItem;
        private set {
            _pickedItem = value;
            Input.MouseMode = value is null ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Hidden;
        }
    }

    private PickedItem? _pickedItem;

    public override void _Ready() {
        testAddButton.ButtonUp += TryAdd;
    }

    public override void _Input(InputEvent @event) {
        var e = MakeInputLocal(@event);
        e.Process(click => ProcessClick(click.Position), move => MovePicked(move.Position));
    }

    private void TryAdd() {
        var item = ItemGrid.TryAdd();
        if (item is not null) this.AddChild(item);
    }

    private void MovePicked(Vector2 v) {
        if (PickedItem is null) return;
        PickedItem.Item.Position = v + SizeUtils.ToPixels(PickedItem.Offset);
    }

    private void ProcessClick(Vector2 v) {
        var gridPos = SizeUtils.ToGrid(v);

        if (ItemGrid.CheckCoordinatesValid(gridPos))
            ProcessClickInside(gridPos);
        else
            DeletePickedItem();

        void DeletePickedItem() {
            PickedItem?.Item.QueueFree(); PickedItem = null;
        }
    }

    private void ProcessClickInside(Vector2I gridPos) {
        if (PickedItem is null) PickedItem = ItemGrid.TryPickItem(gridPos);
        else {
            if (ItemGrid.TryPlaceItem(PickedItem.Item, gridPos + PickedItem.Offset))
                PickedItem = null;
            else
                TryReplaceItem();
        }

        void TryReplaceItem() {
            var replaceItem = ItemGrid.TryReplaceItem(PickedItem.Item, gridPos + PickedItem.Offset);
            if (replaceItem is not null) {
                var replaceItemPos = SizeUtils.ToGrid(replaceItem.Item.Position);
                var allPoses = replaceItem.Item.Layout.Select(x => x + replaceItemPos);
                var closestPos = allPoses.MinBy(x => (x - gridPos).LengthSquared());
                Input.WarpMouse(ToGlobal(SizeUtils.ToPixels(closestPos)));
                var newReplace = new PickedItem(replaceItem.Item, replaceItemPos - closestPos);
                PickedItem = newReplace;
            }
        }
    }
}
