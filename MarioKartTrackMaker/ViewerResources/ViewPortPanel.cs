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
        private Attachment _AttachmentToAttachTo = null;
        public void InsertObjects(string filepath, Attachment attachment) {
            _ObjectsToLoad.Add(filepath);
            _AttachmentToAttachTo = attachment;
        }
        Random r = new Random();
        Vector3 _frompos = new Vector3();
        Vector3 _fromcampos = new Vector3();
        float _fromzoom = 0;
        float _transtime = 0;
        bool _panzoom;
        float _smoothtranstime
        {
            get { return _transtime * _transtime * _transtime * (_transtime * (_transtime * 6 - 15) + 10); }
        }
        Object3D _targetObj;
        public void GoToObject(Object3D obj, bool pan)
        {
            _frompos = cam.pivot;
            _fromcampos = cam.position;
            _panzoom = pan;
            _transtime = 1;
            _targetObj = obj;
            _fromzoom = cam.zoom;
            Invalidate();
        }
        private float lerp(float a,float b, float c)
        {
            return a + (b - a) * c;
        }
        public struct TraceResult
        {
            public bool Hit;
            public Vector3 HitPos;
            public Object3D HitObject;
            public Vector3 HitNormal;
            public TraceResult(bool Hit, Vector3 HitPos, Vector3 HitNormal, Object3D HitObject) : this() {
                this.Hit = Hit;
                this.HitPos = HitPos;
                this.HitObject = HitObject;
                this.HitNormal = HitNormal;
            }
        }
        struct TriangleIntersection
        {
            public bool intersects;
            public float depth;
            public Vector3 normal;

            public TriangleIntersection(bool v) : this()
            {
                intersects = v;
            }

            public TriangleIntersection(bool v, float t, Vector3 n) : this()
            {
                intersects = v;
                depth = t;
                normal = n;
            }
        }
        public struct Ray
        {
            public Vector3 pos;
            public Vector3 dir;
            public Camera camera;
            public Ray(Vector3 pos, Vector3 dir, Camera camera) : this() {
                this.pos = pos;
                this.dir = dir;
                this.camera = camera;
            }
            //Function Taken from technologicalutopia 
            public double? IntersectBounds(Bounds box)
            {
                //first test if start in box
                if (pos.X >= box.minX
                    && pos.X <= box.maxX
                    && pos.Y >= box.minY
                    && pos.Y <= box.maxY
                    && pos.Z >= box.minZ
                    && pos.Z <= box.maxZ)
                    return 0.0f;// here we concidere cube is full and origine is in cube so intersect at origine


                //Second we check each face
                Vector3 maxT = new Vector3(-1.0f);
                //Vector3 minT = new Vector3(-1.0f);
                //calcul intersection with each faces
                if (pos.X < box.minX && dir.X != 0.0f)
                    maxT.X = (box.minX - pos.X) / dir.X;
                else if (pos.X > box.maxX && dir.X != 0.0f)
                    maxT.X = (box.maxX - pos.X) / dir.X;
                if (pos.Y < box.minY && dir.Y != 0.0f)
                    maxT.Y = (box.minY - pos.Y) / dir.Y;
                else if (pos.Y > box.maxY && dir.Y != 0.0f)
                    maxT.Y = (box.maxY - pos.Y) / dir.Y;
                if (pos.Z < box.minZ && dir.Z != 0.0f)
                    maxT.Z = (box.minZ - pos.Z) / dir.Z;
                else if (pos.Z > box.maxZ && dir.Z != 0.0f)
                    maxT.Z = (box.maxZ - pos.Z) / dir.Z;


                //get the maximum maxT
                if (maxT.X > maxT.Y && maxT.X > maxT.Z)
                {
                    if (maxT.X < 0.0f)
                        return null;// ray go on opposite of face
                                    //coordonate of hit point of face of cube
                    double coord = pos.Z + maxT.X * dir.Z;
                    // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                    if (coord < box.minZ || coord > box.maxZ)
                        return null;
                    coord = pos.Y + maxT.X * dir.Y;
                    if (coord < box.minY || coord > box.maxY)
                        return null;
                    return maxT.X;
                }
                if (maxT.Y > maxT.X && maxT.Y > maxT.Z)
                {
                    if (maxT.Y < 0.0f)
                        return null;// ray go on opposite of face
                                    //coordonate of hit point of face of cube
                    double coord = pos.Z + maxT.Y * dir.Z;
                    // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                    if (coord < box.minZ || coord > box.maxZ)
                        return null;
                    coord = pos.X + maxT.Y * dir.X;
                    if (coord < box.minX || coord > box.maxX)
                        return null;
                    return maxT.Y;
                }
                else //Z
                {
                    if (maxT.Z < 0.0f)
                        return null;// ray go on opposite of face
                                    //coordonate of hit point of face of cube
                    double coord = pos.X + maxT.Z * dir.X;
                    // if hit point coord ( intersect face with ray) is out of other plane coord it miss 
                    if (coord < box.minX || coord > box.maxX)
                        return null;
                    coord = pos.Y + maxT.Z * dir.Y;
                    if (coord < box.minY || coord > box.maxY)
                        return null;
                    return maxT.Z;
                }
            }
            public TraceResult Trace(Camera cam)
            {
                float depth = float.PositiveInfinity;
                bool intersects = false;
                Vector3 normal = new Vector3();
                Object3D outobj = null;
                foreach(Object3D obj in Object3D.database)
                {
                    Matrix4 tnsfm = obj.transform;
                    if(Math.Min((obj.position-camera.pivot).Length, 5000F) <= camera.zoom && inSight(obj, cam)) {
                    foreach (Mesh m in obj.model.meshes)
                    {
                        for (int f = 0; f < Math.Min(m.faces.Count, m.fnmls.Count); f++)
                        {
                                if (IntersectBounds(m.faceBounds[f].MultMtx(tnsfm)) != null)
                                {
                                    List<Vector3> verts = new List<Vector3>();
                                    for (int i = 0; i < m.faces[f].Length; i++)
                                    {
                                        verts.Add(Vector3.TransformPosition(m.Vertices[m.faces[f][i] - 1], tnsfm));
                                    }
                                    for (int i = 0; i < Math.Min(m.faces[f].Length, m.fnmls[f].Length) - 2; i++)
                                    {
                                        TriangleIntersection ti = RayIntersectsTriangle(this,
                                            verts[0],
                                            verts[i + 1],
                                            verts[i + 2],
                                            Vector3.TransformNormal(m.Normals[m.fnmls[f][0] - 1], tnsfm),
                                            Vector3.TransformNormal(m.Normals[m.fnmls[f][i + 1] - 1], tnsfm),
                                            Vector3.TransformNormal(m.Normals[m.fnmls[f][i + 2] - 1], tnsfm));
                                        if (ti.intersects)
                                        {
                                            intersects = true;
                                            if (ti.depth <= depth)
                                            {
                                                depth = ti.depth;
                                                normal = ti.normal;
                                                outobj = obj;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return new TraceResult(intersects, dir.Normalized() * depth + pos, normal, outobj);
                    
            }
        }
        static float edgeFunction(Vector3 a, Vector3 b, Vector3 c, int mode)
        {
            if(mode == 0)
                return (c[0] - a[0]) * (b[1] - a[1]) - (c[1] - a[1]) * (b[0] - a[0]);
            else if (mode == 1)
                return (c[0] - a[0]) * (b[2] - a[2]) - (c[2] - a[2]) * (b[0] - a[0]);
            else if (mode == 2)
                return (c[1] - a[1]) * (b[2] - a[2]) - (c[2] - a[2]) * (b[1] - a[1]);
            return 0;
        }
        static TriangleIntersection RayIntersectsTriangle(Ray r, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 n0, Vector3 n1, Vector3 n2)
        {
            Vector3 e1, e2, h, s, q;
            float a, f, u, v;
            e1 = v1 - v0;
            e2 = v2 - v0;

            h = Vector3.Cross(r.dir, e2);
            a = Vector3.Dot(e1, h);


            if (a > -0.00001 && a < 0.00001)
                return new TriangleIntersection(false);

            f = 1 / a;
            s = r.pos - v0;
            u = f * (Vector3.Dot(s, h));

            if (u < 0.0 || u > 1.0)
                return new TriangleIntersection(false);

            q = Vector3.Cross(s, e1);
            v = f * Vector3.Dot(r.dir, q);

            if (v < 0.0 || u + v > 1.0)
                return new TriangleIntersection(false);

            // at this stage we can compute t to find out where
            // the intersection point is on the line
            float t = f * Vector3.Dot(e2, q);

            Vector3 normal = new Vector3();
            if (t <= 0.00001) // ray intersection
            {
                return new TriangleIntersection(false, t, normal);
            }
            Vector3 p = r.pos + r.dir * t;

            float area;
            float w0, w1, w2;
            area = edgeFunction(v0, v1, v2, 0);
            w0 = edgeFunction(v1, v2, p, 0);
            w1 = edgeFunction(v2, v0, p, 0);
            w2 = edgeFunction(v0, v1, p, 0);
            w0 /= area;
            w1 /= area;
            w2 /= area;
            if (float.IsNaN(w0) || float.IsNaN(w1) || float.IsNaN(w2) || float.IsInfinity(w0) || float.IsInfinity(w1) || float.IsInfinity(w2))
            {
                area = edgeFunction(v0, v1, v2, 1);
                w0 = edgeFunction(v1, v2, p, 1);
                w1 = edgeFunction(v2, v0, p, 1);
                w2 = edgeFunction(v0, v1, p, 1);
                w0 /= area;
                w1 /= area;
                w2 /= area;
                if (float.IsNaN(w0) || float.IsNaN(w1) || float.IsNaN(w2) || float.IsInfinity(w0) || float.IsInfinity(w1) || float.IsInfinity(w2))
                {
                    area = edgeFunction(v0, v1, v2, 2);
                    w0 = edgeFunction(v1, v2, p, 2);
                    w1 = edgeFunction(v2, v0, p, 2);
                    w2 = edgeFunction(v0, v1, p, 2);
                    w0 /= area;
                    w1 /= area;
                    w2 /= area;
                }
            }
            normal[0] = w0 * n0[0] + w1 * n1[0] + w2 * n2[0];
            normal[1] = w0 * n0[1] + w1 * n1[1] + w2 * n2[1];
            normal[2] = w0 * n0[2] + w1 * n1[2] + w2 * n2[2];
            return new TriangleIntersection(true, t, normal.Normalized());


        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DateTime t = DateTime.Now.AddSeconds(1 / 60.0);
            if(_targetObj != null)
            {
                if (_transtime < 0)
                {
                    _transtime = 0;
                    cam.pivot = _targetObj.position;
                    if (_panzoom) cam.position = _fromcampos-_frompos+_targetObj.position;
                    cam.zoom = _targetObj.model.size.maxS * 4F;
                    _targetObj = null;
                }
                else
                {
                    cam.pivot = Vector3.Lerp(_targetObj.position, _frompos, _smoothtranstime);
                    if (_panzoom) cam.position = Vector3.Lerp(_fromcampos - _frompos + _targetObj.position, _fromcampos, _smoothtranstime);
                    cam.zoom = lerp(_targetObj.model.size.maxS * 4F, _fromzoom, _smoothtranstime);
                }
            }
            if (Forward)
            {
                Vector3 vel = new Vector3(0, 0, -cam.zoom / 40F);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (Backward)
            {
                Vector3 vel = new Vector3(0, 0, cam.zoom / 40F);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (SideLeft)
            {
                Vector3 vel = new Vector3(-cam.zoom / 40F, 0, 0);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (SideRight)
            {
                Vector3 vel = new Vector3(cam.zoom / 40F, 0, 0);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (SideUp)
            {
                Vector3 vel = new Vector3(0, cam.zoom / 40F, 0);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (SideDown)
            {
                Vector3 vel = new Vector3(0, -cam.zoom / 40F, 0);
                Matrix4 rotmtx = cam.look_at_mtx.ClearTranslation();
                cam.pivot += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
                cam.position += (Matrix4.CreateTranslation(vel) * rotmtx.Inverted()).ExtractTranslation();
            }
            if (!DesignMode)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                //Import Objects if necessary
                if (_ObjectsToLoad.Count > 0)
                {
                    foreach (string objectpath in _ObjectsToLoad)
                    {
                        Object3D obj = new Object3D(objectpath);
                        if (_AttachmentToAttachTo == null) goto no;
                        if (!obj.attachTo(_AttachmentToAttachTo, Object3D.Active_Object)) goto no;
                        goto yes;
                        no:;
                        obj.position = cam.pivot;
                        yes:;
                        GoToObject(obj, true);
                        Object3D.database.Add(obj);
                        Object3D.Active_Object = obj;
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
                        obj.DrawObject(_pgm, _coll, _wf, inSight(obj, cam));
                }
                switch (Form1.current_tool)
                {
                    case Tools.Select:
                        if (tr.Hit)
                        {
                            GL.Color3(0.8F, 0.8F, 0.8F);
                            ToolModels.DrawBall(tr.HitPos, cam.zoom / 120F, 16);
                        }
                        break;
                    case Tools.Decorate:
                        if (tr.Hit)
                        {
                            GL.Color3(1F, 0, 0);
                            ToolModels.DrawBall(tr.HitPos, cam.zoom / 50F, 16);
                            GL.Color3(1F, 1F, 0);
                            GL.LineWidth(2F);
                            GL.Begin(PrimitiveType.Lines);
                            GL.Vertex3(tr.HitPos);
                            GL.Vertex3(tr.HitPos+tr.HitNormal*cam.zoom/15F);
                            GL.End();
                        }
                        break;
                }
                GL.Clear(ClearBufferMask.DepthBufferBit);
                if(Object3D.Active_Object != null)
                    Object3D.Active_Object.DrawTool();
                SwapBuffers();
            }
            if (Forward || Backward || SideLeft || SideRight || SideUp || SideDown || (_targetObj != null))
            {
                while (DateTime.Now < t) {}
                _transtime-=1/30F;
                Invalidate();
            }

        }

        public static bool inSight(Object3D obj, Camera cam)
        {
            Matrix4 mtx = obj.transform;
            Vector3 pos1 = ToScreen(Vector3.TransformPosition(obj.model.size.nXnYnZ, mtx), cam);
            Vector3 pos2 = ToScreen(Vector3.TransformPosition(obj.model.size.pXnYnZ, mtx), cam);
            Vector3 pos3 = ToScreen(Vector3.TransformPosition(obj.model.size.nXpYnZ, mtx), cam);
            Vector3 pos4 = ToScreen(Vector3.TransformPosition(obj.model.size.nXnYpZ, mtx), cam);
            Vector3 pos5 = ToScreen(Vector3.TransformPosition(obj.model.size.pXpYnZ, mtx), cam);
            Vector3 pos6 = ToScreen(Vector3.TransformPosition(obj.model.size.pXnYpZ, mtx), cam);
            Vector3 pos7 = ToScreen(Vector3.TransformPosition(obj.model.size.nXpYpZ, mtx), cam);
            Vector3 pos8 = ToScreen(Vector3.TransformPosition(obj.model.size.pXpYpZ, mtx), cam);
            float Left =
                Math.Min(pos1.X,
                Math.Min(pos2.X,
                Math.Min(pos3.X,
                Math.Min(pos4.X,
                Math.Min(pos5.X,
                Math.Min(pos6.X,
                Math.Min(pos7.X, pos8.X)))))));
            float Right =
                Math.Max(pos1.X,
                Math.Max(pos2.X,
                Math.Max(pos3.X,
                Math.Max(pos4.X,
                Math.Max(pos5.X,
                Math.Max(pos6.X,
                Math.Max(pos7.X, pos8.X)))))));
            float Bottom =
                Math.Min(pos1.Y,
                Math.Min(pos2.Y,
                Math.Min(pos3.Y,
                Math.Min(pos4.Y,
                Math.Min(pos5.Y,
                Math.Min(pos6.Y,
                Math.Min(pos7.Y, pos8.Y)))))));
            float Top =
                Math.Max(pos1.Y,
                Math.Max(pos2.Y,
                Math.Max(pos3.Y,
                Math.Max(pos4.Y,
                Math.Max(pos5.Y,
                Math.Max(pos6.Y,
                Math.Max(pos7.Y, pos8.Y)))))));
            float Depth =
                Math.Max(pos1.Z,
                Math.Max(pos2.Z,
                Math.Max(pos3.Z,
                Math.Max(pos4.Z,
                Math.Max(pos5.Z,
                Math.Max(pos6.Z,
                Math.Max(pos7.Z, pos8.Z)))))));
            if (Left > 1) return false;
            if (Right < -1) return false;
            if (Bottom > 1) return false;
            if (Top < -1) return false;
            if (Depth < 0) return false;
            return true;
        }
        public Ray FromMousePos(Matrix4 mtx)
        {
            return new Ray(cam.position,
                    Matrix4.Mult(
                        Matrix4.CreateTranslation(new Vector3(-1 + (mx / 1F / Width) * 2, 1 - (my / 1F / Height) * 2, 1)),
                        mtx.ClearTranslation().Inverted()
                    ).ExtractTranslation(),
                    cam);
        }
        public static Vector3 ToScreen(Vector3 pos, Camera cam)
        {

            Vector3 proj = Vector3.TransformPosition(pos, cam.matrix);
            return new Vector3(proj.X / proj.Z, proj.Y / proj.Z, proj.Z);
        }
        Point _prev = new Point();
        bool Forward, Backward, SideLeft, SideRight, SideUp, SideDown = false;
        private Ray MPray;
        private TraceResult tr = new TraceResult();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Forward |= (e.KeyCode == Keys.W);
            Backward |= (e.KeyCode == Keys.S);
            SideLeft |= (e.KeyCode == Keys.A);
            SideRight |= (e.KeyCode == Keys.D);
            SideUp |= (e.KeyCode == Keys.E);
            SideDown |= (e.KeyCode == Keys.Q);
            //if(e.KeyCode == Keys.G)
            //{
            //}
            Invalidate();
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Forward &= !(e.KeyCode == Keys.W);
            Backward &= !(e.KeyCode == Keys.S);
            SideLeft &= !(e.KeyCode == Keys.A);
            SideRight &= !(e.KeyCode == Keys.D);
            SideUp &= !(e.KeyCode == Keys.E);
            SideDown &= !(e.KeyCode == Keys.Q);
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _prev = e.Location;
            if (Form1.current_tool == Tools.Select)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (tr.Hit)
                    {
                        Object3D.Active_Object = tr.HitObject;
                        float distance = float.PositiveInfinity;
                        Attachment closestAtch = null;
                        foreach (Attachment atch in Object3D.Active_Object.model.attachments)
                        {
                            foreach (Object3D.attachmentInfo atif in Object3D.Active_Object.atch_info)
                                if (atif.thisAtch == atch)
                                    goto no;
                            float this_dist = (atch.get_world_transform(Object3D.Active_Object.transform).ExtractTranslation() - tr.HitPos).Length;
                            if (distance > this_dist)
                            {
                                closestAtch = atch;
                                distance = this_dist;
                            }
                            no:;
                        }
                        if (closestAtch != null)
                        {
                            Object3D.Active_Object.Active_Attachment = closestAtch;
                        }
                        ((Form1)Form.ActiveForm).listBox1_DoStuffWhenIndexChanged = false;
                        ((Form1)Form.ActiveForm).UpdateActiveObject();
                        ((Form1)Form.ActiveForm).listBox1_DoStuffWhenIndexChanged = true;
                    }
                    else { Object3D.Active_Object = null;Invalidate();
                        ((Form1)Form.ActiveForm).listBox1_DoStuffWhenIndexChanged = false;
                        ((Form1)Form.ActiveForm).UpdateActiveObject();
                        ((Form1)Form.ActiveForm).listBox1_DoStuffWhenIndexChanged = true;
                    }
                }
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            cam.zoom = Math.Max(cam.clip_near, cam.zoom+e.Delta/12F* (float)Math.Pow(cam.zoom, 0.375F));
            Invalidate();
        }
        bool _prevhit = false;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mx = e.X;
            my = e.Y;
            bool invalidate = false;
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
                invalidate = true;
            }
            Matrix4 mtx = cam.matrix;
            MPray = FromMousePos(mtx);
            tr = MPray.Trace(cam);

            if (tr.Hit || invalidate || _prevhit) Invalidate();
            _prevhit = tr.Hit;
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
