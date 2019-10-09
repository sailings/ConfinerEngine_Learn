using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FileFormatWavefront;
using FileFormatWavefront.Model;

namespace ConfinerEngine
{
    public partial class RenderForm : Form
    {
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
            int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000
        }

        private GraphicBuffer graphicBuffer;
        private Graphics formGraphic;
        DirectBitmap dbm;

        IntPtr pTarget = IntPtr.Zero;
        IntPtr pSource = IntPtr.Zero;
        IntPtr pOrig = IntPtr.Zero;

        FileLoadResult<Scene> model;

        public RenderForm()
        {
            InitializeComponent();
            formGraphic = CreateGraphics();
            graphicBuffer = new GraphicBuffer(Width, Height);
            dbm = new DirectBitmap(Width, Height);

            pTarget = formGraphic.GetHdc();
            pSource = CreateCompatibleDC(pTarget);

            //model = FileFormatObj.Load(@"Assets\phoenix\body.obj",false);
            model = FileFormatObj.Load(@"Assets\phoenix\123.obj", false);
        }

        private bool rotated = false;

        public void Render()
        {
            pOrig = SelectObject(pSource, dbm.Bitmap.GetHbitmap());

            dbm.Clear(Color.LightYellow);

            //dbm.DrawTriangle(new Point(Width / 2, Height / 4), new Point(Width / 4, Height / 2), new Point(Width * 3 / 4, Height / 2), Color.Red);
            //dbm.DrawLine(0, 0, 100, 200, Color.Green);
            //dbm.DrawLine(300, 400, 10, 60, Color.Yellow);

            for (int i = 0; i < model.Model.Groups[0].Faces.Count; i++)
            {
                var p0 = model.Model.Vertices[model.Model.Groups[0].Faces[i].Indices[0].vertex];
                var p1 = model.Model.Vertices[model.Model.Groups[0].Faces[i].Indices[1].vertex];
                var p2 = model.Model.Vertices[model.Model.Groups[0].Faces[i].Indices[2].vertex];

                var sp0 = new Point((int)((p0.x + 1.0f) * Width / 2.0f), (int)((p0.y + 1.0f) * Height / 2.0f));
                var sp1 = new Point((int)((p1.x + 1.0f) * Width / 2.0f), (int)((p1.y + 1.0f) * Height / 2.0f));
                var sp2 = new Point((int)((p2.x + 1.0f) * Width / 2.0f), (int)((p2.y + 1.0f) * Height / 2.0f));

                //dbm.DrawTriangleWire(sp0, sp1, sp2, Color.Black);
                dbm.DrawTriangle(sp0, sp1, sp2, Color.Black);
            }

            if (!rotated)
            {
                dbm.Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                rotated = true;
            }
            graphicBuffer.SwapBuffer();

            BitBlt(pTarget, 0, 0, dbm.Width, dbm.Height, pSource, 0, 0, TernaryRasterOperations.SRCCOPY);
            DeleteObject(pOrig);
        }
    }
}