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
    }
}
