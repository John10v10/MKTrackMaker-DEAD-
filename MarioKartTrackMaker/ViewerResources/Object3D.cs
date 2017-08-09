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
            public Attachment thatAtch;
            public Object3D AttachedTo;
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
        public Vector3 rotation = Vector3.Zero;
        public Vector3 scale = Vector3.One;
        public Matrix4 transform
        {
            get
            {
                Matrix4 mat = Matrix4.Mult(Matrix4.CreateScale(scale), Matrix4.Mult(Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation)), Matrix4.CreateTranslation(position)));
                return mat;
            }
        }
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
        public void DrawObject(int program, int collision_mode, bool wireframe)
        {
            GL.PushMatrix();
            Matrix4 mat = transform;
            GL.MultMatrix(ref mat);
            if (this == Active_Object)
            {
                DrawAttachments();
            }
            model.DrawModel(program, collision_mode, wireframe);
            GL.PopMatrix();
        }

        private void DrawAttachments()
        {
            foreach (Attachment atch in model.attachments)
            {
                foreach (attachmentInfo atif in atch_info)
                    if (atif.thisAtch == atch)
                        goto no;
                atch.draw(model.size.average);
                no:;
            }
        }
    }
}
