using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MarioKartTrackMaker.ViewerResources
{
    public class Mesh
    {
        public List<Vector3> Verts;
        public List<Vector3> Normals;
        public List<Vector2> UVs;
        public List<int> faces;
        public Mesh()
        {

        }
    }
}
