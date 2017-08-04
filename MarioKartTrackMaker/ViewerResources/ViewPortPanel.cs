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
using MarioKartTrackMaker;
using MarioKartTrackMaker.IO;
namespace MarioKartTrackMaker.ViewerResources
{
    public partial class ViewPortPanel : GLControl
    {
        public ViewPortPanel(OpenTK.Graphics.GraphicsMode mode) : base(mode)
        {
            InitializeComponent();
        }
        public ViewPortPanel()
        {
            InitializeComponent();
        }
        bool loaded = false;
        private bool _wf;
        public bool wireframemode { get { return _wf; } set { _wf = value; Invalidate(); } }
        private int _coll = 1;
        public int collisionviewmode { get { return _coll; } set { _coll = value; Invalidate(); } }
        public Camera cam;
        private int _fs, _vs, _pgm;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cam = new Camera(new Vector3(0F,0F,-10F), new Vector3(0F, 0F, 0F), this);
            cam.position = new Vector3(100f, -120f, 200f);
            if (!DesignMode)
            {
                GL.ClearColor(Color.Black);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Multisample);
                GL.Enable(EnableCap.CullFace);
                _fs = Shader.CompileFragmentShader();
                _vs = Shader.CompileVertexShader();
                _pgm = Shader.ProgramLink(_vs, _fs);
                loaded = true;
            }
        }
        private int mx, my = 0;

        //Schedule for Importing
        private List<string> _ObjectsToLoad = new List<string>();
        public void InsertObjects(string filepath) {
            _ObjectsToLoad.Add(filepath);
        }
        Random r = new Random();
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DateTime t = DateTime.Now.AddSeconds(1 / 60.0);
            if (Forward)
            {
                Vector3 vel = new Vector3(0, 0, -cam.zoom / 150F);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (Backward)
            {
                Vector3 vel = new Vector3(0, 0, cam.zoom / 150F);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (SideLeft)
            {
                Vector3 vel = new Vector3(-cam.zoom / 150F, 0, 0);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (SideRight)
            {
                Vector3 vel = new Vector3(cam.zoom / 150F, 0, 0);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (!DesignMode)
            {
                //Import Objects if necessary
                if (_ObjectsToLoad.Count > 0)
                {
                    foreach (string objectpath in _ObjectsToLoad)
                    {
                        Object3D obj = new Object3D(objectpath);
                        obj.position = new Vector3((float)(r.NextDouble() - 0.5) * 100000, (float)(r.NextDouble() - 0.5) * 100000, (float)(r.NextDouble() - 0.5) * 100000);
                        Object3D.database.Add(obj);
                    }
                   _ObjectsToLoad = new List<string>();
                }
                //End of Import
                Matrix4 mtx = cam.matrix;
                GL.LoadMatrix(ref mtx);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                for (float i = -100; i <= 100; i += 10)
                {
                    GL.Color3(Color.Green);
                    GL.LineWidth(0.5F);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex3(i, 100, 0);
                    GL.Vertex3(i, -100, 0);
                    GL.End();
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex3(100,i, 0);
                    GL.Vertex3(-100,i, 0);
                    GL.End();
                }
                GL.LineWidth(1F);
                /*GL.Begin(PrimitiveType.Polygon);
                GL.Color3(Color.Red);
                for (int i = 0; i < 360; i += 60)
                {
                    GL.Vertex3(Math.Sin(i * Math.PI / 180) * 30 + mx - Width / 2, Math.Cos(i * Math.PI / 180) * 30 - my + Height / 2, -8);
                }
                GL.End();
                GL.Begin(PrimitiveType.Polygon);
                GL.Color3(Color.White);
                for (int i = 0; i < 360; i += 60)
                {
                    GL.Vertex3(Math.Sin(i * Math.PI / 180) * 20 + mx - Width / 2, Math.Cos(i * Math.PI / 180) * 20 - my + Height / 2, -5);
                }
                GL.End();*/
                foreach (Object3D obj in Object3D.database)
                {
                    obj.DrawObject(_pgm, _coll, _wf);
                }
                SwapBuffers();
            }
            if (Forward || Backward || SideLeft || SideRight)
            {
                while (DateTime.Now < t) {}
                Invalidate();
            }
        }
        Point _prev = new Point();
        bool Forward, Backward, SideLeft, SideRight = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Forward |= (e.KeyCode == Keys.W);
            Backward |= (e.KeyCode == Keys.S);
            SideLeft |= (e.KeyCode == Keys.A);
            SideRight |= (e.KeyCode == Keys.D);
            Invalidate();
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Forward &= !(e.KeyCode == Keys.W);
            Backward &= !(e.KeyCode == Keys.S);
            SideLeft &= !(e.KeyCode == Keys.A);
            SideRight &= !(e.KeyCode == Keys.D);
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _prev = e.Location;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            cam.zoom = Math.Max(1F, cam.zoom+e.Delta/12F* (float)Math.Pow(cam.zoom, 0.375F));
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mx = e.X;
            my = e.Y;
            if (e.Button == MouseButtons.Right)
            {
                if ((ModifierKeys & Keys.Shift) != 0)
                {
                    Vector3 vel = new Vector3(-(e.Location.X - _prev.X) * cam.zoom / 250F, (e.Location.Y - _prev.Y) * cam.zoom / 250F, 0);
                    Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                    cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                    cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                    _prev = e.Location;
                }
                else
                {
                    Vector3 vel = new Vector3(-(e.Location.X - _prev.X) * cam.zoom / 100F, (e.Location.Y - _prev.Y) * cam.zoom / 100F, 0);
                    Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                    cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                    _prev = e.Location;
                }
            }
            Invalidate();
        }

        private void ViewPortPanel_Load(object sender, EventArgs e)
        {

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
