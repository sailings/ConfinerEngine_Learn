using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfinerEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            string fpsText;

            RenderForm renderForm = new RenderForm();
            renderForm.Show();

            while (!renderForm.IsDisposed)
            {
                stopWatch.Start();
                renderForm.Render();
                //Thread.Sleep(10);
                Application.DoEvents();
                stopWatch.Stop();
                fpsText = (1000.0f / stopWatch.ElapsedMilliseconds).ToString("F1");
                renderForm.Text = $"FPS[{fpsText}]";
                stopWatch.Reset();
            }
        }
    }
}