using System;
using Godot;

namespace Grate.Types
{
    public class Grid : Reference
    {
        public readonly Vector2Int GridSize;
        public readonly int CellSize;

        public Vector2 RectSize => (GridSize * CellSize).ToVector2();

        public Grid(Vector2Int gridSize, int cellSize)
        {
            GridSize = gridSize;
            CellSize = cellSize;
        }

        public Vector2Int LocalToGrid(Vector2 local)
        {
            if (!HasPoint(local)) throw new Exception("Point not on the grid");
            return ConvertToGrid(local);
        }

        public Vector2 CenterPointOfCell(Vector2Int cellCoordinates)
        {
            return LeftTopPointOfCell(cellCoordinates) + Vector2.One * CellSize / 2;
        }

        public Vector2 LeftTopPointOfCell(Vector2Int cellCoordinates)
        {
            return cellCoordinates.ToVector2() * CellSize;
        }

        public bool HasPoint(Vector2 local)
        {
            return IsValidCoordinate(ConvertToGrid(local));
        }

        public bool IsValidCoordinate(Vector2Int coord)
        {
            return (coord.x >= 0 && coord.x < GridSize.x)
                && (coord.y >= 0 && coord.y < GridSize.y);
        }

        private Vector2Int ConvertToGrid(Vector2 local)
        {
            var x = Mathf.FloorToInt(local.x / CellSize);
            var y = Mathf.FloorToInt(local.y / CellSize);

            return new Vector2Int(x, y);
        }
    }
}
