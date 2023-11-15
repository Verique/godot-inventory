// using System.Collections.Generic;
// using Godot;

// namespace Grate.Types.Grid {
// 	public partial class PixelGridHighlighterNode : Control {
// 		public IReadOnlyCollection<(Vector2I, Color)> Highlights { get => _highlights; set { _highlights = value; QueueRedraw(); } }
// 		public PixelGrid PixelGrid { get => _grid; set { _grid = value; QueueRedraw(); } }

// 		private IReadOnlyCollection<(Vector2I, Color)> _highlights = new List<(Vector2I, Color)>();
// 		private PixelGrid _grid = default!;

// 		public override void _Draw() {
// 			foreach (var (pos, color) in _highlights) {
// 				var rect = new Rect2() { Size = _grid.CellSize * Vector2.One, Position = _grid.LeftTopPointOfCell(pos) };
// 				DrawRect(rect, color);
// 			}
// 		}
// 	}
// }
