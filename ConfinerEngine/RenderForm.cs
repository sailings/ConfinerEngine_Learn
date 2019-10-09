using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ConfinerEngine
{
    public partial class RenderForm : Form
    {
        private GraphicBuffer graphicBuffer;
        private Graphics formGraphic;
        DirectBitmap dbm;

        public RenderForm()
        {
            InitializeComponent();
            formGraphic = CreateGraphics();
            graphicBuffer = new GraphicBuffer(Width, Height);
            dbm = new DirectBitmap(Width, Height);
        }
      
        public void Render()
        {
            dbm.Clear(Color.LightYellow);

            dbm.DrawTriangle(new Point(Width / 2, Height / 4), new Point(Width / 4, Height / 2), new Point(Width * 3 / 4, Height / 2), Color.Red);
            dbm.DrawLine(0, 0, 100, 200, Color.Green);
            dbm.DrawLine(300, 400, 10, 60, Color.Yellow);

            graphicBuffer.SwapBuffer();
            formGraphic.DrawImage(dbm.Bitmap, new PointF(0, 0));
        }
    }
}