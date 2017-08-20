using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MarioKartTrackMaker.ViewerResources
{
    /// <summary>
    /// This class defines a decoration object which contains a decoration mesh and a baked transform.
    /// </summary>
    public class DecorationObject
    {
        /// <summary>
        /// The baked transform.
        /// </summary>
        public Matrix4 transform = Matrix4.Identity;
        /// <summary>
        /// The decoration object's decoration mesh defined by the database index.
        /// </summary>
        private int _mesh;
        /// <summary>
        /// The decoration object's decoration mesh.
        /// </summary>
        public DecorationMesh mesh
        {
            get
            {
                return DecorationMesh.database[_mesh];
            }
        }
        /// <summary>
        /// Let's construct a decoration object!
        /// </summary>
        /// <param name="filepath">The path to import the model file.</param>
        public DecorationObject(string filepath)
        {
            _mesh = DecorationMesh.AddMesh(filepath);
        }
        /// <summary>
        /// Renders the decoration object.
        /// </summary>
        /// <param name="program">The id of the shader program.</param>
        /// <param name="mode">The collision mode. 1 displays only the model, 2 displays only the model's collisions, and 3 displays both.</param>
        /// <param name="wireframe">Render this as wireframe?</param>
        public void DrawObject(int program, int mode, bool wireframe)
        {
            GL.PushMatrix();
            GL.MultMatrix(ref transform);
            mesh.DrawMesh(program, mode, wireframe);
            GL.PopMatrix();
        }
    }
}
