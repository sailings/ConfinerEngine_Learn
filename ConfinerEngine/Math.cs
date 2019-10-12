using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfinerEngine
{
    public struct Vector2
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Vector2F
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vector2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2F operator /(Vector2F vec, float div)
        {
            return new Vector2F(vec.x / div, vec.y / div);
        }

        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.x - b.x, a.y - b.y);
        }

        public static Vector2F Min(Vector2F v1, Vector2F v2)
        {
            float x = Math.Min(v1.x, v2.x);
            float y = Math.Min(v1.y, v2.y);
            return new Vector2F(x, y);
        }

        public static Vector2F Max(Vector2F v1, Vector2F v2)
        {
            float x = Math.Max(v1.x, v2.x);
            float y = Math.Max(v1.y, v2.y);
            return new Vector2F(x, y);
        }
        public static Vector2F operator *(Vector2F v, float factor)
        {
            return new Vector2F(v.x * factor, v.y * factor);
        }
    }

    public struct Vector3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct Vector3F
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3F operator -(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3F operator /(Vector3F vec, float div)
        {
            return new Vector3F(vec.x / div, vec.y / div, vec.z / div);
        }

        public static Vector3F operator *(Vector3F v, float factor)
        {
            return new Vector3F(v.x * factor, v.y * factor, v.z * factor);
        }

        public static float Dot(Vector3F a, Vector3F b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3F Vec3_From_Vec4(Vector4F v)
        {
            return new Vector3F(v.x,v.y,v.z);
        }

        public float Length()
        {
            return (float)Math.Sqrt(Dot(this, this));
        }

        public Vector3F Normalize()
        {
            return this/Length();
        }

        public static Vector3F Cross(Vector3F a, Vector3F b)
        {
            float x = a.y * b.z - a.z * b.y;
            float y = a.z * b.x - a.x * b.z;
            float z = a.x * b.y - a.y * b.x;
            return new Vector3F(x, y, z);
        }

        public static Vector3F Vec3_Modulate(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.x * b.x, a.y * b.y, a.z * b.z);
        }
    }

    public struct Vector4F
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public Vector4F(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Vector4F Vec4_From_Vec3(Vector3F v, float w)
        {
            return new Vector4F(v.x, v.y, v.z, w);
        }

        public override string ToString()
        {
            return $"{x},{y},{z},{w}";
        }
    }

    public struct BoundingBox
    {
        public int min_x { get; set; }
        public int min_y { get; set; }
        public int max_x { get; set; }
        public int max_y { get; set; }
    }

    public class Matrix4x4
    {
        public float[,] m = new float[4, 4];

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{m[0, 0]},{m[0, 1]},{m[0, 2]},{m[0, 3]}");
            sb.AppendLine($"{m[1, 0]},{m[1, 1]},{m[1, 2]},{m[1, 3]}");
            sb.AppendLine($"{m[2, 0]},{m[2, 1]},{m[2, 2]},{m[2, 3]}");
            sb.AppendLine($"{m[3, 0]},{m[3, 1]},{m[3, 2]},{m[3, 3]}");
            return sb.ToString();
        }

        public static Matrix4x4 Identity
        {
            get
            {
                return new Matrix4x4
                {
                    m = new float[4, 4]{
                        { 1, 0, 0, 0 },
                        { 0, 1, 0, 0 },
                        { 0, 0, 1, 0 },
                        { 0, 0, 0, 1 }
                }};
            }
        }

        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 m = new Matrix4x4 { m = new float[4, 4]};
            int i, j, k;
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    for (k = 0; k < 4; k++)
                    {
                        m.m[i,j] += a.m[i,k] * b.m[k,j];
                    }
                }
            }
            return m;
        }


        public static Vector4F operator *(Vector4F v, Matrix4x4 m)
        {
            float[] product = new float[4];
            int i;
            for (i = 0; i < 4; i++)
            {
                float a = m.m[i, 0] * v.x;
                float b = m.m[i, 1] * v.y;
                float c = m.m[i, 2] * v.z;
                float d = m.m[i, 3] * v.w;
                product[i] = a + b + c + d;
            }
            return new Vector4F(product[0], product[1], product[2], product[3]);
        }

        public static Matrix4x4 mat4_lookat(Vector3F eye, Vector3F target, Vector3F up)
        {
            Vector3F z_axis = (eye - target).Normalize();
            Vector3F x_axis = Vector3F.Cross(up, z_axis).Normalize();
            Vector3F y_axis = Vector3F.Cross(z_axis, x_axis);
            Matrix4x4 m = Matrix4x4.Identity;

            m.m[0,0] = x_axis.x;
            m.m[0,1] = x_axis.y;
            m.m[0,2] = x_axis.z;

            m.m[1,0] = y_axis.x;
            m.m[1,1] = y_axis.y;
            m.m[1,2] = y_axis.z;

            m.m[2,0] = z_axis.x;
            m.m[2,1] = z_axis.y;
            m.m[2,2] = z_axis.z;

            m.m[0,3] = -Vector3F.Dot(x_axis, eye);
            m.m[1,3] = -Vector3F.Dot(y_axis, eye);
            m.m[2,3] = -Vector3F.Dot(z_axis, eye);

            return m;
        }

        public static Matrix4x4 mat4_translate(float tx, float ty, float tz)
        {
            var m = Identity;
            m.m[0,3] = tx;
            m.m[1,3] = ty;
            m.m[2,3] = tz;
            return m;
        }

        public static Matrix4x4 mat4_rotate_y(float angle)
        {
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);
            var m = Identity;
            m.m[0,0] = c;
            m.m[0,2] = s;
            m.m[2,0] = -s;
            m.m[2,2] = c;
            return m;
        }

        public static Matrix4x4 mat4_scale(float sx, float sy, float sz)
        {
            var m = Identity;
            m.m[0,0] = sx;
            m.m[1,1] = sy;
            m.m[2,2] = sz;
            return m;
        }

        public static Matrix4x4 mat4_perspective(float fovy, float aspect, float near, float far)
        {
            float z_range = far - near;
            Matrix4x4 m = Matrix4x4.Identity;
            m.m[1,1] = 1 / (float)Math.Tan(fovy / 2);
            m.m[0,0] = m.m[1,1] / aspect;
            m.m[2,2] = -(near + far) / z_range;
            m.m[2,3] = -2 * near * far / z_range;
            m.m[3,2] = -1;
            m.m[3,3] = 0;
            return m;
        }

    }

    public struct Matrix3x3
    {
        public float[,] m => new float[3, 3];
    }
}