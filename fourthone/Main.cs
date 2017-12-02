using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace MarioKartTrackMaker_macOS
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
