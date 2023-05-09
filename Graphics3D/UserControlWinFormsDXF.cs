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
        // vertices of grid
        private  float[] _verticesGrid;
        // This array controls how the EBO will use those vertices to create LINES
        private uint[] _indicesGrid;
        private Shader _shader;

        // We need an instance of the new camera class so it can manage the view and projection matrix code.
        // We also need a boolean set to true to detect whether or not the mouse has been moved for the first time.
        // Finally, we add the last position of the mouse so we can calculate the mouse offset easily.        
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        /* By convention, OpenGL is a right-handed system. What this basically says is that the positive x-axis is to your right, 
         * the positive y-axis is up and the positive z-axis is backwards. 
         * Think of your screen being the center of the 3 axes and the positive z-axis going through your screen towards you.*/

        // These are the handles to OpenGL objects. A handle is an integer representing where the object lives on the
        // graphics card. Consider them sort of like a pointer; we can't do anything with them directly, but we can
        // send them to OpenGL functions that need them.
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        // Add a handle for the EBO
        private int _elementBufferObject;

        /// <summary>
        /// fill in _verticesGrid
        /// </summary>
        /// <param name="szX">number of cells on X</param>
        /// <param name="szY">number of cells on Y</param>
        /// <param name="dimGrid">size of cell in grid</param>
        private void generateVerticesGrid(int szX, int szY, double dimGrid)
        {
            //four lines: two points for each : three numbers for each
            // but - only four vertices, three numbers each!
            _verticesGrid = new float[4 * 3];

            _verticesGrid[0] = (float) ((dimGrid * szX)/2.0);
            _verticesGrid[1] = 0f;
            _verticesGrid[2] = (float)((dimGrid * szX) / 2.0);

            _verticesGrid[3] = (float)((dimGrid * szX) / 2.0);
            _verticesGrid[4] = 0f;
            _verticesGrid[5] = -(float)((dimGrid * szX) / 2.0);

            _verticesGrid[6] = -(float)((dimGrid * szX) / 2.0);
            _verticesGrid[7] = 0f;
            _verticesGrid[8] = -(float)((dimGrid * szX) / 2.0);

            _verticesGrid[9] = -(float)((dimGrid * szX) / 2.0);
            _verticesGrid[10] = 0f;
            _verticesGrid[11] = (float)((dimGrid * szX) / 2.0);

            // Lines: 1-2, 2-3, 3-4, 4-1
            _indicesGrid = new uint[4 * 2];
            _indicesGrid[0] = 1; _indicesGrid[1] = 2;
            _indicesGrid[2] = 2; _indicesGrid[3] = 3;
            _indicesGrid[4] = 3; _indicesGrid[5] = 4;
            _indicesGrid[6] = 4; _indicesGrid[7] = 1;
        }
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
            // We enable depth testing here. If you try to draw something more complex than one plane without this,
            // you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            // Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);


            // We need to send our vertices over to the graphics card so OpenGL can use them.
            // To do this, we need to create what's called a Vertex Buffer Object (VBO).
            // These allow you to upload a bunch of data to a buffer, and send the buffer to the graphics card.
            // This effectively sends all the vertices at the same time.
            // First, we need to create a buffer. This function returns a handle to it, but as of right now, it's empty.
            _vertexBufferObject = GL.GenBuffer();
            // Finally, upload the vertices to the buffer.
            // Arguments:
            //   Which buffer the data should be sent to.
            //   How much data is being sent, in bytes. You can generally set this to the length of your array, multiplied by sizeof(array type).
            //   The vertices themselves.
            //   How the buffer will be used, so that OpenGL can write the data to the proper memory space on the GPU.
            //   There are three different BufferUsageHints for drawing:
            //     StaticDraw: This buffer will rarely, if ever, update after being initially uploaded.
            //     DynamicDraw: This buffer will change frequently after being initially uploaded.
            //     StreamDraw: This buffer will change on every frame.
            //   Writing to the proper memory space is important! Generally, you'll only want StaticDraw,
            //   but be sure to use the right one for your use case.
            // https://stackoverflow.com/questions/15821969/what-is-the-proper-way-to-modify-opengl-vertex-buffer
            GL.BufferData(BufferTarget.ArrayBuffer, _verticesGrid.Length * sizeof(float), _verticesGrid, BufferUsageHint.StaticDraw);
            // One notable thing about the buffer we just loaded data into is that it doesn't have any structure to it. It's just a bunch of floats (which are actaully just bytes).
            // The opengl driver doesn't know how this data should be interpreted or how it should be divided up into vertices. To do this opengl introduces the idea of a 
            // Vertex Array Obejct (VAO) which has the job of keeping track of what parts or what buffers correspond to what data. In this example we want to set our VAO up so that 
            // it tells opengl that we want to interpret 12 bytes as 3 floats and divide the buffer into vertices using that.
            // To do this we generate and bind a VAO (which looks deceptivly similar to creating and binding a VBO, but they are different!).
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // Now, we need to setup how the vertex shader will interpret the VBO data; you can send almost any C datatype (and a few non-C ones too) to it.
            // While this makes them incredibly flexible, it means we have to specify how that data will be mapped to the shader's input variables.

            // To do this, we use the GL.VertexAttribPointer function
            // This function has two jobs, to tell opengl about the format of the data, but also to associate the current array buffer with the VAO.
            // This means that after this call, we have setup this attribute to source data from the current array buffer and interpret it in the way we specified.
            // Arguments:
            //   Location of the input variable in the shader. the layout(location = 0) line in the vertex shader explicitly sets it to 0.
            //   How many elements will be sent to the variable. In this case, 3 floats for every vertex.
            //   The data type of the elements set, in this case float.
            //   Whether or not the data should be converted to normalized device coordinates. In this case, false, because that's already done.
            //   The stride; this is how many bytes are between the last element of one vertex and the first element of the next. 3 * sizeof(float) in this case.
            //   The offset; this is how many bytes it should skip to find the first element of the first vertex. 0 as of right now.
            // Stride and Offset are just sort of glossed over for now, but when we get into texture coordinates they'll be shown in better detail.
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            // Enable variable 0 in the shader.
            GL.EnableVertexAttribArray(0);

            // We create/bind the Element Buffer Object EBO the same way as the VBO, except there is a major difference here which can be REALLY confusing.
            // The binding spot for ElementArrayBuffer is not actually a global binding spot like ArrayBuffer is. 
            // Instead it's actually a property of the currently bound VertexArrayObject, and binding an EBO with no VAO is undefined behaviour.
            // This also means that if you bind another VAO, the current ElementArrayBuffer is going to change with it.
            // Another sneaky part is that you don't need to unbind the buffer in ElementArrayBuffer as unbinding the VAO is going to do this,
            // and unbinding the EBO will remove it from the VAO instead of unbinding it like you would for VBOs or VAOs.
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            // We also upload data to the EBO the same way as we did with VBOs.
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indicesGrid.Length * sizeof(uint), _indicesGrid, BufferUsageHint.StaticDraw);

            // We've got the vertices done, but how exactly should this be converted to pixels for the final image?
            // Modern OpenGL makes this pipeline very free, giving us a lot of freedom on how vertices are turned to pixels.
            // The drawback is that we actually need two more programs for this! These are called "shaders".
            // Shaders are tiny programs that live on the GPU. OpenGL uses them to handle the vertex-to-pixel pipeline.
            // Check out the Shader class in Common to see how we create our shaders, as well as a more in-depth explanation of how shaders work.
            // shader.vert and shader.frag contain the actual shader code.
            _shader = new Shader("Shaders/shaderGrid.vert", "Shaders/shaderGrid.frag");

            // Now, enable the shader.
            // Just like the VBO, this is global, so every function that uses a shader will modify this one until a new one is bound instead.
            _shader.Use();

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // Bind the shader
            _shader.Use();

            glControl1.SwapBuffers();
        }

    }
}
