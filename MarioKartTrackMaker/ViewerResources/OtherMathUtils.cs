using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioKartTrackMaker.ViewerResources
{
    public static class OtherMathUtils
    {
        //Special thanks to DLBmaths's Youtube Video Here: https://www.youtube.com/watch?v=HC5YikQxwZA
        public static Vector3[] ClosestPointsBetweenRays(ViewPortPanel.Ray L1, ViewPortPanel.Ray L2, float Min_1 = float.NegativeInfinity, float Min_2 = float.NegativeInfinity, float Max_1 = float.PositiveInfinity, float Max_2 = float.PositiveInfinity)
        {
            Vector3 U1 = L1.dir;
            Vector3 U2 = L2.dir;
            //Step 1: Bringing in the values
            float Mx1_1 = L1.pos.X;
            float My1_1 = L1.pos.Y;
            float Mz1_1 = L1.pos.Z;
            float tMx = U1.X;
            float tMy = U1.Y;
            float tMz = U1.Z;
            float Mx1_2 = L2.pos.X;
            float My1_2 = L2.pos.Y;
            float Mz1_2 = L2.pos.Z;
            float sMx = U2.X;
            float sMy = U2.Y;
            float sMz = U2.Z;

            //Step 2: Subtraction

            float sMx2 = sMx;
            float sMy2 = sMy;
            float sMz2 = sMz;
            float tMx2 = -tMx;
            float tMy2 = -tMy;
            float tMz2 = -tMz;
            float Mx2 = Mx1_2 - Mx1_1;
            float My2 = My1_2 - My1_1;
            float Mz2 = Mz1_2 - Mz1_1;

            //Step 3: Dot - Multiplying

            float sMx3_1 = sMx2 * U1.X;
            float sMy3_1 = sMy2 * U1.Y;
            float sMz3_1 = sMz2 * U1.Z;
            float tMx3_1 = tMx2 * U1.X;
            float tMy3_1 = tMy2 * U1.Y;
            float tMz3_1 = tMz2 * U1.Z;
            float Mx3_1 = Mx2 * U1.X;
            float My3_1 = My2 * U1.Y;
            float Mz3_1 = Mz2 * U1.Z;

            float sM4_1 = sMx3_1 + sMy3_1 + sMz3_1;
            float tM4_1 = tMx3_1 + tMy3_1 + tMz3_1;
            float M4_1 = Mx3_1 + My3_1 + Mz3_1;

            //sM4_1*s + tM4_1*t = -M4_1 we need to find both s and t.

            float sMx3_2 = sMx2 * U2.X;
            float sMy3_2 = sMy2 * U2.Y;
            float sMz3_2 = sMz2 * U2.Z;
            float tMx3_2 = tMx2 * U2.X;
            float tMy3_2 = tMy2 * U2.Y;
            float tMz3_2 = tMz2 * U2.Z;
            float Mx3_2 = Mx2 * U2.X;
            float My3_2 = My2 * U2.Y;
            float Mz3_2 = Mz2 * U2.Z;

            float sM4_2 = sMx3_2 + sMy3_2 + sMz3_2;
            float tM4_2 = tMx3_2 + tMy3_2 + tMz3_2;
            float M4_2 = Mx3_2 + My3_2 + Mz3_2;

            //sM4_2*s + tM4_2*t = -M4_2 we need to find both s and t.

            //Step 4: Now time to solve for both equations and find s!
            //Just like in the video, let's solve for s first.
            float ratio = tM4_2 / tM4_1;

            float sM5_1 = sM4_1 * ratio;
            float sM5_2 = sM4_2;
            float M5_1 = M4_1 * ratio;
            float M5_2 = M4_2;
            //f in Mf stands for final.
            float sMf = sM5_2 - sM5_1;
            float Mf = M5_2 - M5_1;

            //WE FOUND s!!!
            float s = -Mf/sMf;

            //Step 5: Now time to find t!
            float t = (-M4_1-sM4_1*s)/tM4_1;
            // ... That was easy.

            //FINAL STEP: Finding the distance between points
            //first, let's clamp s and t.
            t = Math.Max(t, Min_1);
            s = Math.Max(s, Min_2);
            t = Math.Min(t, Max_1);
            s = Math.Min(s, Max_2);
            Vector3[] results = new Vector3[2];
            results[0] = L1.pos + L1.dir * t;
            results[1] = L2.pos + L2.dir * s;
            return results;
        }
        public static float edgeFunction(Vector3 a, Vector3 b, Vector3 c, int mode)
        {
            if (mode == 0)
                return (c[0] - a[0]) * (b[1] - a[1]) - (c[1] - a[1]) * (b[0] - a[0]);
            else if (mode == 1)
                return (c[0] - a[0]) * (b[2] - a[2]) - (c[2] - a[2]) * (b[0] - a[0]);
            else if (mode == 2)
                return (c[1] - a[1]) * (b[2] - a[2]) - (c[2] - a[2]) * (b[1] - a[1]);
            return 0;
        }
        public struct TriangleIntersection
        {
            public bool intersects;
            public float depth;
            public Vector3 normal;

            public TriangleIntersection(bool v) : this()
            {
                intersects = v;
            }

            public TriangleIntersection(bool v, float t, Vector3 n) : this()
            {
                intersects = v;
                depth = t;
                normal = n;
            }
        }
        public static TriangleIntersection RayIntersectsTriangle(ViewPortPanel.Ray r, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 n0, Vector3 n1, Vector3 n2)
        {
            Vector3 e1, e2, h, s, q;
            float a, f, u, v;
            e1 = v1 - v0;
            e2 = v2 - v0;

            h = Vector3.Cross(r.dir, e2);
            a = Vector3.Dot(e1, h);


            if (a > -0.00001 && a < 0.00001)
                return new TriangleIntersection(false);

            f = 1 / a;
            s = r.pos - v0;
            u = f * (Vector3.Dot(s, h));

            if (u < 0.0 || u > 1.0)
                return new TriangleIntersection(false);

            q = Vector3.Cross(s, e1);
            v = f * Vector3.Dot(r.dir, q);

            if (v < 0.0 || u + v > 1.0)
                return new TriangleIntersection(false);

            // at this stage we can compute t to find out where
            // the intersection point is on the line
            float t = f * Vector3.Dot(e2, q);

            Vector3 normal = new Vector3();
            if (t <= 0.00001) // ray intersection
            {
                return new TriangleIntersection(false, t, normal);
            }
            Vector3 p = r.pos + r.dir * t;

            float area;
            float w0, w1, w2;
            area = edgeFunction(v0, v1, v2, 0);
            w0 = edgeFunction(v1, v2, p, 0);
            w1 = edgeFunction(v2, v0, p, 0);
            w2 = edgeFunction(v0, v1, p, 0);
            w0 /= area;
            w1 /= area;
            w2 /= area;
            if (float.IsNaN(w0) || float.IsNaN(w1) || float.IsNaN(w2) || float.IsInfinity(w0) || float.IsInfinity(w1) || float.IsInfinity(w2))
            {
                area = edgeFunction(v0, v1, v2, 1);
                w0 = edgeFunction(v1, v2, p, 1);
                w1 = edgeFunction(v2, v0, p, 1);
                w2 = edgeFunction(v0, v1, p, 1);
                w0 /= area;
                w1 /= area;
                w2 /= area;
                if (float.IsNaN(w0) || float.IsNaN(w1) || float.IsNaN(w2) || float.IsInfinity(w0) || float.IsInfinity(w1) || float.IsInfinity(w2))
                {
                    area = edgeFunction(v0, v1, v2, 2);
                    w0 = edgeFunction(v1, v2, p, 2);
                    w1 = edgeFunction(v2, v0, p, 2);
                    w2 = edgeFunction(v0, v1, p, 2);
                    w0 /= area;
                    w1 /= area;
                    w2 /= area;
                }
            }
            normal[0] = w0 * n0[0] + w1 * n1[0] + w2 * n2[0];
            normal[1] = w0 * n0[1] + w1 * n1[1] + w2 * n2[1];
            normal[2] = w0 * n0[2] + w1 * n1[2] + w2 * n2[2];
            return new TriangleIntersection(true, t, normal.Normalized());


        }
        public static float intersectPlane(ViewPortPanel.Ray r, Vector3 n, Vector3 p0)
        {
            // assuming vectors are all normalized
            float denom = Vector3.Dot(-n, r.dir);
            Vector3 p0l0 = p0 - r.pos;
            float t = Vector3.Dot(p0l0, -n) / denom;
            return t;
        }
    }
}
