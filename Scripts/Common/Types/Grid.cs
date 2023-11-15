using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grate.Types {
    public class Grid<T> {
        private T[,] _array;
        public Vector2I Size => new Vector2I(_array.GetLength(0), _array.GetLength(1));

        public Grid(Vector2I size) {
            _array = new T[size.X, size.Y];
        }

        public Grid(int sizeX, int sizeY) : this(new Vector2I(sizeX, sizeY)) { }

        public bool CheckCoordinatesValid(Vector2I position) {
            var (x, y) = position;
            return !(x >= _array.GetLength(0) || x < 0 || y >= _array.GetLength(1) || y < 0);
        }

        public bool CheckPositions(IEnumerable<Vector2I> positions, Func<T?, bool> check) {
            return positions.All(x => CheckCoordinatesValid(x) && check(this[x]));
        }

        public bool AreValidPositions(IEnumerable<Vector2I> positions) {
            return positions.All(CheckCoordinatesValid);
        }

        public T this[Vector2I v] {
            get => _array[v.X, v.Y];
            set => _array[v.X, v.Y] = value;
        }
    }

    public static class GridExtensions {
        public static bool AreValidEmptyPositions<T> (this Grid<T?> that, IEnumerable<Vector2I> positions) where T: class {
            return that.CheckPositions(positions, x => x is null);
        }
        public static bool AreValidEmptyPositions<T> (this Grid<T?> that, IEnumerable<Vector2I> positions) where T: struct {
            return that.CheckPositions(positions, x => !x.HasValue);
        }
    }
}
