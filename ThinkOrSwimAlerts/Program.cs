using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using ThinkOrSwimAlerts.Code;

namespace ThinkOrSwimAlerts
{
    class Program
    {
        static void Main(string[] args)
        {
            AppScreenshot.CaptureApplication("thinkorswim");
        }
    }
}
