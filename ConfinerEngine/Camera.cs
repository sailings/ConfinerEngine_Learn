using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfinerEngine
{
    public class Camera
    {
        //private const float PI = Math.PI;

        public const float NEAR = 0.1f;
        public const float FAR = 10000;
        public const float FOVY = ((float)Math.PI / 180) * 60;
        public static Vector3F UP = new Vector3F(0, 1, 0);

        public Vector3F Position
        {
            get;
            set;
        }

        public Vector3F Target
        {
            get;
            set;
        }
        public float Aspect
        {
            get;
            set;
        }

        public Matrix4x4 camera_get_view_matrix()
        {
            return Matrix4x4.mat4_lookat(Position, Target, UP);
        }

        public Matrix4x4 camera_get_proj_matrix()
        {
            return Matrix4x4.mat4_perspective(FOVY, Aspect, NEAR, FAR);
        }

        public static Vector3F calculate_pan(Vector3F from_camera, Motion motion)
        {
            Vector3F forward = from_camera.Normalize();
            Vector3F left = Vector3F.Cross(UP, forward);
            Vector3F up = Vector3F.Cross(forward, left);

            float distance = from_camera.Length();
            float factor = distance * (float)Math.Tan(FOVY / 2) * 2;
            Vector3F delta_x = left * (motion.pan.x * factor);
            Vector3F delta_y = up *  (motion.pan.y * factor);
            return delta_x + delta_y;
        }

        public static Vector3F calculate_offset(Vector3F from_target, Motion motion)
        {
            float radius = from_target.Length();
            float theta = (float)Math.Atan2(from_target.x, from_target.z);  /* azimuth */
            float phi = (float)Math.Acos(from_target.y / radius);           /* polar */
            float factor = (float)Math.PI * 2;
            Vector3F offset = new Vector3F();
            
            radius *= (float)Math.Pow(0.95, motion.dolly);
            theta -= motion.orbit.x * factor;
            phi -= motion.orbit.y * factor;
            phi = float_clamp(phi, float.Epsilon, (float)Math.PI - float.Epsilon);

            offset.x = radius * (float)Math.Sin(phi) * (float)Math.Sin(theta);
            offset.y = radius * (float)Math.Cos(phi);
            offset.z = radius * (float)Math.Sin(phi) * (float)Math.Cos(theta);

            return offset;
        }

        static float float_clamp(float f, float min, float max)
        {
            return f < min ? min : (f > max ? max : f);
        }
    }
}
