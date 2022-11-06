using System.Collections.Generic;
using Godot;

namespace Grate.Inventory
{
    public class InventoryItemModuleNode : TextureRect
    {
        List<Vector2> DrawPoses = new List<Vector2>();

        public InventoryItemModuleNode(Color color, Vector2 position, InventoryModule module)
        {
            RectPosition = position;
            this.Texture = ResourceLoader.Load<StreamTexture>("res://sprites/item1x1.png");
            this.SelfModulate = color;
            this.MouseFilter = MouseFilterEnum.Pass;
            if (module.Down) DrawPoses.Add(new Vector2(40, 80));
            if (module.Up) DrawPoses.Add(new Vector2(40, 0));
            if (module.Left) DrawPoses.Add(new Vector2(0, 40));
            if (module.Right) DrawPoses.Add(new Vector2(80, 40));
            Update();
        }

        public override void _Draw()
        {
            foreach (var point in DrawPoses)
            {
                DrawCircle(point, 10, Colors.White);
            }
        }
    }
}
