using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using ObjParser;
using MarioKartTrackMaker.IO;
using OpenTK.Graphics.OpenGL;

namespace MarioKartTrackMaker.ViewerResources
{
    /// <summary>
    /// This class defines a decoration, which is to add pretty details to track elements. It is recommended to place them on background 3D objects.
    /// </summary>
    public class DecorationMesh
    {
        /// <summary>
        /// This is the database that holds all the decoration meshes in the program's memory.
        /// </summary>
        public static List<DecorationMesh> database = new List<DecorationMesh>();

        /// <summary>
        /// Checks if the model is already loaded. If so, it'll return the database's index of the model, otherwise -1;
        /// </summary>
        /// <param name="path">The path of the model file to check.</param>
        /// <returns></returns>
        public static int IsLoaded(string path)
        {
            for (int i = 0; i < database.Count; i++)
                if (database[i].path == path)
                    return i;
            return -1;
        }
        /// <summary>
        /// Loads and adds a new decoration mesh from a file into the database.
        /// </summary>
        /// <param name="path">The path of the model file to load.</param>
        /// <returns></returns>
        public static int AddMesh(string path)
        {
            int id = IsLoaded(path);
            if (id == -1)
            {
                DecorationMesh m = new DecorationMesh(path);
                database.Add(m);
                id = database.IndexOf(m);
            }
            return id;
        }
        /// <summary>
        /// The path that this decoration mesh was loaded from.
        /// </summary>
        public string path;
        /// <summary>
        /// The name of this decoration mesh.
        /// </summary>
        public string name;
        /// <summary>
        /// The defined amount of data elements.
        /// </summary>
        public int Vertex_Count, Normal_Count, UV_Count, Face_Count, Face_Normal_Count, Face_UV_Count;
        /// <summary>
        /// The limited array of vertices.
        /// </summary>
        public Vector3[] Vertices = new Vector3[128];
        /// <summary>
        /// The limited array of normals.
        /// </summary>
        public Vector3[] Normals = new Vector3[128];
        /// <summary>
        /// The limited array of UVs.
        /// </summary>
        public Vector2[] UVs = new Vector2[128];
        /// <summary>
        /// The limited array of vertex indices.
        /// </summary>
        public byte[] faces = new byte[256];
        /// <summary>
        /// The limited array of normal indices.
        /// </summary>
        public byte[] fnmls = new byte[256];
        /// <summary>
        /// The limited array of UV indices.
        /// </summary>
        public byte[] fuvs = new byte[256];
        /// <summary>
        /// All the collision meshes contained inside this decoration mesh.
        /// </summary>
        public List<Collision_Mesh> KCLs = new List<Collision_Mesh>();
        /// <summary>
        /// Currently useless, but in the future, I definately want to include the feature to use simple 2D figures for decoration meshes. 
        /// </summary>
        public bool is2D = false;
        /// <summary>
        /// Currently useless, but hope to make decoration meshes shadeless if they hold a shadeless flag in their setting. 
        /// </summary>
        public bool isShadeless = false;
        /// <summary>
        /// This indicates whether this program is running on a Macintosh or not.
        /// </summary>
        private static bool runningOnMac
        {
            get
            {
                int plat = (int)Environment.OSVersion.Platform;
                return ((plat == 4) || (plat == 128));
            }
        }
        /// <summary>
        /// This is dynamic due to the fact that a Windows computer uses \\ when a Macintosh uses //.
        /// </summary>
        private static string filepathSlash
        {
            get
            {
                if (runningOnMac) return "//";
                return "\\";
            }
        }



