using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfinerEngine
{
    public class Motion
    {
        public Vector2F orbit { get; set; }
        public Vector2F pan { get; set; }
        public float dolly { get; set; }

        public void Reset()
        {
            dolly = 0;
            orbit = new Vector2F(0, 0);
            pan = new Vector2F(0, 0);
        }
    }
}