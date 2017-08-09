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
    public class Model
    {
        public static List<Model> database = new List<Model>();

        public static bool runningOnMac;
        public static string filepathSlash;

        public static void DoMacStuff() {
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

        public static int IsLoaded(string path)
        {
            DoMacStuff();
            for (int i = 0; i < database.Count; i++)
                if (database[i].path == path)
                    return i;
            return -1;
        }
        public static int AddModel(string path)
        {
            int id = IsLoaded(path);
            if(id == -1)
            {
                Model m = new Model(path);
                database.Add(m);
                id = database.IndexOf(m);
            }
            return id;
        }
        public string path;
        public string name;
        public Bounds size = new Bounds();
        public List<Mesh> meshes = new List<Mesh>();
        public List<Collision_Mesh> KCLs = new List<Collision_Mesh>();
        public List<Attachment> attachments = new List<Attachment>();
        public Model(string filepath)
        {
            path = filepath;
            name = Path.GetFileNameWithoutExtension(filepath).Replace('_', ' ');
            Obj tobj = new Obj();
            tobj.LoadObj(filepath);
            Mtl tmtl = new Mtl();
            tmtl.LoadMtl(Path.GetDirectoryName(filepath) + filepathSlash + tobj.Mtl);

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
                int texture = ContentPipe.Load_and_AddTexture(Path.GetDirectoryName(filepath) + filepathSlash + mat.DiffuseTexture);

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
            Obj tobjkcl = new Obj();
            tobjkcl.LoadObj(Path.GetDirectoryName(filepath) + filepathSlash + Path.GetFileNameWithoutExtension(filepath) + "_KCL.obj");

            List<Vector3> CVerts = new List<Vector3>();
            foreach (ObjParser.Types.Vertex v in tobjkcl.VertexList)
            {
                CVerts.Add(new Vector3((float)v.X, (float)v.Y, (float)v.Z));
            }
            foreach (string name in tobjkcl.objects)
            {
                Collision_Mesh.CollisionType coll;
                if (name == "ROAD")
                    coll = Collision_Mesh.CollisionType.road;
                else if (name == "WALL")
                    coll = Collision_Mesh.CollisionType.wall;
                else if (name == "OFFROAD")
                    coll = Collision_Mesh.CollisionType.off_road;
                else if (name == "WAYOFFROAD")
                    coll = Collision_Mesh.CollisionType.way_off_road;
                else if (name == "OUTOFBOUNDS")
                    coll = Collision_Mesh.CollisionType.out_of_bounds;
                else if (name == "BOOST")
                    coll = Collision_Mesh.CollisionType.boost;
                else if (name == "RAMP")
                    coll = Collision_Mesh.CollisionType.ramp;
                else if (name == "ENGAGEGLIDER")
                    coll = Collision_Mesh.CollisionType.engage_glider;
                else if (name == "SIDERAMP")
                    coll = Collision_Mesh.CollisionType.side_ramp;
                else if (name == "CANNON")
                    coll = Collision_Mesh.CollisionType.cannon;
                else if (name == "WATER")
                    coll = Collision_Mesh.CollisionType.water;
                else if (name == "LAVA")
                    coll = Collision_Mesh.CollisionType.lava;
                else if (name == "SPINOUT")
                    coll = Collision_Mesh.CollisionType.spin_out;
                else if (name == "KNOCKOUT")
                    coll = Collision_Mesh.CollisionType.knock_out;
                else continue;

                List<int[]> faces = new List<int[]>();
                foreach (ObjParser.Types.Face f in tobjkcl.FaceList)
                {
                    if (f.objectName == name)
                    {
                        faces.Add(f.VertexIndexList);
                    }
                }
                List<Vector3> cverts = new List<Vector3>();
                foreach (Vector3 v in CVerts)
                    cverts.Add(v);
                KCLs.Add(new Collision_Mesh(cverts, faces, coll));
            }
            CalculateBounds();
            ImportAttachments(Path.GetDirectoryName(filepath) + filepathSlash + Path.GetFileNameWithoutExtension(filepath) + "_Atch.txt");
        }

        private void ImportAttachments(string path)
        {
            if (File.Exists(path))
            {
                Attachment atch = new Attachment();
                foreach (string line in File.ReadAllLines(path))
                {
                    string[] parts = line.Split(new string[] { ": ", ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts[0].ToUpper() == "NAME")
                    {
                        atch.name = parts[1];
                    }
                    if (parts[0].ToUpper() == "ISFIRST")
                    {
                        atch.isFirst = parts[1] == "1";
                    }
                    if (parts[0].ToUpper() == "ISFEMALE")
                    {
                        atch.isFemale = parts[1] == "1";
                    }
                    if (parts[0].ToUpper() == "MAT00")
                    {
                        atch.transform[0, 0] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT10")
                    {
                        atch.transform[0, 1] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT20")
                    {
                        atch.transform[0, 2] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT30")
                    {
                        atch.transform[0, 3] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT01")
                    {
                        atch.transform[1, 0] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT11")
                    {
                        atch.transform[1, 1] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT21")
                    {
                        atch.transform[1, 2] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT31")
                    {
                        atch.transform[1, 3] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT02")
                    {
                        atch.transform[2, 0] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT12")
                    {
                        atch.transform[2, 1] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT22")
                    {
                        atch.transform[2, 2] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT32")
                    {
                        atch.transform[2, 3] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT03")
                    {
                        atch.transform[3, 0] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT13")
                    {
                        atch.transform[3, 1] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT23")
                    {
                        atch.transform[3, 2] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "MAT33")
                    {
                        atch.transform[3, 3] = float.Parse(parts[1]);
                    }
                    if (parts[0].ToUpper() == "END")
                    {
                        //atch.transform *= Matrix4.CreateTranslation(100,100,100);
                        attachments.Add(atch);
                        atch = new Attachment();
                    }
                }
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

            foreach (Mesh m in meshes)
            {
                size.minX = Math.Min(size.minX, m.size.minX);
                size.maxX = Math.Max(size.maxX, m.size.maxX);
                size.minY = Math.Min(size.minY, m.size.minY);
                size.maxY = Math.Max(size.maxY, m.size.maxY);
                size.minZ = Math.Min(size.minZ, m.size.minZ);
                size.maxZ = Math.Max(size.maxZ, m.size.maxZ);
            }
        }

        public void DrawModel(int program, int mode, bool wireframe)
        {
            if ((mode & 1) == 1)
            {
                foreach (Mesh mesh in meshes)
                {
                    mesh.DrawMesh(program, wireframe);
                }
            }
            if ((mode & 2) == 2)
            {
                foreach (Collision_Mesh mesh in KCLs)
                {
                    mesh.DrawMesh(wireframe);
                }
            }
        }
    }
}
