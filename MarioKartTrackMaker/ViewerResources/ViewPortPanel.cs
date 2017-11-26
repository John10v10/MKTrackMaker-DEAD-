using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace MarioKartTrackMaker.ViewerResources
{
    /// <summary>
    /// This is the view port that displays the 3D scenery.
    /// </summary>
    public partial class ViewPortPanel : GLControl
    {
        /// <summary>
        /// Constructs a new view port with a specified graphics mode.
        /// </summary>
        /// <param name="mode">Graphics mode</param>
        public ViewPortPanel(OpenTK.Graphics.GraphicsMode mode) : base(mode)
        {
            InitializeComponent();
        }
        /// <summary>
        /// Constructs a new view port.
        /// </summary>
        public ViewPortPanel()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Signal to when the viewport and graphics setup (GL) is fully initiated and ready.
        /// </summary>
        bool loaded = false;
        private bool _wf;
        /// <summary>
        /// Defines whether to render in wireframe mode or not.
        /// </summary>
        public bool wireframemode { get { return _wf; } set { _wf = value; Invalidate(); } }
        private int _coll = 1;
        /// <summary>
        /// Defines whether to render models, collisions or both.
        /// </summary>
        public int collisionviewmode { get { return _coll; } set { _coll = value; Invalidate(); } }
        /// <summary>
        /// The one and only camera of this viewport.
        /// </summary>
        public Camera cam;
        private int _fs, _vs, _pgm;

        /// <summary>
        /// Setup all the graphics (GL) and fully initiate the view port.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cam = new Camera(new Vector3(0F,0F,-10F), new Vector3(0F, 0F, 0F), this);
            cam.position = new Vector3(1000f, -1200f, 2000f);
            if (!DesignMode)
            {
                GL.ClearColor(Color.Black);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Multisample);
                GL.Enable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);
                GL.Enable(EnableCap.AlphaTest);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                _fs = Shader.CompileFragmentShader();
                _vs = Shader.CompileVertexShader();
                _pgm = Shader.ProgramLink(_vs, _fs);
                loaded = true;
            }
            
        }
        /// <summary>
        /// Ooh, mouse positions...
        /// </summary>
        private int mx, my = 0;

        //Schedule for importing. These objects have to be imported in this view port because graphical (GL) commands can only be done when painting this view port.
        /// <summary>
        /// Object file paths scheduled to load.
        /// </summary>
        private List<string> _ObjectsToLoad = new List<string>();
        /// <summary>
        /// Attachments scheduled to attach to previous active object.
        /// </summary>
        private Attachment _AttachmentToAttachTo = null;
        /// <summary>
        /// Schedules an object to be imported into the scene.
        /// </summary>
        /// <param name="filepath">The file path of the object to import.</param>
        /// <param name="attachment">The current attachment for the new part to attach to.</param>
        public void InsertObject(string filepath, Attachment attachment) {
            _ObjectsToLoad.Add(filepath);
            _AttachmentToAttachTo = attachment;
        }
        Random r = new Random();
        //Navigation variables.
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
        /// <summary>
        /// Calls the view port to transition to the specified object.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <param name="pan">Should the camera pan? or should the camera simply follow it's pivot?</param>
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
        /// <summary>
        /// Some simple mathematical function. I should have added this to the MathUtils class.
        /// </summary>
        private float lerp(float a,float b, float c)
        {
            return a + (b - a) * c;
        }
        /// <summary>
        /// I had to build a structure for the result of a ray-trace.
        /// </summary>
        public struct TraceResult
        {
            public bool Hit;
            public Vector3 HitPos;
            public Object3D HitObject;
            public Vector3 HitNormal;
            public int ToolHit;

            public TraceResult(bool Hit, Vector3 HitPos, Vector3 HitNormal, Object3D HitObject, int ToolHit) : this() {
                this.Hit = Hit;
                this.HitPos = HitPos;
                this.HitObject = HitObject;
                this.HitNormal = HitNormal;
                this.ToolHit = ToolHit;
            }
        }
        /// <summary>
        /// The ray structure.
        /// </summary>
        public struct Ray
        {
            /// <summary>
            /// The origin of the ray.
            /// </summary>
            public Vector3 pos;
            /// <summary>
            /// The direction of the ray.
            /// </summary>
            public Vector3 dir;
            /// <summary>
            /// The camera that uses this ray.
            /// </summary>
            public Camera camera;
            /// <summary>
            /// Constructs the ray.
            /// </summary>
            /// <param name="pos">Define the origin.</param>
            /// <param name="dir">Define the direction.</param>
            /// <param name="camera">Get the camera that uses this ray.</param>
            public Ray(Vector3 pos, Vector3 dir, Camera camera) : this() {
                this.pos = pos;
                this.dir = dir;
                this.camera = camera;
            }
            /// <summary>
            /// Constructs the ray.
            /// </summary>
            /// <param name="pos">Define the origin.</param>
            /// <param name="dir">Define the direction.</param>
            public Ray(Vector3 pos, Vector3 dir) : this()
            {
                this.pos = pos;
                this.dir = dir;
            }

            /// <summary>
            /// Function Taken from technologicalutopia 
            /// </summary>
            /// <param name="box">The bounding box to calculate with.</param>
            /// <returns></returns>
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
            /// <summary>
            /// Traces the ray and stores the result into the structure.
            /// </summary>
            /// <param name="cam">The camera that owns this ray.</param>
            /// <param name="objects">Should this ray trace objects?</param>
            /// <param name="MoveTool">Should this ray trace the move tool?</param>
            /// <param name="RotateTool">Should this ray trace the rotation tool?</param>
            /// <param name="ScaleTool">Should this ray trace the scale tool?</param>
            /// <returns></returns>
            public TraceResult Trace(Camera cam, bool objects, bool MoveTool, bool RotateTool, bool ScaleTool)
            {
                float depth = float.PositiveInfinity;
                bool intersects = false;
                Vector3 normal = new Vector3();
                Object3D outobj = null;
                int ToolHit = -1;
                if (objects)
                {
                    foreach (Object3D obj in Object3D.database)
                    {
                        Matrix4 tnsfm = obj.transform;
                        if (inSight(obj, cam))
                        {
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
                                            MathUtils.TriangleIntersection ti = MathUtils.RayIntersectsTriangle(this,
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
                }
                if (MoveTool)
                {
                    foreach (Object3D obj in Object3D.database)
                    {
                        if (inSight(obj, cam))
                        {
                            Matrix4 tnsfm = obj.transform;
                            Vector3 Pos = Vector3.TransformPosition(Vector3.Zero, tnsfm);
                            Ray MoveX = new Ray(Pos, Vector3.TransformPosition(Vector3.UnitX * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 2 : 0.5f), tnsfm)-pos);
                            Vector3[] CPx = MathUtils.ClosestPointsBetweenRays(this, MoveX, cam.clip_near, 0F, cam.clip_far, 0.9F);
                            float DistanceX = (Vector3.TransformPosition(CPx[1], tnsfm.Inverted()) - Vector3.TransformPosition(CPx[0], tnsfm.Inverted())).Length;
                            if (DistanceX / obj.model.size.maxS / ((obj == Object3D.Active_Object) ? 1 : 0.25f) * 10 < 2)
                            {
                                float DistanceFromCameraX = (CPx[0] - cam.position).Length;
                                if (DistanceFromCameraX < depth)
                                {
                                    depth = DistanceFromCameraX;
                                    ToolHit = 0;
                                    //normal = (CPx[1] - CPx[0]).Normalized();
                                       
                                    outobj = obj;
                                }
                            }
                            Ray MoveY = new Ray(Pos, Vector3.TransformPosition(Vector3.UnitY * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 2 : 0.5f), tnsfm) - pos);
                            Vector3[] CPy = MathUtils.ClosestPointsBetweenRays(this, MoveY, cam.clip_near, 0F, cam.clip_far, 0.9F);
                            float DistanceY = (Vector3.TransformPosition(CPy[1], tnsfm.Inverted()) - Vector3.TransformPosition(CPy[0], tnsfm.Inverted())).Length;
                            if (DistanceY / obj.model.size.maxS / ((obj == Object3D.Active_Object) ? 1 : 0.25f) * 10 < 2)
                            {
                                float DistanceFromCameraY = (CPy[0] - cam.position).Length;
                                if (DistanceFromCameraY < depth)
                                {
                                    depth = DistanceFromCameraY;
                                    ToolHit = 1;
                                    //normal = (CPy[1] - CPy[0]).Normalized();
                                    outobj = obj;
                                }
                            }
                            Ray MoveZ = new Ray(Pos, Vector3.TransformPosition(Vector3.UnitZ * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 2 : 0.5f), tnsfm) - pos);
                            Vector3[] CPz = MathUtils.ClosestPointsBetweenRays(this, MoveZ, cam.clip_near, 0F, cam.clip_far, 0.9F);
                            float DistanceZ = (Vector3.TransformPosition(CPz[1], tnsfm.Inverted()) - Vector3.TransformPosition(CPz[0], tnsfm.Inverted())).Length;
                            if (DistanceZ / obj.model.size.maxS / ((obj == Object3D.Active_Object) ? 1 : 0.25f) * 10 < 2)
                            {
                                float DistanceFromCameraZ = (CPz[0] - cam.position).Length;
                                if (DistanceFromCameraZ < depth)
                                {
                                    depth = DistanceFromCameraZ;
                                    ToolHit = 2;
                                    //normal = (CPz[1] - CPz[0]).Normalized();
                                    outobj = obj;
                                }
                            }
                        }
                    }
                }
                if (RotateTool)
                {
                    foreach (Object3D obj in Object3D.database)
                    {
                        if (inSight(obj, cam))
                        {
                            Matrix4 tnsfm = obj.transform;
                            for (int i = 0; i < 16; i++)
                            {
                                double deg = (i + 1) * 22.5 * Math.PI / 180.0;
                                double deg2 = (i) * 22.5 * Math.PI / 180.0;
                                Vector3 pos = Vector3.TransformPosition(new Vector3(0, (float)Math.Sin(deg) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), (float)Math.Cos(deg) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f)), tnsfm);
                                Vector3 dir = Vector3.TransformPosition(new Vector3(0, (float)Math.Sin(deg2) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), (float)Math.Cos(deg2) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f)), tnsfm) - pos;
                                Ray XRing = new Ray(pos, dir);
                                Vector3[] CPx = MathUtils.ClosestPointsBetweenRays(this, XRing, cam.clip_near, 0F, cam.clip_far, 1F);
                                float DistanceX = (CPx[1] - CPx[0]).Length;
                                float DistanceFromCameraX = (CPx[0] - cam.position).Length;
                                if (DistanceX / ((obj == Object3D.Active_Object) ? 1 : 0.5f) / DistanceFromCameraX * 100 < 1)
                                {
                                    if (DistanceFromCameraX < depth)
                                    {
                                        depth = DistanceFromCameraX;
                                        ToolHit = 0;
                                        //normal = (CPx[1] - CPx[0]).Normalized();

                                        outobj = obj;
                                    }
                                }
                                pos = Vector3.TransformPosition(new Vector3((float)Math.Sin(deg) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), 0, (float)Math.Cos(deg) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f)), tnsfm);
                                dir = Vector3.TransformPosition(new Vector3((float)Math.Sin(deg2) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), 0, (float)Math.Cos(deg2) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f)), tnsfm) - pos;
                                Ray YRing = new Ray(pos, dir);
                                Vector3[] CPy = MathUtils.ClosestPointsBetweenRays(this, YRing, cam.clip_near, 0F, cam.clip_far, 1F);
                                float DistanceY = (CPy[1] - CPy[0]).Length;
                                float DistanceFromCameraY = (CPy[0] - cam.position).Length;
                                if (DistanceY / ((obj == Object3D.Active_Object) ? 1 : 0.5f) / DistanceFromCameraY * 100 < 1)
                                {
                                    if (DistanceFromCameraY < depth)
                                    {
                                        depth = DistanceFromCameraY;
                                        ToolHit = 1;
                                        //normal = (CPy[1] - CPy[0]).Normalized();

                                        outobj = obj;
                                    }
                                }
                                pos = Vector3.TransformPosition(new Vector3((float)Math.Sin(deg) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), (float)Math.Cos(deg) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), 0), tnsfm);
                                dir = Vector3.TransformPosition(new Vector3((float)Math.Sin(deg2) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), (float)Math.Cos(deg2) * 0.625F * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.5f), 0), tnsfm) - pos;
                                Ray ZRing = new Ray(pos, dir);
                                Vector3[] CPz = MathUtils.ClosestPointsBetweenRays(this, ZRing, cam.clip_near, 0F, cam.clip_far, 1F);
                                float DistanceZ = (CPz[1] - CPz[0]).Length;
                                float DistanceFromCameraZ = (CPz[0] - cam.position).Length;
                                if (DistanceZ / ((obj == Object3D.Active_Object) ? 1 : 0.5f) / DistanceFromCameraZ * 100 < 1)
                                {
                                    if (DistanceFromCameraZ < depth)
                                    {
                                        depth = DistanceFromCameraZ;
                                        ToolHit = 2;
                                        //normal = (CPz[1] - CPz[0]).Normalized();

                                        outobj = obj;
                                    }
                                }
                            }
                        }
                    }
                }
                if (ScaleTool)
                {
                    foreach (Object3D obj in Object3D.database)
                    {
                        if (inSight(obj, cam))
                        {
                            Matrix4 tnsfm = obj.transform;
                            Vector3 Pos = Vector3.TransformPosition(Vector3.Zero, tnsfm);
                            Ray ScaleX = new Ray(Pos, Vector3.TransformPosition(Vector3.UnitX * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.25f), tnsfm) - Pos);
                            Vector3[] CPx = MathUtils.ClosestPointsBetweenRays(this, ScaleX, cam.clip_near, 0.375F, cam.clip_far, 0.875F);
                            float DistanceX = (Vector3.TransformVector(CPx[1] - CPx[0], tnsfm.ClearTranslation().Inverted())).Length;
                            if (DistanceX / obj.model.size.maxS / ((obj == Object3D.Active_Object) ? 1 : 0.25f) * 16 < Math.PI)
                            {
                                float DistanceFromCameraX = (CPx[0] - cam.position).Length;
                                if (DistanceFromCameraX < depth)
                                {
                                    depth = DistanceFromCameraX;
                                    ToolHit = 0;
                                    //normal = (CPx[1] - CPx[0]).Normalized();

                                    outobj = obj;
                                }
                            }
                            Ray ScaleY = new Ray(Pos, Vector3.TransformPosition(Vector3.UnitY * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.25f), tnsfm) - Pos);
                            Vector3[] CPy = MathUtils.ClosestPointsBetweenRays(this, ScaleY, cam.clip_near, 0.375F, cam.clip_far, 0.875F);
                            float DistanceY = (Vector3.TransformVector(CPy[1]-CPy[0], tnsfm.ClearTranslation().Inverted())).Length;
                            if (DistanceY / obj.model.size.maxS / ((obj == Object3D.Active_Object) ? 1 : 0.25f) * 16 < Math.PI)
                            {
                                float DistanceFromCameraY = (CPy[0] - cam.position).Length;
                                if (DistanceFromCameraY < depth)
                                {
                                    depth = DistanceFromCameraY;
                                    ToolHit = 1;
                                    //normal = (CPy[1] - CPy[0]).Normalized();
                                    outobj = obj;
                                }
                            }
                            Ray ScaleZ = new Ray(Pos, Vector3.TransformPosition(Vector3.UnitZ * obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.25f), tnsfm) - Pos);
                            Vector3[] CPz = MathUtils.ClosestPointsBetweenRays(this, ScaleZ, cam.clip_near, 0.375F, cam.clip_far, 0.875F);
                            float DistanceZ = (Vector3.TransformVector(CPz[1] - CPz[0], tnsfm.ClearTranslation().Inverted())).Length;
                            if (DistanceZ / obj.model.size.maxS / ((obj == Object3D.Active_Object) ? 1 : 0.25f) * 16 < Math.PI)
                            {
                                float DistanceFromCameraZ = (CPz[0] - cam.position).Length;
                                if (DistanceFromCameraZ < depth)
                                {
                                    depth = DistanceFromCameraZ;
                                    ToolHit = 2;
                                    //normal = (CPz[1] - CPz[0]).Normalized();
                                    outobj = obj;
                                }
                            }
                            double? sph_t = MathUtils.intersectSphere(this, obj.model.size.maxS * ((obj == Object3D.Active_Object) ? 1 : 0.25f)/4F, tnsfm);
                            if(sph_t != null)
                            {
                                if (sph_t < depth)
                                {
                                    depth = (float)sph_t;
                                    ToolHit = 3;
                                    //normal = (CPz[1] - CPz[0]).Normalized();
                                    outobj = obj;
                                }
                            }
                        }
                    }
                }
                return new TraceResult(intersects, dir * depth + pos, normal, outobj, ToolHit);

            }
        }
        /// <summary>
        /// YAY! We're painting the scene! :D
        /// </summary>  
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if(_targetObj != null)
            {
                if (_transtime < 0)
                {
                    _transtime = 0;
                    cam.pivot = _targetObj.position;
                    if (_panzoom) cam.position = _fromcampos-_frompos+_targetObj.position;
                    cam.zoom = _targetObj.model.size.maxS * 4F * Math.Max(_targetObj.scale.X, Math.Max(_targetObj.scale.Y, _targetObj.scale.Z));
                    _targetObj = null;
                }
                else
                {
                    cam.pivot = Vector3.Lerp(_targetObj.position, _frompos, _smoothtranstime);
                    if (_panzoom) cam.position = Vector3.Lerp(_fromcampos - _frompos + _targetObj.position, _fromcampos, _smoothtranstime);
                    cam.zoom = lerp(_targetObj.model.size.maxS * 4F * Math.Max(_targetObj.scale.X, Math.Max(_targetObj.scale.Y, _targetObj.scale.Z)), _fromzoom, _smoothtranstime);
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
                //Import Objects if signaled
                if (_ObjectsToLoad.Count > 0)
                {
                    foreach (string objectpath in _ObjectsToLoad)
                    {
                        Object3D obj = new Object3D(objectpath);
                        if (_AttachmentToAttachTo == null) goto no;
                        foreach (Object3D.attachmentInfo atif in Object3D.Active_Object.atch_info)
                            if (atif.thisAtch == _AttachmentToAttachTo)
                                goto no;
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
                //Here I was testing the closest vectors between two Rays with MathUtils Function
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
                        obj.DrawObject(_pgm, _coll, _wf, inSight(obj, cam), cam);
                }
                switch (MainForm.current_tool)
                {
                    case Tools.Select:
                        if (tr.Hit)
                        {
                            GL.Color3(0.8F, 0.8F, 0.8F);
                            ToolModels.DrawBall(tr.HitPos, cam.zoom / 120F, 16);
                        }
                        break;
                    case Tools.Snap:
                        if (tr.Hit)
                        {
                            float distance = float.PositiveInfinity;
                            Attachment closestAtch = null;
                            foreach (Attachment atch in tr.HitObject.model.attachments)
                            {
                                if (tr.HitObject == Object3D.Active_Object) goto no;
                                foreach (Object3D.attachmentInfo atif in tr.HitObject.atch_info)
                                    if (atif.thisAtch == atch)
                                        goto no;
                                float this_dist = (atch.get_world_transform(tr.HitObject.transform).ExtractTranslation() - tr.HitPos).Length;
                                if (distance > this_dist && Object3D.Active_Object != null  && Object3D.Active_Object.Active_Attachment != null && atch.isFemale != Object3D.Active_Object.Active_Attachment.isFemale
                                    && !Object3D.Active_Object.ContainsObjectInChain(tr.HitObject))
                                {
                                    closestAtch = atch;
                                    distance = this_dist;
                                }
                                no:;
                            }
                            if (closestAtch != null && Object3D.Active_Object != null) {
                                GL.Color3(0, 0.75F, 0);
                                GL.LineWidth(2F);
                                GL.Begin(PrimitiveType.Lines);
                                GL.Vertex3(Object3D.Active_Object.Active_Attachment.get_world_transform(Object3D.Active_Object.transform).ExtractTranslation());
                                GL.Vertex3(closestAtch.get_world_transform(tr.HitObject.transform).ExtractTranslation());
                                GL.End();
                            }
                        }
                        break;
                    case Tools.Decorate:
                        if (tr.Hit)
                        {
                            if (!float.IsNaN(MainForm.decorate_erase_mode))
                            {
                                GL.Color3(1F, 0.5F, 1F);
                                ToolModels.DrawBall(tr.HitPos, MainForm.decorate_erase_mode, 32);
                            }
                            else
                            {
                                GL.Color3(1F, 0, 0);
                                ToolModels.DrawBall(tr.HitPos, cam.zoom / 50F, 16);
                                GL.Color3(1F, 1F, 0);
                                GL.LineWidth(2F);
                                GL.Begin(PrimitiveType.Lines);
                                GL.Vertex3(tr.HitPos);
                                GL.Vertex3(tr.HitPos + tr.HitNormal * cam.zoom / 15F);
                                GL.End();
                            }
                        }
                        break;
                }
                GL.Clear(ClearBufferMask.DepthBufferBit);
                foreach (Object3D obj in Object3D.database)
                {
                    obj.DrawTool((_IsDragging != -1 && (obj == _ObjectToDrag)) ?_IsDragging:((obj == tr.HitObject) ? tr.ToolHit : -1));
                }
                //Testing Out Collisions
                /*if (ModifierKeys == Keys.Alt) {
                    GL.PointSize(1F);
                    GL.Begin(PrimitiveType.Points);
                    GL.Color3(1F, 1F, 1F);
                    for (int _y = 0; _y < Height; _y += 2)
                    {
                        for (int _x = 0; _x < Width; _x += 2)
                        {
                            Ray _r = FromScreen(_x, _y, mtx);
                            TraceResult _tr = _r.Trace(cam, false, false, false, true);
                            if (_tr.ToolHit != -1)
                            {
                                GL.Color3(1F, 1F, 0);
                                GL.Vertex3(_r.pos + _r.dir * 100);
                            }
                        }
                    }
                    GL.End();
                }*/
                
                SwapBuffers();
            }
            if (Forward || Backward || SideLeft || SideRight || SideUp || SideDown || (_targetObj != null))
            {
                _transtime-=1/30F;
                Invalidate();
            }

        }

        /// <summary>
        /// Checks if an object is currently seen by the camera.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="cam">The camera.</param>
        public static bool inSight(Object3D obj, Camera cam)
        {
            return inSight(obj.model.size, obj.transform, cam);
        }
        /// <summary>
        /// Checks if a decoration object is currently seen by the camera.
        /// </summary>
        /// <param name="dec">The decoration object.</param>
        /// <param name="obj">The object the decoration object lies on.</param>
        /// <param name="cam">The camera.</param>
        public static bool inSight(DecorationObject dec, Object3D obj, Camera cam)
        {
            return inSight(dec.mesh.size, dec.transform*obj.transform, cam);
        }
        /// <summary>
        /// Checks if the specified boundaries are in the camera's sight.
        /// </summary>
        /// <param name="bounds">The specified bounds.</param>
        /// <param name="mtx">The transformation of the bounds.</param>
        /// <param name="cam">The camera.</param>
        /// <returns></returns>
        public static bool inSight(Bounds bounds, Matrix4 mtx, Camera cam)
        {
            Vector3 pos1 = ToScreen(Vector3.TransformPosition(bounds.nXnYnZ, mtx), cam);
            Vector3 pos2 = ToScreen(Vector3.TransformPosition(bounds.pXnYnZ, mtx), cam);
            Vector3 pos3 = ToScreen(Vector3.TransformPosition(bounds.nXpYnZ, mtx), cam);
            Vector3 pos4 = ToScreen(Vector3.TransformPosition(bounds.nXnYpZ, mtx), cam);
            Vector3 pos5 = ToScreen(Vector3.TransformPosition(bounds.pXpYnZ, mtx), cam);
            Vector3 pos6 = ToScreen(Vector3.TransformPosition(bounds.pXnYpZ, mtx), cam);
            Vector3 pos7 = ToScreen(Vector3.TransformPosition(bounds.nXpYpZ, mtx), cam);
            Vector3 pos8 = ToScreen(Vector3.TransformPosition(bounds.pXpYpZ, mtx), cam);
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
        /// <summary>
        /// Gets the mouse coordinates and converts them into a 3D point. This is useful for constructing rays based on the camera and mouse position.
        /// </summary>
        /// <param name="mtx">The camera's matrix.</param>
        public Ray FromMousePos(Matrix4 mtx)
        {
            return FromScreen(mx,my,mtx);
        }
        /// <summary>
        /// Gets the specified x and y screen coordinates and converts them into a 3D point.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">the y coordinate.</param>
        /// <param name="mtx">The camera's matrix.</param>
        /// <returns></returns>
        public Ray FromScreen(float x, float y, Matrix4 mtx)
        {
            return new Ray(cam.position,
                    Matrix4.Mult(
                        Matrix4.CreateTranslation(new Vector3(-1 + (x / 1F / Width) * 2, 1 - (y / 1F / Height) * 2, 1)),
                        mtx.ClearTranslation().Inverted()
                    ).ExtractTranslation(),
                    cam);
        }
        /// <summary>
        /// Gets the specified 3D point and converts it to screen coordinates, including the depth.
        /// </summary>
        /// <param name="pos">The specified 3D point.</param>
        /// <param name="cam">The camera.</param>
        public static Vector3 ToScreen(Vector3 pos, Camera cam)
        {

            Vector3 proj = Vector3.TransformPosition(pos, cam.matrix_toScreen);
            return new Vector3(proj.X / proj.Z * (proj.Z/Math.Abs(proj.Z)), proj.Y / proj.Z * (proj.Z / Math.Abs(proj.Z)), proj.Z);
        }
        Point _prev = new Point();
        /// <summary>
        /// Oh just a controller flag.
        /// </summary>
        bool Forward, Backward, SideLeft, SideRight, SideUp, SideDown = false;
        /// <summary>
        /// The view port's ray calculated from the camera and the mouse coordinates.
        /// </summary>
        private Ray MPray;
        /// <summary>
        /// The view port's trace result calculated from the camera and the mouse coordinates.
        /// </summary>
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
            if (e.KeyCode == Keys.F)
                GoToObject(Object3D.Active_Object, ModifierKeys == Keys.Shift);
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
        /// <summary>
        /// Defines what part of a selected tool is dragging.
        /// </summary>
        int _IsDragging = -1;
        /// <summary>
        /// Specifies the object to drag.
        /// </summary>
        Object3D _ObjectToDrag = null;
        object _ObjectToDragPreviousPosition;
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
                if (_IsDragging != -1)
                {
                    _IsDragging = -1;
                    if(Form.ActiveForm is MainForm)((MainForm)Form.ActiveForm).UpdateObjectStats();
                    if(Form.ActiveForm is MainForm)((MainForm)Form.ActiveForm).DisplayObjectList();
                }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _prev = e.Location;
            if (MainForm.current_tool == Tools.Select)
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
                        if (Form.ActiveForm is MainForm) ((MainForm)Form.ActiveForm).DoStuffWhenSelectedObjectIndexChanged = false;
                        if (Form.ActiveForm is MainForm) ((MainForm)Form.ActiveForm).UpdateActiveObject();
                        if (Form.ActiveForm is MainForm) ((MainForm)Form.ActiveForm).DoStuffWhenSelectedObjectIndexChanged = true;
                    }
                    else
                    {
                        Object3D.Active_Object = null;
                        Invalidate();
                        if (Form.ActiveForm is MainForm) ((MainForm)Form.ActiveForm).DoStuffWhenSelectedObjectIndexChanged = false;
                        if (Form.ActiveForm is MainForm) ((MainForm)Form.ActiveForm).UpdateActiveObject();
                        if (Form.ActiveForm is MainForm) ((MainForm)Form.ActiveForm).DoStuffWhenSelectedObjectIndexChanged = true;
                    }
                }
            }
            else if (MainForm.current_tool == Tools.Move)
            {
                Matrix4 mtx = cam.matrix;
                if (e.Button == MouseButtons.Left)
                {
                    if (tr.ToolHit != -1)
                    {
                        _IsDragging = tr.ToolHit;
                        _ObjectToDrag = tr.HitObject;
                        Matrix4 tnsfm = _ObjectToDrag.transform;
                        Ray Move = new Ray(Vector3.TransformPosition(Vector3.Zero, tnsfm), Vector3.TransformNormal((tr.ToolHit == 0) ? Vector3.UnitX : ((tr.ToolHit == 1) ? Vector3.UnitY : Vector3.UnitZ), tnsfm));
                        Vector3[] cp = MathUtils.ClosestPointsBetweenRays(FromMousePos(mtx), Move);
                        _ObjectToDragPreviousPosition = cp[1] - _ObjectToDrag.position;
                        _ObjectToDragPreviousMatrix = tnsfm;
                    }
                }
            }
            else if (MainForm.current_tool == Tools.Rotate)
            {
                Matrix4 mtx = cam.matrix;
                if (e.Button == MouseButtons.Left)
                {
                    if (tr.ToolHit == 0)
                    {
                        _IsDragging = tr.ToolHit;
                        _ObjectToDrag = tr.HitObject;
                        Matrix4 tnsfm = _ObjectToDrag.transform;
                        float size = 100;
                        Ray __r = FromMousePos(mtx);
                        float __t = 0;
                        size = _ObjectToDrag.model.size.maxS * 2;
                        __t = MathUtils.intersectPlane(__r, Vector3.TransformNormal(Vector3.UnitX, tnsfm), Vector3.TransformPosition(Vector3.Zero, tnsfm));
                        Vector3 PlaneHitPos = Vector3.TransformPosition(__r.pos + __r.dir * Math.Abs(__t), tnsfm.Inverted());
                        Vector3 Size = tnsfm.ExtractScale();
                        float angle = (float)Math.Atan2(-PlaneHitPos.Z * Size.Z, -PlaneHitPos.Y * Size.Y);
                        _ObjectToDragPreviousPosition = new object[] { _ObjectToDrag.rotation, angle };
                        _ObjectToDragPreviousMatrix = tnsfm;
                    }
                    else if (tr.ToolHit == 1)
                    {
                        _IsDragging = tr.ToolHit;
                        _ObjectToDrag = tr.HitObject;
                        Matrix4 tnsfm = _ObjectToDrag.transform;
                        float size = 100;
                        Ray __r = FromMousePos(mtx);
                        float __t = 0;
                        size = _ObjectToDrag.model.size.maxS * 2;
                        __t = MathUtils.intersectPlane(__r, Vector3.TransformNormal(Vector3.UnitY, tnsfm), Vector3.TransformPosition(Vector3.Zero, tnsfm));
                        Vector3 PlaneHitPos = Vector3.TransformPosition(__r.pos + __r.dir * Math.Abs(__t), tnsfm.Inverted());
                        Vector3 Size = tnsfm.ExtractScale();
                        float angle = (float)Math.Atan2(PlaneHitPos.X * Size.X, PlaneHitPos.Z * Size.Z);
                        _ObjectToDragPreviousPosition = new object[] { _ObjectToDrag.rotation, angle };
                        _ObjectToDragPreviousMatrix = tnsfm;
                    }
                    else if (tr.ToolHit == 2)
                    {
                        _IsDragging = tr.ToolHit;
                        _ObjectToDrag = tr.HitObject;
                        Matrix4 tnsfm = _ObjectToDrag.transform;
                        float size = 100;
                        Ray __r = FromMousePos(mtx);
                        float __t = 0;
                        size = _ObjectToDrag.model.size.maxS * 2;
                        __t = MathUtils.intersectPlane(__r, Vector3.TransformNormal(Vector3.UnitZ, tnsfm), Vector3.TransformPosition(Vector3.Zero, tnsfm));
                        Vector3 PlaneHitPos = Vector3.TransformPosition(__r.pos + __r.dir * Math.Abs(__t), tnsfm.Inverted());
                        Vector3 Size = tnsfm.ExtractScale();
                        float angle = (float)Math.Atan2(PlaneHitPos.Y * Size.Y, PlaneHitPos.X * Size.X);
                        _ObjectToDragPreviousPosition = new object[] { _ObjectToDrag.rotation, angle };
                        _ObjectToDragPreviousMatrix = tnsfm;
                    }
                }
            }
            else if (MainForm.current_tool == Tools.Scale)
            {
                Matrix4 mtx = cam.matrix;
                if (e.Button == MouseButtons.Left)
                {
                    if (tr.ToolHit != -1)
                    {
                        _IsDragging = tr.ToolHit;
                        _ObjectToDrag = tr.HitObject;
                        Matrix4 tnsfm = _ObjectToDrag.transform;
                        Vector3 objpos = Vector3.TransformPosition(Vector3.Zero, tnsfm);
                        float ScalePoint = (MathUtils.ClosestPointFromLine(FromMousePos(mtx), objpos) - objpos).Length;
                        _ObjectToDragPreviousPosition = new object[] { _ObjectToDrag.scale, ScalePoint };
                        _ObjectToDragPreviousMatrix = tnsfm;
                    }
                }
            }
            else if (MainForm.current_tool == Tools.Snap)
            {

                if (e.Button == MouseButtons.Left)
                {
                    if (tr.Hit)
                    {
                        float distance = float.PositiveInfinity;
                        Attachment closestAtch = null;
                        foreach (Attachment atch in tr.HitObject.model.attachments)
                        {
                            if (tr.HitObject == Object3D.Active_Object) goto no;
                            foreach (Object3D.attachmentInfo atif in tr.HitObject.atch_info)
                                if (atif.thisAtch == atch)
                                    goto no;
                            float this_dist = (atch.get_world_transform(tr.HitObject.transform).ExtractTranslation() - tr.HitPos).Length;
                            if (distance > this_dist && Object3D.Active_Object.Active_Attachment != null && atch.isFemale != Object3D.Active_Object.Active_Attachment.isFemale
                                && !Object3D.Active_Object.ContainsObjectInChain(tr.HitObject))
                            {
                                closestAtch = atch;
                                distance = this_dist;
                            }
                            no:;
                        }
                        if (closestAtch != null && Object3D.Active_Object != null)
                        {
                            if (Object3D.Active_Object.Active_Attachment != null)
                            {
                                Object3D.Active_Object.attachTo(Object3D.Active_Object.Active_Attachment, closestAtch, tr.HitObject);
                            }
                            else
                                Object3D.Active_Object.attachTo(closestAtch, tr.HitObject);
                        }
                    }
                }
            }
            else if (MainForm.current_tool == Tools.Decorate)
            {

                if (e.Button == MouseButtons.Left)
                {
                    if (Form.ActiveForm is MainForm)
                    {
                        if (((MainForm)Form.ActiveForm).DF.DecorationsList.SelectedItems.Count > 0)
                        {
                            float jit = (float)(((MainForm)Form.ActiveForm).DF.JitterNumeric.Value);
                            TraceResult ltr = FromScreen(mx + lerp(-jit, jit, (float)(r.NextDouble())), my + lerp(-jit, jit, (float)(r.NextDouble())), cam.matrix).Trace(cam, true, false, false, false);

                            if (ltr.Hit)
                            {
                                if (((MainForm)Form.ActiveForm).DF.place_mode != 2)
                                {
                                    DecorationObject dec = new DecorationObject((string)((MainForm)Form.ActiveForm).DF.DecorationsList.SelectedItems[0].Tag, ltr.HitObject);
                                    Matrix4 dirmtx = FromNormal(ltr.HitNormal);
                                    Vector3 relativeCamVector = Vector3.TransformPosition((cam.position - ltr.HitPos), dirmtx.Inverted());
                                    float tilt = (float)Math.Atan2(relativeCamVector.X, -relativeCamVector.Y);
                                    float tiltjitrange = (float)((float)(((MainForm)Form.ActiveForm).DF.TurnJitterNumeric.Value) * Math.PI / 180);
                                    tilt += lerp(-tiltjitrange / 2, tiltjitrange / 2, (float)(r.NextDouble()));
                                    PrevDP = ltr.HitPos;
                                    dec.transform = dirmtx * Matrix4.CreateFromAxisAngle(ltr.HitNormal, tilt) * Matrix4.CreateScale(lerp((float)((MainForm)Form.ActiveForm).DF.SizeJitterMinNumeric.Value, (float)((MainForm)Form.ActiveForm).DF.SizeJitterMaxNumeric.Value, (float)(r.NextDouble()))) * Matrix4.CreateTranslation(ltr.HitPos) * ltr.HitObject.transform.Inverted();
                                    ltr.HitObject.Decorations.Add(dec);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Calculates the rotation matrix from the specified normal. This is pretty much my own algorithm.
        /// Once again this is another math utility that should be moved to the MathUtils class.
        /// </summary>
        /// <param name="normal">The specified normal.</param>
        Matrix4 FromNormal(Vector3 normal)
        {
            Vector3 axisZ = new Vector3(normal.X, normal.Y, 0).Normalized();
            float sinY = -axisZ.X;
            float cosY = axisZ.Y;
            if (float.IsNaN(axisZ.Length))
            {
                sinY = 0;
                cosY = 1;
            }
            Matrix4 mtxZ = new Matrix4(cosY, sinY, 0, 0, -sinY, cosY, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            Matrix4 mtxAngle = new Matrix4(cosY, -sinY, 0, 0, sinY, cosY, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            Vector3 axisX = Vector3.TransformVector(normal, mtxAngle).Normalized();
            float cosX = axisX.Z;
            float sinX = axisX.Y;
            Matrix4 mtxX = new Matrix4(1, 0, 0, 0, 0, cosX, -sinX, 0, 0, sinX, cosX, 0, 0, 0, 0, 1);
            return mtxX * mtxZ;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            cam.zoom = Math.Max(cam.clip_near, cam.zoom-e.Delta/12F* (float)Math.Pow(cam.zoom, 0.375F));
            Matrix4 mtx = cam.matrix;
            MPray = FromMousePos(mtx);
            tr = MPray.Trace(cam, MainForm.current_tool == Tools.Select || MainForm.current_tool == Tools.Snap || MainForm.current_tool == Tools.Decorate, MainForm.current_tool == Tools.Move && _IsDragging == -1, MainForm.current_tool == Tools.Rotate && _IsDragging == -1, MainForm.current_tool == Tools.Scale && _IsDragging == -1);

            Invalidate();
        }
        bool _prevhit = false;
        private Matrix4 _ObjectToDragPreviousMatrix;
        Vector3 PrevDP = new Vector3();
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
            tr = MPray.Trace(cam, MainForm.current_tool == Tools.Select || MainForm.current_tool == Tools.Snap || MainForm.current_tool == Tools.Decorate, MainForm.current_tool == Tools.Move && _IsDragging == -1, MainForm.current_tool == Tools.Rotate && _IsDragging == -1, MainForm.current_tool == Tools.Scale && _IsDragging == -1);
            if (MainForm.current_tool == Tools.Move)
            {
                if (_ObjectToDrag != null)
                {
                    Matrix4 tnsfm = _ObjectToDragPreviousMatrix;
                    if (_IsDragging == 0)
                    {
                        Ray MoveX = new Ray(Vector3.TransformPosition(Vector3.Zero, tnsfm), Vector3.TransformNormal(Vector3.UnitX, tnsfm));
                        Vector3[] cp = MathUtils.ClosestPointsBetweenRays(FromMousePos(mtx), MoveX);
                        Vector3 prevpos = (Vector3)_ObjectToDragPreviousPosition;
                        _ObjectToDrag.position = cp[1] - prevpos;
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                    if (_IsDragging == 1)
                    {
                        Ray MoveY = new Ray(Vector3.TransformPosition(Vector3.Zero, tnsfm), Vector3.TransformNormal(Vector3.UnitY, tnsfm));
                        Vector3[] cp = MathUtils.ClosestPointsBetweenRays(FromMousePos(mtx), MoveY);
                        Vector3 prevpos = (Vector3)_ObjectToDragPreviousPosition;
                        _ObjectToDrag.position = cp[1] - prevpos;
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                    if (_IsDragging == 2)
                    {
                        Ray MoveZ = new Ray(Vector3.TransformPosition(Vector3.Zero, tnsfm), Vector3.TransformNormal(Vector3.UnitZ, tnsfm));
                        Vector3[] cp = MathUtils.ClosestPointsBetweenRays(FromMousePos(mtx), MoveZ);
                        Vector3 prevpos = (Vector3)_ObjectToDragPreviousPosition;
                        _ObjectToDrag.position = cp[1] - prevpos;
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                }
            }
            if (MainForm.current_tool == Tools.Rotate)
            {
                if (_ObjectToDrag != null)
                {
                    Matrix4 tnsfm = _ObjectToDragPreviousMatrix;
                    if (_IsDragging == 0)
                    {
                        float size = 100;
                        Ray __r = FromMousePos(mtx);
                        float __t = 0;
                        size = _ObjectToDrag.model.size.maxS * 2;
                        tnsfm = _ObjectToDragPreviousMatrix;
                        __t = MathUtils.intersectPlane(__r, Vector3.TransformNormal(Vector3.UnitX, tnsfm), Vector3.TransformPosition(Vector3.Zero, tnsfm));
                        Vector3 PlaneHitPos = Vector3.TransformPosition(__r.pos + __r.dir * Math.Abs(__t), tnsfm.Inverted());
                        Vector3 Size = tnsfm.ExtractScale();
                        float angle = (float)Math.Atan2(-PlaneHitPos.Z * Size.Z, -PlaneHitPos.Y * Size.Y);
                        _ObjectToDrag.rotation = ((Quaternion)((object[])(_ObjectToDragPreviousPosition))[0]) * Quaternion.FromAxisAngle(Vector3.UnitX, angle - ((float)((object[])(_ObjectToDragPreviousPosition))[1]));
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                    else if (_IsDragging == 1)
                    {
                        float size = 100;
                        Ray __r = FromMousePos(mtx);
                        float __t = 0;
                        size = _ObjectToDrag.model.size.maxS * 2;
                        tnsfm = _ObjectToDragPreviousMatrix;
                        __t = MathUtils.intersectPlane(__r, Vector3.TransformNormal(Vector3.UnitY, tnsfm), Vector3.TransformPosition(Vector3.Zero, tnsfm));
                        Vector3 PlaneHitPos = Vector3.TransformPosition(__r.pos + __r.dir * Math.Abs(__t), tnsfm.Inverted());
                        Vector3 Size = tnsfm.ExtractScale();
                        float angle = (float)Math.Atan2(PlaneHitPos.X * Size.X, PlaneHitPos.Z * Size.Z);
                        _ObjectToDrag.rotation = ((Quaternion)((object[])(_ObjectToDragPreviousPosition))[0]) * Quaternion.FromAxisAngle(Vector3.UnitY, angle - ((float)((object[])(_ObjectToDragPreviousPosition))[1]));
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                    else if (_IsDragging == 2)
                    {
                        float size = 100;
                        Ray __r = FromMousePos(mtx);
                        float __t = 0;
                        size = _ObjectToDrag.model.size.maxS * 2;
                        tnsfm = _ObjectToDragPreviousMatrix;
                        __t = MathUtils.intersectPlane(__r, Vector3.TransformNormal(Vector3.UnitZ, tnsfm), Vector3.TransformPosition(Vector3.Zero, tnsfm));
                        Vector3 PlaneHitPos = Vector3.TransformPosition(__r.pos + __r.dir * Math.Abs(__t), tnsfm.Inverted());
                        Vector3 Size = tnsfm.ExtractScale();
                        float angle = (float)Math.Atan2(PlaneHitPos.Y * Size.Y, PlaneHitPos.X * Size.X);
                        _ObjectToDrag.rotation = ((Quaternion)((object[])(_ObjectToDragPreviousPosition))[0]) * Quaternion.FromAxisAngle(Vector3.UnitZ, angle - ((float)((object[])(_ObjectToDragPreviousPosition))[1]));
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                }
            }
            if (MainForm.current_tool == Tools.Scale)
            {
                if (_ObjectToDrag != null)
                {
                    Matrix4 tnsfm = _ObjectToDragPreviousMatrix;
                    if (_IsDragging == 0)
                    {
                        Vector3 objpos = Vector3.TransformPosition(Vector3.Zero, tnsfm);
                        float ScalePoint = (MathUtils.ClosestPointFromLine(FromMousePos(mtx), objpos) - objpos).Length;
                        if (ModifierKeys == Keys.Shift)
                            _ObjectToDrag.scale = Vector3.One * ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).X * (ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1]));

                        else
                        {
                            Vector3 scale = ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]);
                            scale.X *= (ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1]));
                            _ObjectToDrag.scale = scale;
                        }
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                    if (_IsDragging == 1)
                    {
                        Vector3 objpos = Vector3.TransformPosition(Vector3.Zero, tnsfm);
                        float ScalePoint = (MathUtils.ClosestPointFromLine(FromMousePos(mtx), objpos) - objpos).Length;
                        if (ModifierKeys == Keys.Shift)
                            _ObjectToDrag.scale = Vector3.One * ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).Y * (ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1]));

                        else
                        {
                            Vector3 scale = ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]);
                            scale.Y *= (ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1]));
                            _ObjectToDrag.scale = scale;
                        }
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                    if (_IsDragging == 2)
                    {
                        Vector3 objpos = Vector3.TransformPosition(Vector3.Zero, tnsfm);
                        float ScalePoint = (MathUtils.ClosestPointFromLine(FromMousePos(mtx), objpos) - objpos).Length;
                        if (ModifierKeys == Keys.Shift)
                            _ObjectToDrag.scale = Vector3.One * ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).Z * (ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1]));

                        else
                        {
                            Vector3 scale = ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]);
                            scale.Z *= (ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1]));
                            _ObjectToDrag.scale = scale;
                        }
                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                    if (_IsDragging == 3)
                    {
                        Vector3 objpos = Vector3.TransformPosition(Vector3.Zero, tnsfm);
                        float ScalePoint = (MathUtils.ClosestPointFromLine(FromMousePos(mtx), objpos) - objpos).Length;
                        if (ModifierKeys == Keys.Shift)
                            _ObjectToDrag.scale = Vector3.One * (Math.Min(((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).Z, Math.Min(((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).X, ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).Y)) + Math.Max(((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).Z, Math.Max(((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).X, ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]).Y))) / 2 * (float)Math.Pow((ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1])), 1 / 2F);

                        else
                            _ObjectToDrag.scale = ((Vector3)((object[])(_ObjectToDragPreviousPosition))[0]) * (float)Math.Pow((ScalePoint / ((float)((object[])(_ObjectToDragPreviousPosition))[1])), 1 / 2F);

                        _ObjectToDrag.FixAttachments();
                        invalidate = true;
                    }
                }
            }
            if (MainForm.current_tool == Tools.Decorate)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (Form.ActiveForm is MainForm)
                        if (((MainForm)Form.ActiveForm).DF.DecorationsList.SelectedItems.Count > 0)
                        {
                            float jit = (float)(((MainForm)Form.ActiveForm).DF.JitterNumeric.Value);
                            TraceResult ltr = FromScreen(mx + lerp(-jit, jit, (float)(r.NextDouble())), my + lerp(-jit, jit, (float)(r.NextDouble())), cam.matrix).Trace(cam, true, false, false, false);
                            if (ltr.Hit)
                            {
                                if (((MainForm)Form.ActiveForm).DF.place_mode == 1)
                                {
                                    if ((PrevDP - ltr.HitPos).Length >= (float)((MainForm)Form.ActiveForm).DF.FlowNumeric.Value * 10)
                                    {
                                        DecorationObject dec = new DecorationObject((string)((MainForm)Form.ActiveForm).DF.DecorationsList.SelectedItems[0].Tag, ltr.HitObject);
                                        Matrix4 dirmtx = FromNormal(ltr.HitNormal);
                                        Vector3 relativeCamVector = Vector3.TransformPosition((cam.position - ltr.HitPos), dirmtx.Inverted());
                                        float tilt = (float)Math.Atan2(relativeCamVector.X, -relativeCamVector.Y);
                                        float tiltjitrange = (float)((float)(((MainForm)Form.ActiveForm).DF.TurnJitterNumeric.Value) * Math.PI / 180);
                                        tilt += lerp(-tiltjitrange / 2, tiltjitrange / 2, (float)(r.NextDouble()));
                                        PrevDP = ltr.HitPos;
                                        dec.transform = dirmtx * Matrix4.CreateFromAxisAngle(ltr.HitNormal, tilt) * Matrix4.CreateScale(lerp((float)((MainForm)Form.ActiveForm).DF.SizeJitterMinNumeric.Value, (float)((MainForm)Form.ActiveForm).DF.SizeJitterMaxNumeric.Value, (float)(r.NextDouble()))) * Matrix4.CreateTranslation(ltr.HitPos) * ltr.HitObject.transform.Inverted();
                                        ltr.HitObject.Decorations.Add(dec);
                                    }
                                }
                                else if (((MainForm)Form.ActiveForm).DF.place_mode == 2)
                                {
                                    DecorationObject[] allDOs = ltr.HitObject.Decorations.ToArray();
                                    foreach (DecorationObject DO in allDOs)
                                    {
                                        if (((DO.transform * ltr.HitObject.transform).ExtractTranslation()-ltr.HitPos).Length <= MainForm.decorate_erase_mode)
                                        DO.Dispose();
                                    }
                                }
                            }
                        }
                }
            }
            if (tr.Hit || invalidate || _prevhit) Invalidate();
            _prevhit = tr.Hit || (tr.ToolHit != -1);
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
