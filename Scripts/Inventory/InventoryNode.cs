using System;
using Godot;
using Grate.Input;
using Grate.Types;

namespace Grate.Inventory
{
    public class InventoryNode : Control
    {
        [Export] public Vector2 GridSize = new Vector2(10, 5);
        [Export] public int CellSize = 80;

        public event Action<Vector2>? LeftMouseButtonUp;

        public Grid Grid { get; private set; }

        private Vector2? _cursorPos;

        public InventoryNode()
        {
            Grid = new Grid(GridSize.ToVector2Int(), CellSize);
        }

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore;
        }

        public override void _Draw()
        {
            for (int x = 0; x <= Grid.GridSize.x; x++)
            {
                var startPoint = new Vector2(x * Grid.CellSize, 0);
                DrawLine(startPoint, startPoint + new Vector2(0, Grid.GridSize.y * Grid.CellSize), Color.ColorN("blue", 1));
            }
            for (int y = 0; y <= Grid.GridSize.y; y++)
            {
                var startPoint = new Vector2(0, y * Grid.CellSize);
                DrawLine(startPoint, startPoint + new Vector2(Grid.GridSize.x * Grid.CellSize, 0), Color.ColorN("blue", 1));
            }
            if (_cursorPos is Vector2 validPos) DrawCircle(validPos, Grid.CellSize / 2, Color.ColorN("white", 0.3f));
        }

        public override void _Input(InputEvent @event)
        {
            var _e = MakeInputLocal(@event);
            switch (_e)
            {
                case InputEventMouseMotion e:
                    if (Grid.HasPoint(e.Position))
                        _cursorPos = Grid.CenterPointOfCell(Grid.LocalToGrid(e.Position));
                    else
                        _cursorPos = null;
                    Update();
                    return;
                case InputEventMouseButton mb:
                    if (mb.IsLeftMouseUp())
                        LeftMouseButtonUp?.Invoke(mb.Position);
                    break;
                default:
                    return;
            }
        }

        public void CreateItemNode(IInventoryItem item)
        {
            AddChild(new InventoryItemNode(item, Grid));
        }
    }
}
