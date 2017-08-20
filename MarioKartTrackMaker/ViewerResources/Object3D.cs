using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
namespace MarioKartTrackMaker.ViewerResources
{
    /// <summary>
    /// This class defines a track part or track element.
    /// </summary>
    public class Object3D
    {
        /// <summary>
        /// The database of all the 3D objects in the scene.
        /// </summary>
        public static List<Object3D> database = new List<Object3D>();
        /// <summary>
        /// Defines the active object in the scene.
        /// </summary>
        public static Object3D Active_Object;
        /// <summary>
        /// This defines all the secondary selected 3D objects. This is currently useless right now, but I plan to use it in the future.
        /// </summary>
        public static List<Object3D> Selected_Objects = new List<Object3D>();
        /// <summary>
        /// This structure defines that attachments on this 3D object are actually connected to another attachment and object.
        /// </summary>
        public class attachmentInfo
        {
            public Attachment thisAtch;
            public Object3D thisObject;
            public Attachment thatAtch;
            public attachmentInfo thatAtchInfo;
            public Object3D AttachedTo;

            /// <summary>
            /// Attaches the 3D object to the other 3D object via the other attachment.
            /// </summary>
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
        /// <summary>
        /// The list of all the connected attachments.
        /// </summary>
        public List<attachmentInfo> atch_info = new List<attachmentInfo>();
        /// <summary>
        /// The list of all the decorations that lie on this object.
        /// </summary>
        public List<DecorationObject> Decorations = new List<DecorationObject>();
        /// <summary>
        /// The 3D object's model defined by the database index.
        /// </summary>
        private int _model;
        /// <summary>
        /// Gets the 3D object's model.
        /// </summary>
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
        /// <summary>
        /// The position of the 3D object.
        /// </summary>
        public Vector3 position {
            get { return _position; }
            set
            {
                _position = value;
                FixTransform();
            }
        }
        /// <summary>
        /// The rotation of the 3D object.
        /// </summary>
        public Quaternion rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                FixTransform();
            }
        }
        /// <summary>
        /// The scale of the 3D object.
        /// </summary>
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

        /// <summary>
        /// The overall transformation of the 3D object.
        /// </summary>
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

        /// <summary>
        /// The diffuse color of the 3D object.
        /// </summary>
        public Vector3 Color = Vector3.One;

        /// <summary>
        /// Fixes the transformation matrix on the position, rotation, and scale.
        /// </summary>
        private void FixTransform()
        {
            _transform = Matrix4.Mult(Matrix4.CreateScale(scale), Matrix4.Mult(Matrix4.CreateFromQuaternion(rotation), Matrix4.CreateTranslation(position)));
        }
        /// <summary>
        /// Defines the attachment that is currently selected.
        /// </summary>
        public Attachment Active_Attachment;

        /// <summary>
        /// Let's construct a 3D object!
        /// </summary>
        /// <param name="filepath">The path to import the model file.</param>
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
        /// <summary>
        /// By doing this, if this object is supposed to be attached to another object, but it isn't, it'll attach that object to this object, and then any objects to that objects, and so on.
        /// </summary>
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
        /// <summary>
        /// Checks if the specified 3D object is contained inside a group of attached 3D objects.
        /// </summary>
        /// <param name="obj">The specified 3D object.</param>
        /// <returns></returns>
        public bool ContainsObjectInChain(Object3D obj)
        {
            List<Object3D> Passed_Objects = new List<Object3D>();
            return _ContainsAttachmentInChain(obj, ref Passed_Objects);
        }

        /// <summary>
        /// Draws the current manipulation tool for this 3D object.
        /// </summary>
        /// <param name="HoverState">Which one apears white? Set to -1 if you don't want that.</param>
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
                case Tools.Decorate:
                    break;
            }
            GL.PopMatrix();
        }
        /// <summary>
        /// Renders the 3D object.
        /// </summary>
        /// <param name="program">The id of the shader program.</param>
        /// <param name="collision_mode">The collision mode. 1 displays only the model, 2 displays only the model's collisions, and 3 displays both.</param>
        /// <param name="wireframe">Render this as wireframe?</param>
        /// <param name="inSight">Render the model if the 3D object is in camera's sight.</param>
        /// <param name="cam">The camera.</param>
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

        /// <summary>
        /// Displays the attachments.
        /// </summary>
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

        /// <summary>
        /// This will attach the object to another object.
        /// </summary>
        /// <param name="atch">The target attachment.</param>
        /// <param name="targetObj">The target object.</param>
        /// <returns></returns>
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
        /// <summary>
        /// This will attach the object to another object with the specified attachment.
        /// </summary>
        /// <param name="thisatch">The specified attachment.</param>
        /// <param name="thatatch">The target attachment.</param>
        /// <param name="targetObj">The target object.</param>
        /// <returns></returns>
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
