using Microsoft.VisualBasic.Devices;
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
        private readonly float[] _vertices =
       {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };
        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        private int _elementBufferObject;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;
        private Texture _texture;
        private Texture _texture2;
        // We need an instance of the new camera class so it can manage the view and projection matrix code.
        // We also need a boolean set to true to detect whether or not the mouse has been moved for the first time.
        // Finally, we add the last position of the mouse so we can calculate the mouse offset easily.
        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private double _time;
        public UserControlWinFormsDXF()
        {
            InitializeComponent();

            
        }
        private void scenePreInit()
        {            
            GL.ClearColor(0.2f, 0.7f, 0.9f, 1.0f);
            
            GL.Enable(EnableCap.DepthTest);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Graphics3D/Shaders/shader.vert", "Graphics3D/Shaders/shader.frag");
            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _texture = Texture.LoadFromFile("Graphics3D/Resources/container.png");
            _texture.Use(TextureUnit.Texture0);

            _texture2 = Texture.LoadFromFile("Graphics3D/Resources/awesomeface.png");
            _texture2.Use(TextureUnit.Texture1);

            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);

            // We initialize the camera so that it is 3 units back from where the rectangle is.
            // We also give it the proper aspect ratio.
            double SizeX = this.glControl1.Size.Width;
            double SizeY = this.glControl1.Size.Height;
            _camera = new Camera(Vector3.UnitZ * 3, (float)SizeX / (float)SizeY);
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
            if (glControl1.IsDisposed) return;
            glControl1.MakeCurrent();

            // This clears the image, using what you set as GL.ClearColor earlier.
            // OpenGL provides several different types of data that can be rendered.
            // You can clear multiple buffers by using multiple bit flags.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use(TextureUnit.Texture0);
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            glControl1.SwapBuffers();
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            const float cameraSpeed = 1.5f;

            if (e.KeyCode == (Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed; // Forward
            }

            if (e.KeyCode == (Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed; // Backwards
            }
            if (e.KeyCode == (Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed; // Left
            }
            if (e.KeyCode == (Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed; // Right
            }
            if (e.KeyCode == (Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed; // Up
            }
            if (e.Shift)
            {
                _camera.Position -= _camera.Up * cameraSpeed; // Down
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            const float sensitivity = 0.2f;
            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(e.X, e.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = e.X - _lastPos.X;
                var deltaY = e.Y - _lastPos.Y;
                _lastPos = new Vector2(e.X, e.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }
    }
}
