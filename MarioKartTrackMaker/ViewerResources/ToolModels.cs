using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using MarioKartTrackMaker;
namespace MarioKartTrackMaker.ViewerResources
{
    public static class ToolModels
    {
        //DrawBall Function copied from BrawlLib, originally known as DrawSphere
        public static void DrawBall(Vector3 center, float radius, uint precision)
        {
            if (radius < 0.0f)
                radius = -radius;

            float halfPI = (float)(Math.PI * 0.5);
            float oneThroughPrecision = 1.0f / precision;
            float twoPIThroughPrecision = (float)(Math.PI * 2.0 * oneThroughPrecision);

            float theta1, theta2, theta3;
            Vector3 norm, pos;

            for (uint j = 0; j < precision / 2; j++)
            {
                theta1 = (j * twoPIThroughPrecision) - halfPI;
                theta2 = ((j + 1) * twoPIThroughPrecision) - halfPI;

                GL.Begin(PrimitiveType.TriangleStrip);
                for (uint i = 0; i <= precision; i++)
                {
                    theta3 = i * twoPIThroughPrecision;


                    norm.X = (float)(Math.Cos(theta1) * Math.Cos(theta3));
                    norm.Y = (float)Math.Sin(theta1);
                    norm.Z = (float)(Math.Cos(theta1) * Math.Sin(theta3));
                    pos.X = center.X + radius * norm.X;
                    pos.Y = center.Y + radius * norm.Y;
                    pos.Z = center.Z + radius * norm.Z;

                    GL.Normal3(norm.X, norm.Y, norm.Z);
                    GL.TexCoord2(i * oneThroughPrecision, 2.0f * j * oneThroughPrecision);
                    GL.Vertex3(pos.X, pos.Y, pos.Z);


                    norm.X = (float)(Math.Cos(theta2) * Math.Cos(theta3));
                    norm.Y = (float)Math.Sin(theta2);
                    norm.Z = (float)(Math.Cos(theta2) * Math.Sin(theta3));
                    pos.X = center.X + radius * norm.X;
                    pos.Y = center.Y + radius * norm.Y;
                    pos.Z = center.Z + radius * norm.Z;

                    GL.Normal3(norm.X, norm.Y, norm.Z);
                    GL.TexCoord2(i * oneThroughPrecision, 2.0f * (j + 1) * oneThroughPrecision);
                    GL.Vertex3(pos.X, pos.Y, pos.Z);
                }
                GL.End();
            }
            GL.Color3(1F, 1F, 1F);
        }
        

