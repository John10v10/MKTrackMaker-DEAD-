using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using ObjParser;
using MarioKartTrackMaker.IO;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioKartTrackMaker.ViewerResources
{
    class DecorationMesh
    {
        public string path;
        public string name;
        public int Vertex_Count, Normal_Count, UV_Count, Face_Count, Face_Normal_Count, Face_UV_Count = 0;
        public Vector3[] Vertices = new Vector3[128];
        public Vector3[] Normals = new Vector3[128];
        public Vector2[] UVs = new Vector2[128];
        public int[] faces = new int[144];
        public int[] fnmls = new int[144];
        public int[] fuvs = new int[144];

        public static bool runningOnMac;
        public static string filepathSlash;

        public static void DoMacStuff()
        {
            int plat = (int)Environment.OSVersion.Platform;
            if ((plat == 4) || (plat == 128))
            {
                runningOnMac = true;
            }
            else
            {
                runningOnMac = false;
            }

            filepathSlash = "\\";
            if (runningOnMac)
            {
                filepathSlash = "//";
            }
        }

        public int texture;
        public Bounds size = new Bounds();
        public DecorationMesh(string filepath)
        {
            int i;
            path = filepath;
            name = Path.GetFileNameWithoutExtension(filepath).Replace('_', ' ');
            Obj tobj = new Obj();
            tobj.LoadObj(filepath);
            DoMacStuff();
            Mtl tmtl = new Mtl();
            tmtl.LoadMtl(Path.GetDirectoryName(filepath) + filepathSlash + tobj.Mtl);
            if (tobj.VertexList.Count > 128)
            {
                throw new OverflowException("You have too many vertices in this mesh. Maximum count is 128.");
            }
            i = 0;
            foreach (ObjParser.Types.Vertex v in tobj.VertexList)
            {
                Vertices[i] = new Vector3((float)v.X, (float)v.Y, (float)v.Z);
                i++;
            }
            if (tobj.NormalList.Count > 128)
            {
                throw new OverflowException("You have too many normals in this mesh. Maximum count is 128.");
            }
            i = 0;
            foreach (ObjParser.Types.Normal vn in tobj.NormalList)
            {
                Normals[i] = new Vector3((float)vn.X, (float)vn.Y, (float)vn.Z);
                i++;
            }
            if (tobj.TextureList.Count > 128)
            {
                throw new OverflowException("You have too many UV coordinates in this mesh. Maximum count is 128.");
            }
            i = 0;
            foreach (ObjParser.Types.TextureVertex vt in tobj.TextureList)
            {
                UVs[i] = new Vector2((float)vt.X, (float)vt.Y);
                i++;

            }
            ObjParser.Types.Material mat = tmtl.MaterialList[0];
            int texture;
            if (Path.IsPathRooted(mat.DiffuseTexture))
            {
                texture = ContentPipe.Load_and_AddTexture(mat.DiffuseTexture);
            }
            else {
                texture = ContentPipe.Load_and_AddTexture(Path.GetDirectoryName(filepath) + filepathSlash + mat.DiffuseTexture);
            }

            int _i = 0,
                __i = 0,
                ___i = 0;
            foreach (ObjParser.Types.Face f in tobj.FaceList)
            {
                if (f.UseMtl == mat.Name)
                {
                    string error_message = "You have too many Vertex Indexes in this mesh. Maximum count is {0}.";
                    for (int ii = 0; ii < f.VertexIndexList.Length - 2; ii++)
                    {
                        faces[_i] = f.VertexIndexList[0];
                        IncrementThrow(ref _i, error_message, 144);
                        faces[_i] = f.VertexIndexList[ii + 1];
                        IncrementThrow(ref _i, error_message, 144);
                        faces[_i] = f.VertexIndexList[ii + 2];
                        IncrementThrow(ref _i, error_message, 144);
                    }
                    error_message = "You have too many Normal Indexes in this mesh. Maximum count is {0}.";
                    for (int ii = 0; ii < f.NormalIndexList.Length - 2; ii++)
                    {
                        fnmls[__i] = f.NormalIndexList[0];
                        IncrementThrow(ref __i, error_message, 144);
                        fnmls[__i] = f.NormalIndexList[ii + 1];
                        IncrementThrow(ref __i, error_message, 144);
                        fnmls[__i] = f.NormalIndexList[ii + 2];
                        IncrementThrow(ref __i, error_message, 144);
                    }
                    error_message = "You have too many UV Indexes in this mesh. Maximum count is {0}.";
                    for (int ii = 0; ii < f.TextureVertexIndexList.Length - 2; ii++)
                    {
                        fuvs[___i] = f.TextureVertexIndexList[0];
                        IncrementThrow(ref ___i, error_message, 144);
                        fuvs[___i] = f.TextureVertexIndexList[ii + 1];
                        IncrementThrow(ref ___i, error_message, 144);
                        fuvs[___i] = f.TextureVertexIndexList[ii + 2];
                        IncrementThrow(ref ___i, error_message, 144);
                    }
                }
            }
            CalculateBounds();
        }
        private void IncrementThrow(ref int i, string error_message, int max)
        {
            i++;
            if(i >= max)
            {
                throw new OverflowException(string.Format(error_message, max));
            }
        }
        private void CalculateBounds()
        {
            size.minX = float.PositiveInfinity;
            size.minY = float.PositiveInfinity;
            size.minZ = float.PositiveInfinity;
            size.maxX = float.NegativeInfinity;
            size.maxY = float.NegativeInfinity;
            size.maxZ = float.NegativeInfinity;

            foreach (Vector3 v in Vertices)
            {
                size.minX = Math.Min(size.minX, v.X);
                size.maxX = Math.Max(size.maxX, v.X);
                size.minY = Math.Min(size.minY, v.Y);
                size.maxY = Math.Max(size.maxY, v.Y);
                size.minZ = Math.Min(size.minZ, v.Z);
                size.maxZ = Math.Max(size.maxZ, v.Z);
            }
        }
        public void DrawMesh(int program, bool wireframe)
        {
            //GL.UseProgram(program);
            int texloc = GL.GetUniformLocation(program, "texture");
            int usetexLoc = GL.GetUniformLocation(program, "useTexture");
            int selloc = GL.GetUniformLocation(program, "selected");
            GL.Uniform1(selloc, 0);
            if (texture != -1)
            {
                GL.Uniform1(texloc, texture - 1);
                GL.Uniform1(usetexLoc, 1);
            }
            else
            {
                GL.Uniform1(usetexLoc, 0);
            }

            if (wireframe)
            {
                GL.Begin(PrimitiveType.Lines);
                for (int f = 0; f < Math.Min(faces.Length, Math.Min(fnmls.Length, fuvs.Length)); f+=3)
                {
                    if (fnmls[f] - 1 >= 0) GL.Normal3(Normals[fnmls[f] - 1]);
                    if (fuvs[f] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f] - 1]);
                    GL.Vertex3(Vertices[faces[f] - 1]);
                    if (fnmls[f + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f+1] - 1]);
                    if (fuvs[f + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f+1] - 1]);
                    GL.Vertex3(Vertices[faces[f + 1] - 1]);
                    if (fnmls[f + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f+1] - 1]);
                    if (fuvs[f + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f+1] - 1]);
                    GL.Vertex3(Vertices[faces[f + 1] - 1]);
                    if (fnmls[f + 2] - 1 >= 0) GL.Normal3(Normals[fnmls[f + 2] - 1]);
                    if (fuvs[f + 2] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f + 2] - 1]);
                    GL.Vertex3(Vertices[faces[f + 2] - 1]);
                    if (fnmls[f + 2] - 1 >= 0) GL.Normal3(Normals[fnmls[f + 2] - 1]);
                    if (fuvs[f + 2] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f + 2] - 1]);
                    GL.Vertex3(Vertices[faces[f + 2] - 1]);
                    if (fnmls[f] - 1 >= 0) GL.Normal3(Normals[fnmls[f] - 1]);
                    if (fuvs[f] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f] - 1]);
                    GL.Vertex3(Vertices[faces[f] - 1]);
                }
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                for (int f = 0; f < Math.Min(faces.Length, Math.Min(fnmls.Length, fuvs.Length)); f+=3)
                {
                    if (fnmls[f] >= 0) GL.Normal3(Normals[fnmls[f]]);
                    if (fuvs[f]>= 0) GL.TexCoord2(UVs[fuvs[f]]);
                    GL.Vertex3(Vertices[faces[f]]);
                    if (fnmls[f + 1] >= 0) GL.Normal3(Normals[fnmls[f + 1]]);
                    if (fuvs[f + 1] >= 0) GL.TexCoord2(UVs[fuvs[f + 1]]);
                    GL.Vertex3(Vertices[faces[f + 1]]);
                    if (fnmls[f + 2] >= 0) GL.Normal3(Normals[fnmls[f + 2]]);
                    if (fuvs[f + 2] >= 0) GL.TexCoord2(UVs[fuvs[f + 2]]);
                    GL.Vertex3(Vertices[faces[f + 2]]);
                }
                GL.End();
            }
            //GL.UseProgram(0);
        }
    }
}
