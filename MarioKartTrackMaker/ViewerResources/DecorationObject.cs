using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioKartTrackMaker.ViewerResources
{
    public class DecorationObject
    {
        public Matrix4 transform = Matrix4.Identity;
        private int _mesh;
        public DecorationMesh mesh
        {
            get
            {
                return DecorationMesh.database[_mesh];
            }
        }
        public DecorationObject(string filepath)
        {
            _mesh = DecorationMesh.AddMesh(filepath);
        }
        public void DrawObject(int program, bool wireframe)
        {
            GL.PushMatrix();
            GL.MultMatrix(ref transform);
            mesh.DrawMesh(program, wireframe);
            GL.PopMatrix();
        }
    }
}
