using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MarioKartTrackMaker_macOS
{
    public partial class MarioKartGLView : AppKit.NSOpenGLView
    {
        #region Constructors

        // Called when created from unmanaged code
        public MarioKartGLView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public MarioKartGLView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion
    }
}
