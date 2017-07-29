using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace MarioKartTrackMaker
{
    public partial class ViewPortPanel : GLControl
    {
        public ViewPortPanel()
        {
            InitializeComponent();
        }
        bool loaded = false;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                GL.ClearColor(Color.Black);
                loaded = true;
            }
        }
        int mx, my = 0;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!DesignMode)
            {
                Console.WriteLine("...");
                Matrix4 mtx = Matrix4.CreateOrthographic(Width, Height, 0.01F, 10F);
                GL.LoadMatrix(ref mtx);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.Begin(PrimitiveType.Polygon);
                GL.Color3(Color.Red);
                for (int i = 0; i < 360; i += 60)
                {
                    GL.Vertex3(Math.Sin(i * Math.PI / 180) * 30 + mx - Width / 2, Math.Cos(i * Math.PI / 180) * 30 - my + Height / 2, -5);
                }
                GL.End();
                GL.Begin(PrimitiveType.Polygon);
                GL.Color3(Color.White);
                for (int i = 0; i < 360; i += 60)
                {
                    GL.Vertex3(Math.Sin(i * Math.PI / 180) * 20 + mx - Width / 2, Math.Cos(i * Math.PI / 180) * 20 - my + Height / 2, -5);
                }
                GL.End();
                SwapBuffers();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mx = e.X;
            my = e.Y;
            Invalidate();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!DesignMode && loaded)
            {
                GL.Viewport(0, 0, Width, Height); 
            }
            Invalidate();
        }
    }
}
