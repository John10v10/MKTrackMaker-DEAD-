using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ObjParser;
using MarioKartTrackMaker.IO;

namespace MarioKartTrackMaker.ViewerResources
{
    public class Mesh
    {
        public List<Vector3> Vertices;
        public List<Vector3> Normals;
        public List<Vector2> UVs;
        public List<int[]> faces;
        public List<int[]> fnmls;
        public List<int[]> fuvs;
        public Vector3 Color = Vector3.One;
        public int texture;
        public Mesh(string filepath)
        {
            Obj tobj = new Obj();
            tobj.LoadObj(filepath);
            
            texture = ContentPipe.Load_and_AddTexture(@"Parts_n_Models\Planetary\TEX_Saturn.png");
            Vertices = new List<Vector3>();
            foreach (ObjParser.Types.Vertex v in tobj.VertexList)
            {
                Vertices.Add(new Vector3((float)v.X, (float)v.Y, (float)v.Z));
            }
            Normals = new List<Vector3>();
            foreach (ObjParser.Types.Normal vn in tobj.NormalList)
            {
                Normals.Add(new Vector3((float)vn.X, (float)vn.Y, (float)vn.Z));
            }
            UVs = new List<Vector2>();
            foreach (ObjParser.Types.TextureVertex vt in tobj.TextureList)
            {
                UVs.Add(new Vector2((float)vt.X, (float)vt.Y));
            }
            faces = new List<int[]>();
            fnmls = new List<int[]>();
            fuvs = new List<int[]>();
            foreach (ObjParser.Types.Face f in tobj.FaceList)
            {
                faces.Add(f.VertexIndexList);
                fnmls.Add(f.NormalIndexList);
                fuvs.Add(f.TextureVertexIndexList);
            }
        }
        public void DrawMesh(int program, bool wireframe)
        {
            GL.UseProgram(program);
            GL.Color3(Color);
            if (wireframe)
            {
                GL.Begin(PrimitiveType.Lines);
                for (int f = 0; f < Math.Min(faces.Count, Math.Min(fnmls.Count, fuvs.Count)); f++)
                {
                    for (int i = 0; i < 3; i += 3)
                    {
                        GL.Normal3(Normals[fnmls[f][0] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][0] - 1]);
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                        GL.Normal3(Normals[fnmls[f][i + 1] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        GL.Normal3(Normals[fnmls[f][i + 1] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        GL.Normal3(Normals[fnmls[f][i + 2] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][i + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                        GL.Normal3(Normals[fnmls[f][i + 2] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][i + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                        GL.Normal3(Normals[fnmls[f][0] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][0] - 1]);
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                    }
                }
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                for(int f = 0; f < Math.Min(faces.Count, Math.Min(fnmls.Count, fuvs.Count)); f++)
                {

                    for (int i = 0; i < Math.Min(faces[f].Length, Math.Min(fnmls[f].Length, fuvs[f].Length)) - 2; i++)
                    {
                        GL.Normal3(Normals[fnmls[f][0] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][0] - 1]);
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                        GL.Normal3(Normals[fnmls[f][i + 1] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        GL.Normal3(Normals[fnmls[f][i + 2] - 1]);
                        GL.TexCoord2(UVs[fuvs[f][i + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                    }
                }
                GL.End();
            }
            GL.Color3(Vector3.One);
            GL.UseProgram(0);
        }
    }
}