        public static void DrawMoveTool(int HoverState, float size)
        {

            GL.PushMatrix();
            Matrix4 Scale = Matrix4.CreateScale(size);
            GL.MultMatrix(ref Scale);
            GL.Begin(PrimitiveType.Lines);
            if (HoverState == 0)
                GL.Color3(1F, 1F, 1F);
            else
                GL.Color3(1F, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0.75, 0, 0);
            if (HoverState == 1)
                GL.Color3(1F, 1F, 1F);
            else
                GL.Color3(0, 1F, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0.75, 0);
            if (HoverState == 2)
                GL.Color3(1F, 1F, 1F);
            else
                GL.Color3(0, 0, 1F);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 0.75);
            GL.End();
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < 360; i += 45)
            {
                double deg = (i + 45) * Math.PI / 180.0;
                double deg2 = (i) * Math.PI / 180.0;
                //Bull hate red, so when it wins, the fighte has Xes on for eyes!
                if (HoverState == 0)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(1F, 0, 0);
                GL.Vertex3(0.75, Math.Sin(deg) / 10, Math.Cos(deg) / 10);
                GL.Vertex3(0.75, Math.Sin(deg2) / 10, Math.Cos(deg2) / 10);
                if (HoverState == 0)
                    GL.Color3(0.75F, 0.75F, 0.75F);
                else
                    GL.Color3(0.75F, 0, 0);
                GL.Vertex3(1, 0, 0);
                if (HoverState == 0)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(1F, 0, 0);
                GL.Vertex3(0.75F, Math.Sin(deg2) / 10, Math.Cos(deg2) / 10);
                GL.Vertex3(0.75F, Math.Sin(deg) / 10, Math.Cos(deg) / 10);
                if (HoverState == 0)
                    GL.Color3(0.75F, 0.75F, 0.75F);
                else
                    GL.Color3(0.75F, 0, 0);
                GL.Vertex3(0.75, 0, 0);
                //Y is this GREEN instead of Yellow?
                if (HoverState == 1)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 1F, 0);
                GL.Vertex3(Math.Sin(deg2) / 10, 0.75, Math.Cos(deg2) / 10);
                GL.Vertex3(Math.Sin(deg) / 10, 0.75, Math.Cos(deg) / 10);
                if (HoverState == 1)
                    GL.Color3(0.75F, 0.75F, 0.75F);
                else
                    GL.Color3(0, 0.75F, 0);
                GL.Vertex3(0, 1, 0);
                if (HoverState == 1)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 1F, 0);
                GL.Vertex3(Math.Sin(deg) / 10, 0.75, Math.Cos(deg) / 10);
                GL.Vertex3(Math.Sin(deg2) / 10, 0.75, Math.Cos(deg2) / 10);
                if (HoverState == 1)
                    GL.Color3(0.75F, 0.75F, 0.75F);
                else
                    GL.Color3(0, 0.75F, 0);
                GL.Vertex3(0, 0.75, 0);
                //Deep BLUE Z!
                if (HoverState == 2)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 0, 1F);
                GL.Vertex3(Math.Sin(deg) / 10, Math.Cos(deg) / 10, 0.75);
                GL.Vertex3(Math.Sin(deg2) / 10, Math.Cos(deg2) / 10, 0.75);
                if (HoverState == 2)
                    GL.Color3(0.75F, 0.75F, 0.75F);
                else
                    GL.Color3(0, 0, 0.75F);
                GL.Vertex3(0, 0, 1);
                if (HoverState == 2)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 0, 1F);
                GL.Vertex3(Math.Sin(deg2) / 10, Math.Cos(deg2) / 10, 0.75);
                GL.Vertex3(Math.Sin(deg) / 10, Math.Cos(deg) / 10, 0.75);
                if (HoverState == 2)
                    GL.Color3(0.75F, 0.75F, 0.75F);
                else
                    GL.Color3(0, 0, 0.75F);
                GL.Vertex3(0, 0, 0.75);
            }
            GL.End();
            GL.PopMatrix();
        }

        internal static void DrawDecorateTool(float v)
        {
        }

        internal static void DrawConnectTool(float v)
        {
        }

        internal static void DrawScaleTool(int HoverState, float v)
        {
            GL.PushMatrix();
            Matrix4 Scale = Matrix4.CreateScale(v);
            GL.MultMatrix(ref Scale);
            GL.Begin(PrimitiveType.Lines);
            if (HoverState == 0)
                GL.Color3(1F, 1F, 1F);
            else
                GL.Color3(1F, 0, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0.75, 0, 0);
            if (HoverState == 1)
                GL.Color3(1F, 1F, 1F);
            else
                GL.Color3(0, 1F, 0);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0.75, 0);
            if (HoverState == 2)
                GL.Color3(1F, 1F, 1F);
            else
                GL.Color3(0, 0, 1F);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 0.75);
            GL.End();
            GL.Begin(PrimitiveType.Quads);
            {
                //Bull hate red, so when it wins, the fighte has Xes on for eyes!
                if (HoverState == 0)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(1F, 0, 0);
                GL.Vertex3(0.75, -0.125, -0.125);
                GL.Vertex3(0.75, -0.125, 0.125);
                GL.Vertex3(0.75, 0.125, 0.125);
                GL.Vertex3(0.75, 0.125, -0.125);
                GL.Vertex3(1, 0.125, 0.125);
                GL.Vertex3(1, -0.125, 0.125);
                GL.Vertex3(1, -0.125, -0.125);
                GL.Vertex3(1, 0.125, -0.125);
                GL.Vertex3(1, 0.125, 0.125);
                GL.Vertex3(0.75, 0.125, 0.125);
                GL.Vertex3(0.75, -0.125, 0.125);
                GL.Vertex3(1, -0.125, 0.125);
                GL.Vertex3(0.75, 0.125, -0.125);
                GL.Vertex3(1, 0.125, -0.125);
                GL.Vertex3(1, -0.125, -0.125);
                GL.Vertex3(0.75, -0.125, -0.125);
                GL.Vertex3(1, 0.125, -0.125);
                GL.Vertex3(0.75, 0.125, -0.125);
                GL.Vertex3(0.75, 0.125, 0.125);
                GL.Vertex3(1, 0.125, 0.125);
                GL.Vertex3(0.75, -0.125, -0.125);
                GL.Vertex3(1, -0.125, -0.125);
                GL.Vertex3(1, -0.125, 0.125);
                GL.Vertex3(0.75, -0.125, 0.125);
                //Y is this GREEN instead of Yellow?
                if (HoverState == 1)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 1F, 0);
                GL.Vertex3(0.125, 0.75, -0.125);
                GL.Vertex3(0.125, 0.75, 0.125);
                GL.Vertex3(-0.125, 0.75, 0.125);
                GL.Vertex3(-0.125, 0.75, -0.125);
                GL.Vertex3( -0.125,1, 0.125);
                GL.Vertex3( 0.125,1, 0.125);
                GL.Vertex3( 0.125,1, -0.125);
                GL.Vertex3( -0.125,1, -0.125);
                GL.Vertex3(-0.125, 1, 0.125);
                GL.Vertex3(-0.125, 0.75, 0.125);
                GL.Vertex3(0.125, 0.75, 0.125);
                GL.Vertex3(0.125, 1, 0.125);
                GL.Vertex3(0.125, 1, -0.125);
                GL.Vertex3(0.125,0.75, -0.125);
                GL.Vertex3(-0.125,0.75, -0.125);
                GL.Vertex3(-0.125,1, -0.125);
                GL.Vertex3(0.125,0.75, -0.125);
                GL.Vertex3( 0.125,1, -0.125);
                GL.Vertex3(0.125,1, 0.125);
                GL.Vertex3(0.125,0.75, 0.125);
                GL.Vertex3(-0.125, 1,-0.125);
                GL.Vertex3(-0.125,0.75, -0.125);
                GL.Vertex3(-0.125,0.75, 0.125);
                GL.Vertex3(-0.125,1, 0.125);
                //Deep BLUE Z!
                if (HoverState == 2)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 0, 1F);
                GL.Vertex3(-0.125, -0.125, 0.75);
                GL.Vertex3(-0.125, 0.125, 0.75);
                GL.Vertex3(0.125, 0.125, 0.75);
                GL.Vertex3(0.125, -0.125, 0.75);
                GL.Vertex3(0.125, 0.125,1);
                GL.Vertex3(-0.125, 0.125,1);
                GL.Vertex3(-0.125, -0.125,1);
                GL.Vertex3(0.125, -0.125,1);
                GL.Vertex3(0.125, 0.125,1);
                GL.Vertex3(0.125, 0.125, 0.75);
                GL.Vertex3(-0.125, 0.125,0.75);
                GL.Vertex3(-0.125, 0.125, 1);
                GL.Vertex3(0.125, -0.125, 0.75);
                GL.Vertex3(0.125, -0.125, 1);
                GL.Vertex3(-0.125, -0.125, 1);
                GL.Vertex3(-0.125, -0.125, 0.75);
                GL.Vertex3(0.125, -0.125, 1);
                GL.Vertex3(0.125, -0.125, 0.75);
                GL.Vertex3(0.125, 0.125, 0.75);
                GL.Vertex3(0.125, 0.125,1);
                GL.Vertex3(-0.125, -0.125, 0.75);
                GL.Vertex3(-0.125, -0.125, 1);
                GL.Vertex3(-0.125, 0.125, 1);
                GL.Vertex3(-0.125, 0.125, 0.75);
            }
            GL.End();
            if (HoverState == 3)
                GL.Color3(1F, 1F, 1F);
            else
                GL.Color3(0.75F, 0.75F, 0.75F);
            DrawBall(Vector3.Zero, 0.25F, 16);
            GL.PopMatrix();
        }

        internal static void DrawRotateTool(int HoverState, float v)
        {


            GL.PushMatrix();
            Matrix4 Scale = Matrix4.CreateScale(v);
            GL.MultMatrix(ref Scale);
            GL.LineWidth(2F);
            GL.Begin(PrimitiveType.Lines);
            for (float i = 0; i < 360; i += 360f/32f)
            {
                double deg = (i + 360f / 32f) * Math.PI / 180.0;
                double deg2 = (i) * Math.PI / 180.0;
                if (HoverState == 0)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(1F, 0, 0);
                GL.Vertex3(0, Math.Sin(deg) * 0.625F, Math.Cos(deg) * 0.625F);
                GL.Vertex3(0, Math.Sin(deg2) * 0.625F, Math.Cos(deg2) * 0.625F);
                if (HoverState == 1)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 1F, 0);
                GL.Vertex3(Math.Sin(deg) * 0.625F, 0, Math.Cos(deg) * 0.625F);
                GL.Vertex3(Math.Sin(deg2) * 0.625F, 0, Math.Cos(deg2) * 0.625F);
                if (HoverState == 2)
                    GL.Color3(1F, 1F, 1F);
                else
                    GL.Color3(0, 0, 1F);
                GL.Vertex3(Math.Sin(deg)* 0.625F, Math.Cos(deg) * 0.625F, 0);
                GL.Vertex3(Math.Sin(deg2) * 0.625F, Math.Cos(deg2) * 0.625F, 0);
            }
            GL.End();
            GL.LineWidth(1F);
            GL.PopMatrix();
        }
    }
}
