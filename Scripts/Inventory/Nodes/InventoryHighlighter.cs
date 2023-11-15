using Godot;
using Grate.CustomInput;
using System.Collections.Generic;

namespace Grate.Inventory.Nodes;
public partial class InventoryHighlighter : Node2D {
    private readonly Color defaultHighlightColor = new Color(1, 1, 1, 0.2f);

    public List<Highlight> Highlights {
        get => _highlights;
        private set {
            _highlights = value;
            QueueRedraw();
        }
    }

    private List<Highlight> _highlights = new();

    public override void _Input(InputEvent @event) {
        var e = MakeInputLocal(@event);
        e.Process(onMouseMove: move => Highlights = new() { new(SizeUtils.ToGrid(move.Position), defaultHighlightColor) });
    }

    // TODO: highlight outside grid, different highlight if picked;
    public override void _Draw() {
        foreach (var highlight in _highlights) {
            var rect = new Rect2() { Size = SizeUtils.CellSize * Vector2.One, Position = SizeUtils.ToPixels(highlight.Position) - Vector2.One / 2 * SizeUtils.CellSize };
            DrawRect(rect, highlight.Color);
        }
    }
}
