using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfinerEngine
{
    public class GraphicBuffer
    {
        public Bitmap Current { get; private set; }
        public Graphics CurrentGraphic { get; private set; }
        public Bitmap BackGround { get; private set; }
        public Graphics BackGraphic { get; private set; }

        public GraphicBuffer(int width, int height)
        {
            Current = new Bitmap(width, height);
            CurrentGraphic = Graphics.FromImage(Current);
            BackGround = new Bitmap(width, height);
            BackGraphic = Graphics.FromImage(BackGround);
        }

        public void SwapBuffer()
        {
            var temp = Current;
            Current = BackGround;
            BackGround = temp;
        }
    }
}