        /// <summary>
        /// The texture id of the decoration mesh.
        /// </summary>
        public int texture;
        /// <summary>
        /// The bounding box size of this decoration mesh.
        /// </summary>
        public Bounds size = new Bounds();
        /// <summary>
        /// Constructor. Loads the decoration mesh from file.
        /// </summary>
        /// <param name="filepath">The path of the file.</param>
        public DecorationMesh(string filepath)
        {
            path = filepath;
            name = Path.GetFileNameWithoutExtension(filepath).Replace('_', ' ');
            Obj tobj = new Obj();
            tobj.LoadObj(filepath);
            Mtl tmtl = new Mtl();
            tmtl.LoadMtl(Path.GetDirectoryName(filepath) + filepathSlash + tobj.Mtl);
            if (tobj.VertexList.Count > 128)
            {
                throw new OverflowException("There are too many vertices in this mesh. Maximum count is 128.");
            }
            Vertex_Count = 0;
            foreach (ObjParser.Types.Vertex v in tobj.VertexList)
            {
                Vertices[Vertex_Count] = new Vector3((float)v.X, (float)v.Y, (float)v.Z);
                Vertex_Count++;
            }
            if (tobj.NormalList.Count > 128)
            {
                throw new OverflowException("There are too many normals in this mesh. Maximum count is 128.");
            }
            Normal_Count = 0;
            foreach (ObjParser.Types.Normal vn in tobj.NormalList)
            {
                Normals[Normal_Count] = new Vector3((float)vn.X, (float)vn.Y, (float)vn.Z);
                Normal_Count++;
            }
            if (Normal_Count == 0) isShadeless = true;
            if (tobj.TextureList.Count > 128)
            {
                throw new OverflowException("There are too many UV coordinates in this mesh. Maximum count is 128.");
            }
            UV_Count = 0;
            foreach (ObjParser.Types.TextureVertex vt in tobj.TextureList)
            {
                UVs[UV_Count] = new Vector2((float)vt.X, (float)vt.Y);
                UV_Count++;

            }
            if (tmtl.MaterialList.Count == 0)
            {
                throw new Exception("This decoration model lacks a material(.mtl).");
            }
            ObjParser.Types.Material mat = tmtl.MaterialList[0];
            if (Path.IsPathRooted(mat.DiffuseTexture))
            {
                texture = ContentPipe.Load_and_AddTexture(mat.DiffuseTexture);
            }
            else
            {
                texture = ContentPipe.Load_and_AddTexture(Path.GetDirectoryName(filepath) + filepathSlash + mat.DiffuseTexture);
            }
            Face_Count = Face_Normal_Count = Face_UV_Count = 0;
            foreach (ObjParser.Types.Face f in tobj.FaceList)
            {
                if (f.UseMtl == mat.Name)
                {
                    string error_message = "There are too many vertex indexes in this mesh. Maximum count is {0}.";
                    for (byte ii = 0; ii < f.VertexIndexList.Length - 2; ii++)
                    {
                        faces[Face_Count] = (byte)f.VertexIndexList[0];
                        IncrementThrow(ref Face_Count, error_message, 256);
                        faces[Face_Count] = (byte)f.VertexIndexList[ii + 1];
                        IncrementThrow(ref Face_Count, error_message, 256);
                        faces[Face_Count] = (byte)f.VertexIndexList[ii + 2];
                        IncrementThrow(ref Face_Count, error_message, 256);
                    }
                    error_message = "There are too many normal indexes in this mesh. Maximum count is {0}.";
                    for (byte ii = 0; ii < f.NormalIndexList.Length - 2; ii++)
                    {
                        fnmls[Face_Normal_Count] = (byte)f.NormalIndexList[0];
                        IncrementThrow(ref Face_Normal_Count, error_message, 256);
                        fnmls[Face_Normal_Count] = (byte)f.NormalIndexList[ii + 1];
                        IncrementThrow(ref Face_Normal_Count, error_message, 256);
                        fnmls[Face_Normal_Count] = (byte)f.NormalIndexList[ii + 2];
                        IncrementThrow(ref Face_Normal_Count, error_message, 256);
                    }
                    error_message = "There are too many UV indexes in this mesh. Maximum count is {0}.";
                    for (byte ii = 0; ii < f.TextureVertexIndexList.Length - 2; ii++)
                    {
                        fuvs[Face_UV_Count] = (byte)f.TextureVertexIndexList[0];
                        IncrementThrow(ref Face_UV_Count, error_message, 256);
                        fuvs[Face_UV_Count] = (byte)f.TextureVertexIndexList[ii + 1];
                        IncrementThrow(ref Face_UV_Count, error_message, 256);
                        fuvs[Face_UV_Count] = (byte)f.TextureVertexIndexList[ii + 2];
                        IncrementThrow(ref Face_UV_Count, error_message, 256);
                    }
                }
            }
            if (Face_Normal_Count == 0) isShadeless = true;
            string KCL_filepath = Path.GetDirectoryName(filepath) + filepathSlash + Path.GetFileNameWithoutExtension(filepath) + "_KCL.obj";
            if (File.Exists(KCL_filepath))
            {
                Obj tobjkcl = new Obj();
                tobjkcl.LoadObj(KCL_filepath);

                List<Vector3> CVerts = new List<Vector3>();
                foreach (ObjParser.Types.Vertex v in tobjkcl.VertexList)
                {
                    CVerts.Add(new Vector3((float)v.X, (float)v.Y, (float)v.Z));
                    if (CVerts.Count >= 96)
                    {
                        throw new OverflowException("There are too many collision vertices in this mesh. Maximum count is 96.");
                    }
                }
                int KCL_Face_Count = 0;
                foreach (string name in tobjkcl.objects)
                {
                    string error_message = "There are too many collision vertex indexes in this mesh. Maximum count is {0}.";
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
                            IncrementThrow(ref KCL_Face_Count, error_message, 192);
                        }
                    }
                    List<Vector3> cverts = new List<Vector3>();
                    foreach (Vector3 v in CVerts)
                        cverts.Add(v);
                    KCLs.Add(new Collision_Mesh(cverts, faces, coll));
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
        /// <summary>
        /// Calculates the boundaries of this decoration mesh.
        /// </summary>
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

        /// <summary>
        /// Renders the decoration mesh.
        /// </summary>
        /// <param name="program">The id of the shader program.</param>
        /// <param name="mode">The collision mode. 1 displays only the model, 2 displays only the model's collisions, and 3 displays both.</param>
        /// <param name="wireframe">Render this as wireframe?</param>
        public void DrawMesh(int program, int mode, bool wireframe)
        {
            if ((mode & 1) == 1)
            {
                GL.UseProgram(program);
                int sclloc = GL.GetUniformLocation(program, "scale");
                Vector3 One = Vector3.One;
                GL.Uniform3(sclloc, ref One);
                int texloc = GL.GetUniformLocation(program, "texture");
                int usetexLoc = GL.GetUniformLocation(program, "useTexture");
                int selloc = GL.GetUniformLocation(program, "selected");
                GL.Uniform1(selloc, 0);
                int shlloc = GL.GetUniformLocation(program, "shadeless");
                GL.Uniform1(shlloc, (isShadeless) ? 1 : 0);
                if (texture != -1)
                {
                    GL.Uniform1(texloc, texture - 1);
                    GL.Uniform1(usetexLoc, 1);
                }
                else
                {
                    GL.Uniform1(usetexLoc, 0);
                }

                GL.Color3(Vector3.One);
                if (wireframe)
                {
                    GL.Begin(PrimitiveType.Lines);
                    for (int f = 0; f < Math.Min(Face_Count, Math.Min(Face_Normal_Count, Face_UV_Count)); f += 3)
                    {
                        if (fnmls[f] - 1 >= 0) GL.Normal3(Normals[fnmls[f] - 1]);
                        if (fuvs[f] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f] - 1]);
                        GL.Vertex3(Vertices[faces[f] - 1]);
                        if (fnmls[f + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f + 1] - 1]);
                        if (fuvs[f + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f + 1] - 1]);
                        if (fnmls[f + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f + 1] - 1]);
                        if (fuvs[f + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f + 1] - 1]);
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
                    for (int f = 0; f < Math.Min(Face_Count, Math.Min(Face_Normal_Count, Face_UV_Count)); f += 3)
                    {
                        if (fnmls[f] - 1 >= 0) GL.Normal3(Normals[fnmls[f] - 1]);
                        if (fuvs[f] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f] - 1]);
                        GL.Vertex3(Vertices[faces[f] - 1]);
                        if (fnmls[f + 1] - 1 >= 0) GL.Normal3(Normals[fnmls[f + 1] - 1]);
                        if (fuvs[f + 1] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f + 1] - 1]);
                        GL.Vertex3(Vertices[faces[f + 1] - 1]);
                        if (fnmls[f + 2] - 1 >= 0) GL.Normal3(Normals[fnmls[f + 2] - 1]);
                        if (fuvs[f + 2] - 1 >= 0) GL.TexCoord2(UVs[fuvs[f + 2] - 1]);
                        GL.Vertex3(Vertices[faces[f + 2] - 1]);
                    }
                    GL.End();
                }
                GL.UseProgram(0);
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
