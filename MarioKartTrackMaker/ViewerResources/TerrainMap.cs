using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace MarioKartTrackMaker.ViewerResources
{
    public class TerrainMap : Mesh
    {
        static List<TerrainMap> database = new List<TerrainMap>();
        public float textureSize = 1;
        public enum HillShape
        {
            Smooth,
            Linear,
            Contrast,
            Point,
            Bump
        }
        public struct Hill
        {
            public Vector2 Pos;
            public float radius;
            public float height;
            public HillShape shape;
            public float getHeight(float x, float y)
            {
                float d = (new Vector2(x, y) - Pos).Length;
                float linearPoint = Math.Max(0, 1 - d / radius);
                switch (shape)
                {
                    case HillShape.Smooth:
                        return (float)(Math.Sin((linearPoint - 0.5) * Math.PI) + 1) / 2 * height;
                    case HillShape.Linear:
                        return linearPoint * height;
                    case HillShape.Contrast:
                        return (linearPoint > 0) ? height : 0;
                    case HillShape.Point:
                        return linearPoint * linearPoint * height;
                    case HillShape.Bump:
                        return (1-((1-linearPoint) * (1-linearPoint))) * height;
                }
                return 0;
            }
            public Hill(Vector2 pos, float r, float h, HillShape s)
            {
                Pos = pos;
                radius = r;
                height = h;
                shape = s;
            }
        }
        public List<Hill> hills = new List<Hill>();
        public void constructMesh()
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            UVs = new List<Vector2>();
            faces = new List<int[]>();
            fnmls = new List<int[]>();
            fuvs = new List<int[]>();
            int dim = 0;
            int width = 32;
            int height = 32;
            for (float y = -height / 2; y <= height / 2; y+= 1 /2F)
            {
                for (float x = -width / 2; x <= width / 2; x+= 1 / 2F)
                {
                    float hgt = 0;
                    foreach (Hill h in hills)
                    {
                        hgt += h.getHeight(x, y) * 100;
                    }
                    Vertices.Add(new Vector3(x * 100, y * 100, hgt));
                    Normals.Add(new Vector3(0,0,1));
                    UVs.Add(new Vector2(0, 0));
                }
                dim++;
            }
            for (int y = 0; y < dim - 1; y++)
            {
                for (int x = 0; x < dim - 1; x++)
                {
                    faces.Add(new int[] { (y) * dim + x + 1, (y) * dim + x + 2, (y + 1) * dim + x + 2, (y + 1) * dim + x + 1 });
                    fnmls.Add(new int[] { (y) * dim + x + 1, (y) * dim + x + 2, (y + 1) * dim + x + 2, (y + 1) * dim + x + 1 });
                    fuvs.Add(new int[] { (y) * dim + x + 1, (y) * dim + x + 2, (y + 1) * dim + x + 2, (y + 1) * dim + x + 1 });
                }
            }
            CalculateBounds();
            CalculateNormals();
            CalculateFaceBounds();
        }
        public new void DrawMesh(int program, bool wireframe, bool selected, Vector3 Color)
        {
            int sclloc = GL.GetUniformLocation(program, "scale");
            Vector3 scl = Vector3.One;
            GL.ProgramUniform3(program, sclloc, ref scl);
            base.DrawMesh(program, wireframe, selected, Color);
        }
    }
}