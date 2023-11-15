using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grate.Inventory.Nodes {
    public partial class Item : Node2D {
        public List<Vector2I> Layout { get; private set; } = new List<Vector2I>();
        public event Action<Vector2>? ItemClicked;
        [Export] PackedScene _cellScene = default!;

        public void Initialize(IEnumerable<Vector2I> layout) {
            foreach (var child in this.GetChildren()) child.QueueFree();
            foreach (var cell in layout) {
                var node = _cellScene.Instantiate<Sprite2D>();
                node.Scale = SizeUtils.ScaleTexture(node.Texture);
                node.Position = SizeUtils.ToPixels(cell);
                node.Name = cell.ToString();
                AddChild(node);
            }
            Layout = layout.ToList();
        }

        public override void _Ready() {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            Modulate = Color.Color8((byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255), (byte)rng.RandiRange(0, 255));
        }
    }
}
