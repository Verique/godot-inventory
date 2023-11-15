using Godot;

namespace Grate.Inventory.Nodes; 
public partial class InventoryDrawer : Node2D {
    public override void _Draw() {
        for (int x = 0; x <= SizeUtils.SizeX; x++) {
            var startPoint = new Vector2(x - .5f, -.5f) * SizeUtils.CellSize;
            DrawLine(startPoint, startPoint + new Vector2(0, SizeUtils.SizeY * SizeUtils.CellSize), new Color(0, 0, 1, 1));
        }
        for (int y = 0; y <= SizeUtils.SizeY; y++) {
            var startPoint = new Vector2(-.5f, y - .5f) * SizeUtils.CellSize;
            DrawLine(startPoint, startPoint + new Vector2(SizeUtils.SizeX * SizeUtils.CellSize, 0), new Color(0, 0, 1, 1));
        }
    }
}
