using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace MarioKartTrackMaker.ViewerResources
{
    public class Bounds
    {
        public float minX, maxX, minY, maxY, minZ, maxZ = 0;
        public float minS
        {
            get
            {
                float rslt = Math.Abs(minX);
                rslt = Math.Min(rslt, Math.Abs(maxX));
                rslt = Math.Min(rslt, Math.Abs(minY));
                rslt = Math.Min(rslt, Math.Abs(maxY));
                rslt = Math.Min(rslt, Math.Abs(minZ));
                rslt = Math.Min(rslt, Math.Abs(maxZ));
                return rslt;
            }
        }
        public float maxS
        {
            get
            {
                float rslt = Math.Abs(minX);
                rslt = Math.Max(rslt, Math.Abs(maxX));
                rslt = Math.Max(rslt, Math.Abs(minY));
                rslt = Math.Max(rslt, Math.Abs(maxY));
                rslt = Math.Max(rslt, Math.Abs(minZ));
                rslt = Math.Max(rslt, Math.Abs(maxZ));
                return rslt;
            }
        }
        public float average
        {
            get
            {
                return (minS + maxS) / 2f;
            }
        }
        public Vector3 nXnYnZ
        {
            get { return new Vector3(minX, minY, minZ); }
        }
        public Vector3 pXnYnZ
        {
            get { return new Vector3(maxX, minY, minZ); }
        }
        public Vector3 nXpYnZ
        {
            get { return new Vector3(minX, maxY, minZ); }
        }
        public Vector3 nXnYpZ
        {
            get { return new Vector3(minX, minY, maxZ); }
        }
        public Vector3 pXpYnZ
        {
            get { return new Vector3(maxX, maxY, minZ); }
        }
        public Vector3 pXnYpZ
        {
            get { return new Vector3(maxX, minY, maxZ); }
        }
        public Vector3 nXpYpZ
        {
            get { return new Vector3(minX, maxY, maxZ); }
        }
        public Vector3 pXpYpZ
        {
            get { return new Vector3(maxX, maxY, maxZ); }
        }
        public void DrawBounds()
        {
            GL.Color3(1F, 1F, 1F);
            GL.Begin(PrimitiveType.Lines);
            {
                GL.Vertex3(minX, minY, minZ);
                GL.Vertex3(maxX, minY, minZ);
                GL.Vertex3(minX, maxY, minZ);
                GL.Vertex3(maxX, maxY, minZ);
                GL.Vertex3(minX, minY, maxZ);
                GL.Vertex3(maxX, minY, maxZ);
                GL.Vertex3(minX, maxY, maxZ);
                GL.Vertex3(maxX, maxY, maxZ);
                GL.Vertex3(minX, minY, minZ);
                GL.Vertex3(minX, maxY, minZ);
                GL.Vertex3(maxX, minY, minZ);
                GL.Vertex3(maxX, maxY, minZ);
                GL.Vertex3(minX, minY, maxZ);
                GL.Vertex3(minX, maxY, maxZ);
                GL.Vertex3(maxX, minY, maxZ);
                GL.Vertex3(maxX, maxY, maxZ);
                GL.Vertex3(minX, minY, minZ);
                GL.Vertex3(minX, minY, maxZ);
                GL.Vertex3(maxX, minY, minZ);
                GL.Vertex3(maxX, minY, maxZ);
                GL.Vertex3(minX, maxY, minZ);
                GL.Vertex3(minX, maxY, maxZ);
                GL.Vertex3(maxX, maxY, minZ);
                GL.Vertex3(maxX, maxY, maxZ);
            }
            GL.End();
        }
        public Bounds MultMtx(Matrix4 mtx)
        {
            Bounds b = new Bounds();
            //old Min/Max as in before new min/max calculation
            Vector3[] TransformedPoints = new Vector3[] {
            Vector3.TransformPosition(new Vector3(minX, minY, minZ), mtx),
            Vector3.TransformPosition(new Vector3(maxX, minY, minZ), mtx),
            Vector3.TransformPosition(new Vector3(minX, maxY, minZ), mtx),
            Vector3.TransformPosition(new Vector3(maxX, maxY, minZ), mtx),
            Vector3.TransformPosition(new Vector3(minX, minY, maxZ), mtx),
            Vector3.TransformPosition(new Vector3(maxX, minY, maxZ), mtx),
            Vector3.TransformPosition(new Vector3(minX, maxY, maxZ), mtx),
            Vector3.TransformPosition(new Vector3(maxX, maxY, maxZ), mtx)};
            b.minX = float.PositiveInfinity;
            b.minY = float.PositiveInfinity;
            b.minZ = float.PositiveInfinity;
            b.maxX = float.NegativeInfinity;
            b.maxY = float.NegativeInfinity;
            b.maxZ = float.NegativeInfinity;
            foreach (Vector3 p in TransformedPoints)
            {
                b.minX = Math.Min(b.minX, p.X);
                b.maxX = Math.Max(b.maxX, p.X);
                b.minY = Math.Min(b.minY, p.Y);
                b.maxY = Math.Max(b.maxY, p.Y);
                b.minZ = Math.Min(b.minZ, p.Z);
                b.maxZ = Math.Max(b.maxZ, p.Z);
            }
            return b;
        }
    }
    public class Mesh
    {
        public List<Vector3> Vertices;
        public List<Vector3> Normals;
        public List<Vector2> UVs;
        public List<Bounds> faceBounds = new List<Bounds>();
        public List<int[]> faces;
        public List<int[]> fnmls;
        public List<int[]> fuvs;
        public Vector3 Color = Vector3.One;
        public int texture;
        public Bounds size = new Bounds();
        public Mesh(List<Vector3> Vertices, List<Vector3> Normals, List<Vector2> UVs, List<int[]> faces, List<int[]> fnmls, List<int[]> fuvs, Vector3 Color, int texture)
        {
            this.Vertices = Vertices;
            this.Normals = Normals;
            this.UVs = UVs;
            this.faces = faces;
            this.fnmls = fnmls;
            this.fuvs = fuvs;
            this.Color = Color;
            this.texture = texture;
            Optimize();
            CalculateBounds();
            CalculateFaceBounds();
        }

        private void CalculateFaceBounds()
        {
            foreach (int[] f in faces)
            {
                Bounds size = new Bounds();
                size.minX = float.PositiveInfinity;
                size.minY = float.PositiveInfinity;
                size.minZ = float.PositiveInfinity;
                size.maxX = float.NegativeInfinity;
                size.maxY = float.NegativeInfinity;
                size.maxZ = float.NegativeInfinity;

                foreach (int i in f)
                {
                    Vector3 v = Vertices[i - 1];
                    size.minX = Math.Min(size.minX, v.X);
                    size.maxX = Math.Max(size.maxX, v.X);
                    size.minY = Math.Min(size.minY, v.Y);
                    size.maxY = Math.Max(size.maxY, v.Y);
                    size.minZ = Math.Min(size.minZ, v.Z);
                    size.maxZ = Math.Max(size.maxZ, v.Z);
                }
                faceBounds.Add(size);
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

        private void RemoveVertex(int index)
        {
            Vertices.RemoveAt(index);
            List<int[]> faces_to_remove = new List<int[]>();
            for (int f = 0; f < faces.Count; f++)
            {
                for(int i = 0; i < faces[f].Length; i++)
                {
                    if (faces[f][i]-1 == index)
                    {
                        faces_to_remove.Add(faces[f]);
                        break;
                    }
                    else if (faces[f][i]-1 > index)
                        faces[f][i]--;
                }
            }
            foreach (int[] f in faces_to_remove)
            {
                int i = faces.IndexOf(f);
                faces.RemoveAt(i);
                if (fnmls.Count > 0)
                {
                    fnmls.RemoveAt(i);
                }
                if (fuvs.Count > 0)
                {
                    fuvs.RemoveAt(i);
                }
            }
        }
        private void RemoveUV(int index)
        {
            UVs.RemoveAt(index);
            for (int f = 0; f < fuvs.Count; f++)
            {
                for (int i = 0; i < fuvs[f].Length; i++)
                {
                    if (fuvs[f][i] > index) fuvs[f][i]--;
                }
            }
        }
        private void RemoveNormal(int index)
        {
            Normals.RemoveAt(index);
            for (int f = 0; f < fnmls.Count; f++)
            {
                for (int i = 0; i < fnmls[f].Length; i++)
                {
                    if (fnmls[f][i] > index) fnmls[f][i]--;
                }
            }
        }
        public static int amountremoved = 0;
        private bool InFaces(int index)
        {
            foreach (int[] f in faces)
                foreach (int i in f)
                    if (i - 1 == index)
                        return true;
            return false;
        }
        private bool InFnmls(int index)
        {
            foreach (int[] f in fnmls)
                foreach (int i in f)
                    if (i - 1 == index)
                        return true;
            return false;
        }
        private bool InFuvs(int index)
        {
            foreach (int[] f in fuvs)
                foreach (int i in f)
                    if (i - 1 == index)
                        return true;
            return false;
        }
        private void Optimize()
        {
            List<int> Verts_to_remove = new List<int>();
            for(int i = 0; i < Vertices.Count; i++)
            {
                if (!InFaces(i))
                {
                    Verts_to_remove.Add(i);
                }
            }
            for (int i = 0; i < Verts_to_remove.Count; i++)
            {
                RemoveVertex(Verts_to_remove[i]);
                for(int ii = 0; ii < Verts_to_remove.Count; ii++)
                {
                    if (Verts_to_remove[i] < Verts_to_remove[ii])
                        Verts_to_remove[ii]--;
                }
            }
            List<int> Norms_to_remove = new List<int>();
            for (int i = 0; i < Normals.Count; i++)
            {
                if (!InFnmls(i))
                {
                    Norms_to_remove.Add(i);
                }
            }
            for (int i = 0; i < Norms_to_remove.Count; i++)
            {
                RemoveNormal(Norms_to_remove[i]);
                for (int ii = 0; ii < Norms_to_remove.Count; ii++)
                {
                    if (Norms_to_remove[i] < Norms_to_remove[ii])
                        Norms_to_remove[ii]--;
                }
            }
            List<int> UVs_to_remove = new List<int>();
            for (int i = 0; i < UVs.Count; i++)
            {
                if (!InFuvs(i))
                {
                    UVs_to_remove.Add(i);
                }
            }
            for (int i = 0; i < UVs_to_remove.Count; i++)
            {
                RemoveUV(UVs_to_remove[i]);
                amountremoved++;
                for (int ii = 0; ii < UVs_to_remove.Count; ii++)
                {
                    if (UVs_to_remove[i] < UVs_to_remove[ii])
                        UVs_to_remove[ii]--;
                }
            }
        }

        public void DrawMesh(int program, bool wireframe, bool selected)
        {
            GL.UseProgram(program);
            int texloc = GL.GetUniformLocation(program, "texture");
            int usetexLoc = GL.GetUniformLocation(program, "useTexture");
            int selloc = GL.GetUniformLocation(program, "selected");
            GL.Uniform1(selloc, (selected)?1:0);
            if (texture != -1)
            {
                GL.Uniform1(texloc, texture - 1);
                GL.Uniform1(usetexLoc, 1);
            }
            else
            {
                GL.Uniform1(usetexLoc, 0);
            }
            GL.Color3(Color);
            
            if (wireframe)
            {
                GL.Begin(PrimitiveType.Lines);
                for (int f = 0; f < Math.Min(faces.Count, Math.Min(fnmls.Count, fuvs.Count)); f++)
                {
                    for (int i = 0; i < 3; i += 3)
                    {
                        if (fnmls[f][0] - 1 >= 0) GL.Normal3(Normals[fnmls[f][0] - 1]);
                        if (fuvs[f][0] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][0] - 1]);
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                        if (fnmls[f][i + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f][i + 1] - 1]);
                        if (fuvs[f][i + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        if (fnmls[f][i + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f][i + 1] - 1]);
                        if (fuvs[f][i + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        if (fnmls[f][i + 2] - 1 >= 0) GL.Normal3(Normals[fnmls[f][i + 2] - 1]);
                        if (fuvs[f][i + 2] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][i + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                        if (fnmls[f][i + 2] - 1 >= 0) GL.Normal3(Normals[fnmls[f][i + 2] - 1]);
                        if (fuvs[f][i + 2] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][i + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                        if (fnmls[f][0] - 1 >= 0) GL.Normal3(Normals[fnmls[f][0] - 1]);
                        if (fuvs[f][0] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][0] - 1]);
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
                        if (fnmls[f][0] - 1 >= 0) GL.Normal3(Normals[fnmls[f][0] - 1]);
                        if (fuvs[f][0] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][0] - 1]);
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                        if (fnmls[f][i + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f][i + 1] - 1]);
                        if (fuvs[f][i + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        if (fnmls[f][i + 2] - 1 >= 0) GL.Normal3(Normals[fnmls[f][i + 2] - 1]);
                        if (fuvs[f][i + 2] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f][i + 2] - 1]);
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
