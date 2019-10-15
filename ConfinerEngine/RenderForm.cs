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

        private Motion motion = new Motion();

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

        FileLoadResult<Scene> model1;
        FileLoadResult<Scene> model2;

        private Matrix4x4 modelMatrix = Matrix4x4.Identity;
        private Matrix4x4 cameraVPMatrix = Matrix4x4.Identity;
        private Camera camera = new Camera();

        private int renderWidth, renderHeight;
        Image imageBody = new Image();
        Texture textureBody = new Texture();

        Image imageWings = new Image();
        Texture textureWings = new Texture();

        PointF mPos;
        bool altKeyDown = false;

        public RenderForm()
        {
            InitializeComponent();
            formGraphic = panel1.CreateGraphics();
            renderWidth = panel1.Width;
            renderHeight = panel1.Height;
            panel1.MouseWheel += Panel1_MouseWheel;
            graphicBuffer = new GraphicBuffer(renderWidth, renderHeight);

            dbm = new DirectBitmap(renderWidth, renderHeight);

            pTarget = formGraphic.GetHdc();
            pSource = CreateCompatibleDC(pTarget);

            model1 = FileFormatObj.Load(@"Assets\phoenix\body.obj",false);
            model2 = FileFormatObj.Load(@"Assets\phoenix\wings.obj", false);
            imageBody =Image.Load_Tga(@"Assets\phoenix\body_diffuse.tga");
            textureBody = Texture.Texture_From_Image(imageBody);

            imageWings = Image.Load_Tga(@"Assets\phoenix\wings_diffuse.tga");
            textureWings = Texture.Texture_From_Image(imageWings);

            camera.Position = new Vector3F(0, 0, 1.5f);
            camera.Target = new Vector3F(0, 0, 0);
            camera.Aspect = renderWidth * 1.0f / renderHeight;

            cameraVPMatrix = camera.camera_get_proj_matrix() * camera.camera_get_view_matrix();

            var translation = Matrix4x4.mat4_translate(376.905f, -169.495f, 0);
            var rotation = Matrix4x4.mat4_rotate_y((float)Math.PI);
            var scale = Matrix4x4.mat4_scale(0.001f, 0.001f, 0.001f);
            modelMatrix = scale * (rotation * translation);
        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            motion.dolly += e.Delta/120.0f;
        }
       
        private void UpdateCamera()
        {
            Vector3F from_target = camera.Position - camera.Target;
            Vector3F from_camera = camera.Target - camera.Position;
            Vector3F pan = Camera.calculate_pan(from_camera, motion);
            Vector3F offset = Camera.calculate_offset(from_target, motion);
            camera.Target = camera.Target + pan;
            camera.Position = camera.Target + offset;

            cameraVPMatrix = camera.camera_get_proj_matrix() * camera.camera_get_view_matrix();

            txtCTx.Text = camera.Target.x.ToString();
            txtCTy.Text = camera.Target.y.ToString();
            txtCTz.Text = camera.Target.z.ToString();

            txtCPx.Text = camera.Position.x.ToString();
            txtCPy.Text = camera.Position.y.ToString();
            txtCPz.Text = camera.Position.z.ToString();
        }

        private void DrawModel(FileLoadResult<Scene> model, Texture texture, DirectBitmap dbm)
        {
            for (int i = 0; i < model.Model.UngroupedFaces.Count; i++)
            {
                var p0 = model.Model.Vertices[model.Model.UngroupedFaces[i].Indices[0].vertex];
                var p1 = model.Model.Vertices[model.Model.UngroupedFaces[i].Indices[1].vertex];
                var p2 = model.Model.Vertices[model.Model.UngroupedFaces[i].Indices[2].vertex];

                var cp0 = Vector4F.Vec4_From_Vec3(new Vector3F(p0.x, p0.y, p0.z), 1) * modelMatrix * cameraVPMatrix;
                var cp1 = Vector4F.Vec4_From_Vec3(new Vector3F(p1.x, p1.y, p1.z), 1) * modelMatrix * cameraVPMatrix;
                var cp2 = Vector4F.Vec4_From_Vec3(new Vector3F(p2.x, p2.y, p2.z), 1) * modelMatrix * cameraVPMatrix;

                var ndcP0 = Vector3F.Vec3_From_Vec4(cp0) / cp0.w;
                var ndcP1 = Vector3F.Vec3_From_Vec4(cp1) / cp1.w;
                var ndcP2 = Vector3F.Vec3_From_Vec4(cp2) / cp2.w;

                if (Vector3F.is_back_facing(ndcP0, ndcP1, ndcP2))
                    continue;

                var sp0 = new Vector3F((int)((ndcP0.x + 1.0f) * renderWidth * 0.5f), (int)((ndcP0.y + 1.0f) * renderHeight * 0.5f), (ndcP0.z + 1) * 0.5f);
                var sp1 = new Vector3F((int)((ndcP1.x + 1.0f) * renderWidth * 0.5f), (int)((ndcP1.y + 1.0f) * renderHeight * 0.5f), (ndcP1.z + 1) * 0.5f);
                var sp2 = new Vector3F((int)((ndcP2.x + 1.0f) * renderWidth * 0.5f), (int)((ndcP2.y + 1.0f) * renderHeight * 0.5f), (ndcP2.z + 1) * 0.5f);

                var uv0 = model.Model.Uvs[model.Model.UngroupedFaces[i].Indices[0].uv.Value];
                var uv1 = model.Model.Uvs[model.Model.UngroupedFaces[i].Indices[1].uv.Value];
                var uv2 = model.Model.Uvs[model.Model.UngroupedFaces[i].Indices[2].uv.Value];

                CEVertex v0 = new CEVertex { Position = sp0, UV = new Vector2F(uv0.u, uv0.v) };
                CEVertex v1 = new CEVertex { Position = sp1, UV = new Vector2F(uv1.u, uv1.v) };
                CEVertex v2 = new CEVertex { Position = sp2, UV = new Vector2F(uv2.u, uv2.v) };

                //dbm.DrawTriangleWire(sp0, sp1, sp2, Color.Gray);
                dbm.DrawTriangle(texture, v0, v1, v2, Color.Gray, new Vector4F[] { cp0, cp1, cp2 });
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && altKeyDown)
            {
                var pos_delta = new Vector2F(e.Location.X - mPos.X, e.Location.Y - mPos.Y) / Width;
                motion.orbit = motion.orbit + pos_delta;
                mPos = e.Location;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                var pos_delta = new Vector2F(e.Location.X - mPos.X, e.Location.Y - mPos.Y) / Width;
                motion.pan = motion.pan + pos_delta;
                mPos = e.Location;
            }
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mPos = e.Location;
        }

        private void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            altKeyDown = e.Alt;
        }

        private void RenderForm_KeyUp(object sender, KeyEventArgs e)
        {
            altKeyDown = e.Alt;
        }

        public void Render()
        {
            dbm = new DirectBitmap(renderWidth, renderHeight);
            UpdateCamera();
            dbm.Clear(Color.LightYellow);

            //dbm.DrawTriangle(new Point(Width / 2, Height / 4), new Point(Width / 4, Height / 2), new Point(Width * 3 / 4, Height / 2), Color.Red);
            //dbm.DrawLine(0, 0, 100, 200, Color.Green);
            //dbm.DrawLine(300, 400, 10, 60, Color.Yellow);

            DrawModel(model1, textureBody, dbm);
            DrawModel(model2, textureWings, dbm);

            dbm.Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            pOrig = SelectObject(pSource, dbm.Bitmap.GetHbitmap());
                
            graphicBuffer.SwapBuffer();

            BitBlt(pTarget, 0, 0, dbm.Width, dbm.Height, pSource, 0, 0, TernaryRasterOperations.SRCCOPY);
            DeleteObject(pOrig);

            dbm.Dispose();

            motion.Reset();
        }
    }
}