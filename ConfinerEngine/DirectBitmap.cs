using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConfinerEngine
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void Clear(Color color)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    SetPixel(i, j, color);
                }
            }
        }

        public void DrawTriangleWire(Point p1, Point p2, Point p3, Color color)
        {
            DrawLine(p1, p2, color);
            DrawLine(p2, p3, color);
            DrawLine(p1, p3, color);
        }

        static BoundingBox FindBoundingBox(Vector2F[] abc, int width, int height)
        {
            Vector2F a = abc[0];
            Vector2F b = abc[1];
            Vector2F c = abc[2];

            Vector2F min = Vector2F.Min(Vector2F.Min(a, b), c);
            Vector2F max = Vector2F.Max(Vector2F.Max(a, b), c);
            BoundingBox bbox = new BoundingBox();
            bbox.min_x = Math.Max((int)Math.Ceiling(min.X), 0);
            bbox.min_y = Math.Max((int)Math.Ceiling(min.Y), 0);
            bbox.max_x = Math.Min((int)Math.Floor(max.X), width - 1);
            bbox.max_y = Math.Min((int)Math.Floor(max.Y), height - 1);
            
            return bbox;
        }

        static Vector3F CalculateWeights(Vector2F[] abc, Vector2F p)
        {
            Vector2F a = abc[0];
            Vector2F b = abc[1];
            Vector2F c = abc[2];
            Vector2F ab = b - a;
            Vector2F ac = c - a;
            Vector2F ap = p - a;
            float factor = 1 / (ab.X * ac.Y - ab.Y * ac.X);
            float s = (ac.Y * ap.X - ac.X * ap.Y) * factor;
            float t = (ab.X * ap.Y - ab.Y * ap.X) * factor;
            Vector3F weights = new Vector3F(1 - s - t, s, t);
            return weights;
        }


        public void DrawTriangle(Point p1, Point p2, Point p3, Color color)
        {
            var screen_coords = new List<Point> { p1, p2, p3 }.Select(x => new Vector2F(x.X, x.Y)).ToArray();
            BoundingBox bBox = FindBoundingBox(screen_coords, Width, Height);
            for (int x = bBox.min_x; x <= bBox.max_x; x++)
            {
                for (int y = bBox.min_y; y <= bBox.max_y; y++)
                {
                    Vector2F point = new Vector2F(x, y);
                    Vector3F weights = CalculateWeights(screen_coords, point);
                    if (weights.X >= 0 && weights.Y >= 0 && weights.Z >= 0)
                    {
                        SetPixel(x, y, color);
                    }
                }
            }

        }

        public void DrawLine(Point p1,Point p2,Color color)
        {
            DrawLine(p1.X, p1.Y, p2.X, p2.Y, color);
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                SetPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            if (index < Bits.Length)
                Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
