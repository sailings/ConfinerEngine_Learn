using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfinerEngine
{
    public struct Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public struct Vector2F
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2F operator /(Vector2F vec, float div)
        {
            return new Vector2F(vec.X / div, vec.Y / div);
        }

        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X-b.X,a.Y-b.Y);
        }

        public static Vector2F Min(Vector2F v1, Vector2F v2)
        {
            float x = Math.Min(v1.X, v2.X);
            float y = Math.Min(v1.Y, v2.Y);
            return new Vector2F(x, y);
        }

        public static Vector2F Max(Vector2F v1, Vector2F v2)
        {
            float x = Math.Max(v1.X, v2.X);
            float y = Math.Max(v1.Y, v2.Y);
            return new Vector2F(x, y);
        }
    }

    public struct Vector3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3(int x, int y,int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct Vector3F
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3F operator /(Vector3F vec, float div)
        {
            return new Vector3F(vec.X / div, vec.Y / div, vec.Z / div);
        }
    }

    public struct BoundingBox {
        public int min_x { get; set; }
        public int min_y { get; set; }
        public int max_x { get; set; }
        public int max_y { get; set; }
    }
}

