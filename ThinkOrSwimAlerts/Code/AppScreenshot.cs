﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using ThinkOrSwimAlerts.Configs;

namespace ThinkOrSwimAlerts.Code
{
    public class AppScreenshot
    {
        public static void CaptureApplication(string procName, DotColors dotColors)
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

            // Neither of these work
            //SetWindowPos(handle, 0, 0, 0, 1260, 756, 0 );
            //MoveWindow(handle, 0, 0, 1260, 756, true);

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

            //var dot = (Bitmap)Bitmap.FromFile("C:/Users/Admin/Pictures/Screenshots/blue_dot.png");
            //var histo = GetHistogram(dot);

            var histo = GetHistogram(bmp);

            var count = histo[dotColors.Sell];

            //Console.WriteLine("blah");

            WriteBitmapToFile("C:/Users/Admin/WindowsServices/ThinkOrSwimAlerts/ThinkOrSwimAlerts/screenshots/blue_dot.png", bmp);
        }

        private static Dictionary<string, int> GetHistogram(Bitmap bmp, DotColors colors)
        {
            // Store the histogram in a dictionary          
            Dictionary<string, int> histo = new Dictionary<string, int>();
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    // Get pixel color 
                    string c = bmp.GetPixel(x, y).Name;
                    // If it exists in our 'histogram' increment the corresponding value, or add new
                    if (histo.ContainsKey(c))
                        histo[c] = histo[c] + 1;
                    else
                        histo.Add(c, 1);
                }
            }

            return histo;
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

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    }
}