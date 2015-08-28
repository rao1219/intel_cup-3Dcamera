using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;
//using CsGL.OpenGL;

namespace _3D_ver03
{
    /// <summary>
    /// CameraWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CameraWindow : UserControl
    {
        private ExternalFunctions ex;
        public CameraWindow()
        {
            InitializeComponent();
            ex = new ExternalFunctions();
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL instance that's been passed to us.
            ExternalFunctions.SetWindowsSize((int)this.ActualWidth, (int)this.ActualHeight);
            ExternalFunctions.OnPaint();
        }
    }

    /*private class NeededClass : CsGL.OpenGL.OpenGLControl
    {
        protected override void OnCreateControl() 
        { 
            GL.glClearDepth(1.0f); 
            GL.glDepthFunc(GL.GL_LEQUAL); 
            GL.glEnable(GL.GL_DEPTH_TEST); 
        }
        protected override void OnSizeChanged(EventArgs e) 
        { 
            base.OnSizeChanged(e); 
            Size s = Size; 
            double aspect_ratio = (double)s.Width / (double)s.Height; 
            ViewCtrl.VIEWSIZE_WIDTH = s.Width; 
            ViewCtrl.VIEWSIZE_HEIGHT = s.Height; 
            GL.glMatrixMode(GL.GL_PROJECTION); 
            GL.glLoadIdentity(); 
            GL.gluPerspective(63.0f, aspect_ratio, 1f, 4000.0f); 
            GL.glMatrixMode(GL.GL_MODELVIEW); GL.glLoadIdentity(); 
        }
        public override void glDraw() 
        { 
            //GL.glClearColor(0.1f, 0.1f, 0.2f, 1.0f); 
            //GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT); GL.glLoadIdentity(); 
            CsGL.OpenGL.OpenGL.glClear(CsGL.OpenGL.OpenGL.GL_COLOR_BUFFER_BIT | CsGL.OpenGL.OpenGL.GL_DEPTH_BUFFER_BIT);
            CsGL.OpenGL.OpenGL.glBegin(CsGL.OpenGL.OpenGL.GL_LINES);
            CsGL.OpenGL.OpenGL.glColor3f(1, 0, 0);
            CsGL.OpenGL.OpenGL.glVertex3f(1, 1, 1);
            CsGL.OpenGL.OpenGL.glVertex3f(0, 0, 0);
            CsGL.OpenGL.OpenGL.glEnd();
            CsGL.OpenGL.OpenGL.glFlush();
        }
    }*/
}
