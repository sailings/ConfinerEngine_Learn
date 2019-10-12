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
        public Vector3F UP = new Vector3F(0, 1, 0);

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
    }

}
