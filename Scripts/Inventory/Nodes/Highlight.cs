using Godot;

namespace Grate.Inventory.Nodes;

public class Highlight {
    public Vector2I Position { get; private set; }
    public Color Color { get; private set; }

    public Highlight(Vector2I position, Color color)
    {
        Position = position;
        Color = color;
    }
}

