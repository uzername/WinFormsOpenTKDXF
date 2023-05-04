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

namespace WinFormsOpenTKDXF
{
    public partial class UserControlWinFormsDXF : UserControl
    {
        public GLControl glControl;
        public UserControlWinFormsDXF()
        {
            InitializeComponent();
            glControl = new GLControl();
            glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl.APIVersion = new Version("3.3.0.0");
            glControl.Flags = ContextFlags.Default;
            glControl.Profile = ContextProfile.Core;
            
        }
    }
}
