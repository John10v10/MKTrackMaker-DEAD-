using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MarioKartTrackMaker.ViewerResources
{
    public class Camera
    {
        public ViewPortPanel vpc;
        public Vector3 pivot;
        private Vector3 _p;
        public float zoom = 100F;
        public Vector3 position {
            get { return _p; }
            set { _p = (value - pivot).Normalized()*zoom + pivot; }
        }
        public Camera(Vector3 p, Vector3 pv, ViewPortPanel v)
        {
            vpc = v;
            position = p;
            pivot = pv;
        }
        public Matrix4 look_at_mtx { get { return Matrix4.LookAt(position, pivot, Vector3.UnitZ); } }
        public Matrix4 matrix
        {
            get { return look_at_mtx * Matrix4.CreatePerspectiveFieldOfView(1F, vpc.Width * 1F / vpc.Height, 0.1F, 10000F); }
        }
    }
}
