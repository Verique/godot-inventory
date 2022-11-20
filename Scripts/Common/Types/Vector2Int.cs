using Godot;
namespace Grate.Types
{
    public static class VectorExtensions
    {
        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }
    }

    [System.Serializable]
    public class Vector2Int
    {
        public int x = 0;
        public int y = 0;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int SqrMagnitude => x ^ 2 + y ^ 2;

        public void Deconstruct(out int X, out int Y)
        {
            X = x;
            Y = y;
        }

        public static Vector2Int operator *(Vector2Int vector, int operand)
        {
            return new Vector2Int(vector.x * operand, vector.y * operand);
        }

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x - v2.x, v1.y - v2.y);
        }

        public Vector2 ToVector2() => new Vector2(x, y);

        public static Vector2Int Zero = new Vector2Int(0, 0);
        public static Vector2Int One = new Vector2Int(1, 1);
        public static Vector2Int Left = new Vector2Int(-1, 0);
        public static Vector2Int Right = new Vector2Int(1, 0);
        public static Vector2Int Up = new Vector2Int(0, 1);
        public static Vector2Int Down = new Vector2Int(0, -1);

        public override string ToString()
        {
            return $"{{{x};{y}}}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2Int v)) return false;

            return x.Equals(v.x) && y.Equals(v.y);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
