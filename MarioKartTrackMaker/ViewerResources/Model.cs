using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarioKartTrackMaker.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ObjParser;

namespace MarioKartTrackMaker.ViewerResources
{
    class Model
    {
        public List<Mesh> meshes = new List<Mesh>();
        public Model(string filepath)
        {
            Obj tobj = new Obj();
            tobj.LoadObj(filepath);
            Mtl tmtl = new Mtl();
            tmtl.LoadMtl(Path.GetDirectoryName(filepath) + "\\" + tobj.Mtl);

            List<Vector3> Vertices = new List<Vector3>();
            foreach (ObjParser.Types.Vertex v in tobj.VertexList)
            {
                Vertices.Add(new Vector3((float)v.X, (float)v.Y, (float)v.Z));
            }
            List<Vector3> Normals = new List<Vector3>();
            foreach (ObjParser.Types.Normal vn in tobj.NormalList)
            {
                Normals.Add(new Vector3((float)vn.X, (float)vn.Y, (float)vn.Z));
            }
            List<Vector2> UVs = new List<Vector2>();
            foreach (ObjParser.Types.TextureVertex vt in tobj.TextureList)
            {
                UVs.Add(new Vector2((float)vt.X, (float)vt.Y));
            }
            foreach (ObjParser.Types.Material mat in tmtl.MaterialList)
            {
                List<int[]> faces = new List<int[]>();
                List<int[]> fnmls = new List<int[]>();
                List<int[]> fuvs = new List<int[]>();
                int texture = ContentPipe.Load_and_AddTexture(Path.GetDirectoryName(filepath) + "\\" + mat.DiffuseTexture);

                foreach (ObjParser.Types.Face f in tobj.FaceList)
                {
                    if (f.UseMtl == mat.Name)
                    {
                        faces.Add(f.VertexIndexList);
                        fnmls.Add(f.NormalIndexList);
                        fuvs.Add(f.TextureVertexIndexList);
                    }
                }
                List<Vector3> cverts = new List<Vector3>();
                List<Vector3> cnorms = new List<Vector3>();
                List<Vector2> cuvs = new List<Vector2>();
                foreach (Vector3 v in Vertices)
                    cverts.Add(v);
                foreach (Vector3 n in Normals)
                    cnorms.Add(n);
                foreach (Vector2 t in UVs)
                    cuvs.Add(t);
                meshes.Add(new Mesh(cverts, cnorms, cuvs, faces, fnmls, fuvs, Vector3.One, texture));

            }
        }
        public void DrawModel(int program, bool wireframe)
        {
            foreach (Mesh mesh in meshes)
            {
                mesh.DrawMesh(program, wireframe);
            }
        }
    }
}
