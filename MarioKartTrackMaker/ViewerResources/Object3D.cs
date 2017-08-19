using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
namespace MarioKartTrackMaker.ViewerResources
{
    public class Object3D
    {
        public static List<Object3D> database = new List<Object3D>();
        public static Object3D Active_Object;
        public static List<Object3D> Selected_Objects = new List<Object3D>();
        public class attachmentInfo
        {
            public Attachment thisAtch;
            public Object3D thisObject;
            public Attachment thatAtch;
            public attachmentInfo thatAtchInfo;
            public Object3D AttachedTo;

            public void Attach()
            {
                thisObject.transform = thisAtch.transform.Inverted()*thatAtch.transform*AttachedTo.transform;
                
                foreach (attachmentInfo atin in AttachedTo.atch_info)
                {
                    if (
                        (atin.thisObject == AttachedTo) &&
                        (atin.thisAtch == thatAtch) &&
                        (atin.thatAtch == thisAtch) &&
                        (atin.AttachedTo == thisObject)
                        )
                        goto no;
                }
                attachmentInfo ata = new attachmentInfo();
                ata.thisObject = AttachedTo;
                ata.thisAtch = thatAtch;
                ata.thatAtchInfo = this;
                ata.thatAtch = thisAtch;
                ata.AttachedTo = thisObject;
                AttachedTo.atch_info.Add(ata);
                thatAtchInfo = ata;
                no:;
            }
        }
        public List<attachmentInfo> atch_info = new List<attachmentInfo>();
        public List<DecorationObject> Decorations = new List<DecorationObject>();
        private int _model;
        public Model model
        {
            get
            {
                return Model.database[_model];
            }
        }
        private Vector3 _position = Vector3.Zero;
        private Quaternion _rotation = Quaternion.Identity;
        private Vector3 _scale = Vector3.One;
        public Vector3 position {
            get { return _position; }
            set
            {
                _position = value;
                FixTransform();
            }
        }
        public Quaternion rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                FixTransform();
            }
        }
        public Vector3 scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                FixTransform();
            }
        }
        private Matrix4 _transform = Matrix4.Identity;

        public Matrix4 transform
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
                _position = value.ExtractTranslation();
                _rotation = value.ExtractRotation();
                _scale = value.ExtractScale();
            }
        }

        public Vector3 Color = Vector3.One;

        private void FixTransform()
        {
            _transform = Matrix4.Mult(Matrix4.CreateScale(scale), Matrix4.Mult(Matrix4.CreateFromQuaternion(rotation), Matrix4.CreateTranslation(position)));
        }
        public Attachment Active_Attachment;

        public Object3D(string filepath)
        {
            _model = Model.AddModel(filepath);
        }
        public override string ToString()
        {
            return string.Format("{0} [{1}, {2}, {3}]", model.name, Math.Round(position.X, 1), Math.Round(position.Y, 1), Math.Round(position.Z, 1));
        }

        private void _FixAttachments(ref List<attachmentInfo> Fixed_Attachments)
        {
            for (int i = 0; i < atch_info.Count; i++)
            {
                if (!Fixed_Attachments.Contains(atch_info[i]))
                {
                    atch_info[i].thatAtchInfo.Attach();
                    Fixed_Attachments.Add(atch_info[i]);
                    Fixed_Attachments.Add(atch_info[i].thatAtchInfo);
                    atch_info[i].AttachedTo._FixAttachments(ref Fixed_Attachments);
                }
            }
        }
        public void FixAttachments()
        {
            List<attachmentInfo> Fixed_Attachments = new List<attachmentInfo>();
            _FixAttachments(ref Fixed_Attachments);
        }

        private bool _ContainsAttachmentInChain(Object3D obj, ref List<Object3D> Passed_Objects)
        {
            bool FOUND = false;
            for (int i = 0; i < atch_info.Count; i++)
            {
                    if (atch_info[i].AttachedTo == obj) return true;
                    Passed_Objects.Add(this);
                    if(!Passed_Objects.Contains(atch_info[i].AttachedTo))
                    {
                        FOUND = atch_info[i].AttachedTo._ContainsAttachmentInChain(obj, ref Passed_Objects);
                    if (FOUND) return true;
                }
            }
            return FOUND;
        }
        public bool ContainsObjectInChain(Object3D obj)
        {
            List<Object3D> Passed_Objects = new List<Object3D>();
            return _ContainsAttachmentInChain(obj, ref Passed_Objects);
        }

        public void DrawTool(int HoverState)
        {
            GL.PushMatrix();
            Matrix4 mat = transform;
            GL.MultMatrix(ref mat);
            switch (MainForm.current_tool)
            {
                case Tools.Select:
                    break;
                case Tools.Move:
                    ToolModels.DrawMoveTool(HoverState, model.size.maxS * ((this == Active_Object)?1:0.25f));
                    break;
                case Tools.Rotate:
                    ToolModels.DrawRotateTool(HoverState, model.size.maxS * ((this == Active_Object) ? 1 : 0.5f));
                    break;
                case Tools.Scale:
                    ToolModels.DrawScaleTool(HoverState, model.size.maxS * ((this == Active_Object) ? 1 : 0.25f));
                    break;
                case Tools.Snap:
                    ToolModels.DrawConnectTool(model.size.maxS * ((this == Active_Object) ? 1 : 0.25f));
                    break;
                case Tools.Decorate:
                    break;
            }
            GL.PopMatrix();
        }
        public void DrawObject(int program, int collision_mode, bool wireframe, bool inSight, Camera cam)
        {
            GL.PushMatrix();
            Matrix4 mat = transform;
            if (this == Active_Object || MainForm.current_tool == Tools.Snap)
            {

                GL.PushMatrix();
                GL.MultMatrix(ref mat);
                DrawAttachments();
                GL.PopMatrix();
            }
            if (inSight)
                model.DrawModel(program, mat, collision_mode, wireframe, this == Active_Object && (MainForm.current_tool == Tools.Select || MainForm.current_tool == Tools.Snap), (model.useColor)?(Color):(Vector3.One));

            GL.PopMatrix();
            GL.PushMatrix();
            GL.MultMatrix(ref mat);
            foreach (DecorationObject decobj in Decorations)
            {
                if(ViewPortPanel.inSight(decobj, this, cam))
                    decobj.DrawObject(program, collision_mode, wireframe);
            }
            GL.PopMatrix();
        }

        private void DrawAttachments()
        {
            foreach (Attachment atch in model.attachments)
            {
                foreach (attachmentInfo atif in atch_info)
                    if (atif.thisAtch == atch)
                        goto no;
                atch.draw(model.size.average, (atch == Active_Attachment)?1:0);
                no:;
            }
        }

        public bool attachTo(Attachment atch, Object3D targetObj)
        {
            foreach(Attachment thisatch in model.attachments)
            {
                if (thisatch.isFirst == ((atch.isFemale)?2:1))

                {
                    attachTo(thisatch, atch, targetObj);
                    return true;
                }
            }
            return false;
        }
        public void attachTo(Attachment thisatch, Attachment thatatch, Object3D targetObj)
        {
            
            foreach (attachmentInfo atin in atch_info)
            {
                if (
                    (atin.thisObject == this) &&
                    (atin.thisAtch == thisatch) &&
                    (atin.thatAtch == thatatch) &&
                    (atin.AttachedTo == targetObj)
                    )
                    goto no;
            }
            attachmentInfo new_atif = new attachmentInfo();
            new_atif.thisAtch = thisatch;
            new_atif.thatAtch = thatatch;
            new_atif.thisObject = this;
            new_atif.AttachedTo = targetObj;
            new_atif.Attach();
            atch_info.Add(new_atif);
            if (Active_Attachment == thisatch)
            {
                Active_Attachment = null;
                foreach (Attachment atch in model.attachments)
                {
                    foreach (attachmentInfo atif in atch_info)
                        if (atif.thisAtch == atch)
                            continue;
                    Active_Attachment = atch;
                }
            }
            List<attachmentInfo> Fixed_Attachments = new List<attachmentInfo>();
            Fixed_Attachments.Add(new_atif);
            _FixAttachments(ref Fixed_Attachments);
            no:;
        }
    }
}
