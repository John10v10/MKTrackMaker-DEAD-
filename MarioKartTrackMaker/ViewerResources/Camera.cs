using OpenTK;

namespace MarioKartTrackMaker.ViewerResources
{
    /// <summary>
    /// This class defines a camera to render a good prespective in the view port.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The view port panel that uses this camera.
        /// </summary>
        public ViewPortPanel vpc;
        /// <summary>
        /// This is the position where the camera focuses on, orbits around, and always looks at.
        /// </summary>
        public Vector3 pivot;
        private Vector3 _p;
        /// <summary>
        /// This value defines how the camera should tilt from the look-at view.
        /// </summary>
        public Vector3 UpDirection = Vector3.UnitZ;
        /// <summary>
        /// The closest depth renderable.
        /// </summary>
        public float clip_near = 50F;
        /// <summary>
        /// The farthest depth renderable.
        /// </summary>
        public float clip_far = 100000F;
        private float _z = 100F;
        /// <summary>
        /// This value defines how close or far the eye should be from the pivot.
        /// </summary>
        public float zoom { get { return _z; }set { _z = value; position = position; } }
        /// <summary>
        /// The eye position of this camera.
        /// </summary>
        public Vector3 position {
            get { return _p; }
            set { _p = (value - pivot).Normalized()*zoom + pivot; }
        }

        /// <summary>
        /// Constructing the camera.
        /// </summary>
        /// <param name="p">Specify the position of the camera.</param>
        /// <param name="pv">Specify the pivot position of the camera.</param>
        /// <param name="v">Specify the view port panel that will use this camera.</param>
        public Camera(Vector3 p, Vector3 pv, ViewPortPanel v)
        {
            vpc = v;
            position = p;
            pivot = pv;
        }
        /// <summary>
        /// Returns the transformation matrix that looks at the pivot position from the eye position.
        /// </summary>
        public Matrix4 look_at_mtx { get { return Matrix4.LookAt(position, pivot, UpDirection); } }
        /// <summary>
        /// Returns the overall prespective matrix.
        /// </summary>
        public Matrix4 matrix
        {
            get { return look_at_mtx * Matrix4.CreatePerspectiveFieldOfView(1F, vpc.Width * 1F / vpc.Height, clip_near, clip_far); }
        }

        /// <summary>
        /// Returns the overall prespective matrix, but with a tiny near clipping. I would only recommended using this for ray-tracing.
        /// </summary>
        public Matrix4 matrix_toScreen
        {
            get { return look_at_mtx * Matrix4.CreatePerspectiveFieldOfView(1F, vpc.Width * 1F / vpc.Height, 0.01F, clip_far); }
        }
    }
}
