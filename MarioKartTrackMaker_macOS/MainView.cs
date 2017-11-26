using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.MacOS;

using Foundation;
using AppKit;
using CoreGraphics;

namespace MarioKartTrackMaker_macOS
{
    public partial class MainView : NSView
    {
        #region Computed Properties
        public MonoMacGameView centralView { get; set; }
        #endregion

        #region Constructors

        // Called when created from unmanaged code
        public MainView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public MainView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        #region Override Methods
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            centralView = new MonoMacGameView(Frame);
            centralView.Frame = Frame;

            // Wire-up any required Game events
            centralView.Load += (sender, e) =>
            {
                // TODO: Initialize settings, load textures and sounds here
            };

            centralView.Resize += (sender, e) =>
            {
                GL.Viewport(0, 0, centralView.Size.Width, centralView.Size.Height);
            };

            centralView.UpdateFrame += (sender, e) =>
            {
                // TODO: Add any game logic or physics
            };

            centralView.RenderFrame += (sender, e) =>
            {
                // Setup buffer
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.MatrixMode(MatrixMode.Projection);

                // Draw a simple triangle
                GL.LoadIdentity();
                GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
                GL.Begin(BeginMode.Triangles);
                GL.Color3(Color.MidnightBlue);
                GL.Vertex2(-1.0f, 1.0f);
                GL.Color3(Color.SpringGreen);
                GL.Vertex2(0.0f, -1.0f);
                GL.Color3(Color.Ivory);
                GL.Vertex2(1.0f, 1.0f);
                GL.End();
            };

            centralView.Run(60.0);
        }
        #endregion
    }
}
