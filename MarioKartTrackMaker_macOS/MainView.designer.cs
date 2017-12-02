// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MarioKartTrackMaker_macOS
{
	[Register ("MainView")]
	partial class MainView
	{
		[Outlet]
		MarioKartTrackMaker_macOS.MainView contentView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (contentView != null) {
				contentView.Dispose ();
				contentView = null;
			}
		}
	}
}
