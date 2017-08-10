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
        private int _model;
        public struct attachmentInfo
        {
            public Attachment thisAtch;
            public Object3D thisObject;
            public Attachment thatAtch;
            public Object3D AttachedTo;

            public void Attach()
            {
                thisObject.transform = thisAtch.transform.Inverted()*thatAtch.transform*AttachedTo.transform;
                
                attachmentInfo ata = new attachmentInfo();
                for(int i = 0; i < AttachedTo.atch_info.Count; i++)
                {
                    if (AttachedTo.atch_info[i].thisAtch == thatAtch)
                    {
                        AttachedTo.atch_info.RemoveAt(i);
                        break;
                    }
                }
                ata.thisObject = AttachedTo;
                ata.thisAtch = thatAtch;
                ata.thatAtch = thisAtch;
                ata.AttachedTo = thisObject;
                AttachedTo.atch_info.Add(ata);
            }
        }
        public List<attachmentInfo> atch_info = new List<attachmentInfo>();
        public Model model
        {
            get
            {
                return Model.database[_model];
            }
        }
        public Vector3 position = Vector3.Zero;
        public Quaternion rotation = Quaternion.Identity;
        public Vector3 scale = Vector3.One;
        public Matrix4 transform
        {
            get
            {
                Matrix4 mat = Matrix4.Mult(Matrix4.CreateScale(scale), Matrix4.Mult(Matrix4.CreateFromQuaternion(rotation), Matrix4.CreateTranslation(position)));
                return mat;
            }
            set
            {
                position = value.ExtractTranslation();
                rotation = value.ExtractRotation();
                scale = value.ExtractScale();
            }
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

        public void DrawTool()
        {
            GL.PushMatrix();
            Matrix4 mat = transform;
            GL.MultMatrix(ref mat);
            switch (Form1.current_tool)
            {
                case Tools.Select:
                    break;
                case Tools.Move:
                    ToolModels.DrawMoveTool(model.size.maxS * 2);
                    break;
                case Tools.Rotate:
                    ToolModels.DrawRotateTool(model.size.maxS * 2);
                    break;
                case Tools.Scale:
                    ToolModels.DrawScaleTool(model.size.maxS * 2);
                    break;
                case Tools.Snap:
                    ToolModels.DrawConnectTool(model.size.maxS * 2);
                    break;
                case Tools.Decorate:
                    break;
            }
            GL.PopMatrix();
        }
        public void DrawObject(int program, int collision_mode, bool wireframe, bool inSight)
        {
            GL.PushMatrix();
            Matrix4 mat = transform;
            GL.MultMatrix(ref mat);
            if (this == Active_Object)
            {
                DrawAttachments();
            }
            if (inSight)
                model.DrawModel(program, collision_mode, wireframe, this == Active_Object && (Form1.current_tool == Tools.Select || Form1.current_tool == Tools.Snap));
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
            for (int i = 0; i < atch_info.Count; i++)
            {
                if (atch_info[i].thisAtch == thatatch)
                {
                    atch_info.RemoveAt(i);
                    break;
                }
            }
            attachmentInfo new_atif = new attachmentInfo();
            new_atif.thisAtch = thisatch;
            new_atif.thatAtch = thatatch;
            new_atif.thisObject = this;
            new_atif.AttachedTo = targetObj;
            new_atif.Attach();
            atch_info.Add(new_atif);
        }
    }
}
