using Godot;

namespace Grate.Inventory {
    public static class SizeUtils {
        public const int SizeX = 10;
        public const int SizeY = 5;
        public const int CellSize = 80;

        public static Vector2 ToPixels(Vector2I v) => CellSize * v;
        public static Vector2I ToGrid(Vector2 v) {
            var t = v / CellSize;
            return new Vector2I(Mathf.RoundToInt(t.X), Mathf.RoundToInt(t.Y));
        }

        public static Vector2 ScaleTexture(Texture2D texture) {
            var textureSize = texture.GetSize();
            return new Vector2(CellSize / textureSize.X, CellSize / textureSize.Y);
        }
    }
}
