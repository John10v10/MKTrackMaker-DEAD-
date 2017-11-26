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
    public partial class MainViewController : AppKit.NSViewController
    {

        #region Constructors

        // Called when created from unmanaged code
        public MainViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public MainViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public MainViewController() : base("MainView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new MainView View
        {
            get
            {
                return (MainView)base.View;
            }
        }

    }
}
