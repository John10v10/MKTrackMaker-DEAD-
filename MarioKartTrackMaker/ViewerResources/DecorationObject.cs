using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace MarioKartTrackMaker.ViewerResources
{
    /// <summary>
    /// This class defines a decoration object which contains a decoration mesh and a baked transform.
    /// </summary>
    public class DecorationObject : IDisposable
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
        /// The 3D object that uses this decoration object.
        /// </summary>
        public Object3D Obj;
        /// <summary>
        /// Let's construct a decoration object!
        /// </summary>
        /// <param name="filepath">The path to import the model file.</param>
        public DecorationObject(string filepath, Object3D obj)
        {
            Obj = obj;
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (Obj.Decorations.Contains(this)) Obj.Decorations.Remove(this);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DecorationObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
