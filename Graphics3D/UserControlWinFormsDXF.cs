using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsOpenTKDXF.Graphics3D;

namespace WinFormsOpenTKDXF
{
    public partial class UserControlWinFormsDXF : UserControl
    {
        private System.Windows.Forms.Timer _timer = null!;
        // I start with simplest grid now

        // We need an instance of the new camera class so it can manage the view and projection matrix code.
        // We also need a boolean set to true to detect whether or not the mouse has been moved for the first time.
        // Finally, we add the last position of the mouse so we can calculate the mouse offset easily.
        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;


        public UserControlWinFormsDXF()
        {
            InitializeComponent();

            
        }
        private void scenePreInit()
        {
            // This will be the color of the background after we clear it, in normalized colors.
            // Normalized colors are mapped on a range of 0.0 to 1.0, with 0.0 representing black, and 1.0 representing
            // the largest possible value for that channel.            
            GL.ClearColor(0.2f, 0.7f, 0.9f, 1.0f);

            // We initialize the camera so that it is 3 units back from zero
            // We also give it the proper aspect ratio.
            double SizeX = glControl1.ClientSize.Width; double SizeY = glControl1.ClientSize.Height;
            _camera = new Camera(Vector3.UnitZ * 3, (float) SizeX / (float)SizeY);

        }
        private void glControl1_Load(object? sender, EventArgs e)
        {
            scenePreInit();
            // Make sure that when the GLControl is resized or needs to be painted,
            // we update our projection matrix or re-render its contents, respectively.
            glControl1.Resize += glControl1_Resize;
            glControl1.Paint += glControl1_Paint;

            // Redraw the screen every 1/20 of a second.
            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += (sender, e) =>
            {
             
                Render();
            };
            _timer.Interval = 50;   // 1000 ms per sec / 50 ms per frame = 20 FPS
            _timer.Start();

            // Ensure that the viewport and projection matrix are set correctly initially.
            glControl1_Resize(glControl1, EventArgs.Empty);
        }

        private void glControl1_Resize(object? sender, EventArgs e)
        {
            glControl1.MakeCurrent();

            if (glControl1.ClientSize.Height == 0)
                glControl1.ClientSize = new System.Drawing.Size(glControl1.ClientSize.Width, 1);

            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            glControl1.MakeCurrent();

            // This clears the image, using what you set as GL.ClearColor earlier.
            // OpenGL provides several different types of data that can be rendered.
            // You can clear multiple buffers by using multiple bit flags.
            // However, we only modify the color, so ColorBufferBit is all we need to clear.
            GL.Clear(ClearBufferMask.ColorBufferBit);


            glControl1.SwapBuffers();
        }

    }
}
