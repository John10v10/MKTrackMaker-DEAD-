using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MarioKartTrackMaker.ViewerResources
{
    /// <summary>
    /// This class defines the connector of a track piece. It is used to attach a 3D object with another.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// The name of this attachment.
        /// </summary>
        public string name = "<unknown>";
        /// <summary>
        /// If this is set to 1, it will be the one to attach to a male attachment.
        /// It it's set to 2, it will be the one to attach to a female attachment.
        /// </summary>
        public int isFirst;
        /// <summary>
        /// Is this a female attachment? Those usually go behind the track piece. If true, the display color will be pink instead of blue.
        /// </summary>
        public bool isFemale;
        /// <summary>
        /// The baked transform of the attachment.
        /// </summary>
        public Matrix4 transform;
        /// <summary>
        /// Gets the transformation matrix of this attachment in world space.
        /// </summary>
        /// <param name="parentMtx">The transformation matrix of this attachment's 3D object.</param>
        /// <returns></returns>
        public Matrix4 get_world_transform(Matrix4 parentMtx)
        {
            return transform * parentMtx;
        }

        /// <summary>
        /// Draw the attachment.
        /// </summary>
        /// <param name="size">Length of the attachment.</param>
        /// <param name="selectState">Depending on how high this value is, the color of the attachment can get lighter or darker.</param>
        public void draw(float size, int selectState)
        {
            GL.PushMatrix();
            GL.MultMatrix(ref transform);
            GL.LineWidth(4F);
            if (isFemale)
                GL.Color3(Vector3.Lerp(new Vector3(0.875F, 0, 1F), Vector3.One, 1 - (float)Math.Pow(1 - (selectState /2F), 2)));
            else
                GL.Color3(Vector3.Lerp(new Vector3(0, 0.875F, 1F), Vector3.One, 1 - (float)Math.Pow(1 - (selectState /2F), 2)));
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            if (isFemale)
                GL.Vertex3(0, -size, 0);
            else
                GL.Vertex3(0, size, 0);
            GL.End();
            GL.LineWidth(1F);
            GL.PopMatrix();
        }
        public override string ToString()
        {
            return name;
        }
    }
}
