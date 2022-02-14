using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace ThinkOrSwimAlerts.Code
{
    public class AppScreenshot
    {
        public static void CaptureApplication(string procName)
        {
            Process proc;

            // Cater for cases when the process can't be located.
            try
            {
                proc = Process.GetProcessesByName(procName)[0];
            }
            catch (IndexOutOfRangeException e)
            {
                return;
            }

            var handle = proc.MainWindowHandle;

            // You need to focus on the application
            SetForegroundWindow(handle);
            ShowWindow(handle, SW_RESTORE);

            // You need some amount of delay, but 1 second may be overkill
            Thread.Sleep(1000);

            Rect rect = new Rect();
            IntPtr error = GetWindowRect(handle, ref rect);

            // sometimes it gives error.
            int retries = 0;
            while (error == (IntPtr)0 && retries < 5 )
            {
                error = GetWindowRect(proc.MainWindowHandle, ref rect);
                retries++;
            }

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using var graphic = Graphics.FromImage(bmp);

            graphic.CopyFromScreen(rect.left,
                rect.top,
                0,
                0,
                new Size(width, height),
                CopyPixelOperation.SourceCopy);

            WriteBitmapToFile("C:/Users/Admin/WindowsServices/ThinkOrSwimAlerts/ThinkOrSwimAlerts/screenshots/tos.png", bmp);
        }

        public static void WriteBitmapToFile(string filename, Bitmap bitmap)
        {
            bitmap.Save(filename, ImageFormat.Png);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }


        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        private static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
    }
}
