using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace MarioKartTrackMaker.ViewerResources
{
    public class Collision_Mesh
    {
        public enum CollisionType : int
        {
            road = 0,
            wall = 1,
            off_road = 2,
            way_off_road = 3,
            out_of_bounds = 4,
            boost = 5,
            ramp = 6,
            engage_glider = 7, //MK7 and MK8
            side_ramp = 8, //MKW
            cannon = 9,
            water = 10,
            lava = 11,
            spin_out = 12,
            knock_out = 13
        }
        public List<Vector3> Vertices;
        public List<int[]> faces;
        public CollisionType type;
        public Collision_Mesh(List<Vector3> Vertices, List<int[]> faces, CollisionType coll)
        {
            this.Vertices = Vertices;
            this.faces = faces;
            type = coll;
            //Optimize();
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
            }
        }
        private bool InFaces(int index)
        {
            foreach (int[] f in faces)
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
        }

        public void DrawMesh(bool wireframe)
        {
            GL.UseProgram(0);
            switch (type)
            {
                case CollisionType.road:
                    GL.Color3(0.75, 0.75, 0.75);
                    break;
                case CollisionType.wall:
                    GL.Color3(0.125, 0.125, 0.125);
                    break;
                case CollisionType.off_road:
                    GL.Color3(0.5, 0.5, 0.5);
                    break;
                case CollisionType.way_off_road:
                    GL.Color3(0.25, 0.25, 0.25);
                    break;
                case CollisionType.out_of_bounds:
                    GL.Color3(0.0625, 0.0625, 0.0625);
                    break;
                case CollisionType.boost:
                    GL.Color3(1, 1, 0);
                    break;
                case CollisionType.ramp:
                    GL.Color3(0.75, 0.5, 0);
                    break;
                case CollisionType.engage_glider:
                    GL.Color3(0, 0.75, 1);
                    break;
                case CollisionType.side_ramp:
                    GL.Color3(0, 0.125, 1);
                    break;
                case CollisionType.cannon:
                    GL.Color3(1, 0.5, 1);
                    break;
                case CollisionType.water:
                    GL.Color3(0.25, 1, 1);
                    break;
                case CollisionType.lava:
                    GL.Color3(1, 0, 0);
                    break;
                case CollisionType.spin_out:
                    GL.Color3(0.625, 0.5, 0);
                    break;
                case CollisionType.knock_out:
                    GL.Color3(0.75, 0.25, 0);
                    break;
            }
            
            if (wireframe)
            {
                GL.Begin(PrimitiveType.Lines);
                for (int f = 0; f < faces.Count; f++)
                {
                    for (int i = 0; i < faces[f].Length - 2; i++)
                    {
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                    }
                }
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                for(int f = 0; f < faces.Count; f++)
                {
                    
                    for (int i = 0; i < faces[f].Length - 2; i++)
                    {
                        GL.Vertex3(Vertices[faces[f][0] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f][i + 2] - 1]);
                    }
                }
                GL.End();
            }
            GL.Color3(Vector3.One);
        }
    }
}
