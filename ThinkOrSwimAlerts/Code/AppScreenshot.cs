using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Serilog;
using ThinkOrSwimAlerts.Configs;
using ThinkOrSwimAlerts.Enums;

namespace ThinkOrSwimAlerts.Code
{
    public class AppScreenshot
    {
        public static BuyOrSell? DetectBuyOrSellSignal(DotColors dotColors)
        {
            Process proc;

            // Cater for cases when the process can't be located.
            try
            {
                proc = Process.GetProcessesByName("thinkorswim")[0];
            }
            catch (IndexOutOfRangeException e)
            {
                return null;
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
            //var dot = (Bitmap)Bitmap.FromFile("C:/Users/Admin/WindowsServices/ThinkOrSwimAlerts/ThinkOrSwimAlerts/screenshots/arrows2.png");
            //var color = GetDotColor(dot, dotColors);

            if (! isInitted)
            {
                InitFirstHisto(bmp, dotColors);
                isInitted = true;
            }

            var color = GetDotColor(bmp, dotColors);

            if (color == dotColors.Sell)
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "Sell");
                return BuyOrSell.Sell;
            }
            else if (color == dotColors.Buy)
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "Buy");
                return BuyOrSell.Buy;
            }
            else
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "Do nothing");
            }

            //var count = histo[dotColors.Sell];

            //Console.WriteLine("blah");

            //WriteBitmapToFile("C:/Users/Admin/WindowsServices/ThinkOrSwimAlerts/ThinkOrSwimAlerts/screenshots/blue_half.png", bmp);

            return null;
        }

        private static bool isInitted = false;
        private static Dictionary<string, int> pastColors;

        private static void InitFirstHisto(Bitmap bmp, DotColors colors)
        {
            // Store the histogram in a dictionary          
            pastColors = new Dictionary<string, int>();
            for (int x = bmp.Width - 1; x >= 0; x--)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    // Get pixel color 
                    string c = bmp.GetPixel(x, y).Name;
                    if (c == colors.Sell || c == colors.Buy)
                    {
                        // If it exists in our 'histogram' increment the corresponding value, or add new
                        if (pastColors.ContainsKey(c))
                        {
                            pastColors[c]++;
                            // TODO 16 pixels in large dot for POLY. This could change
                            // TODO 32 pixels in arrow for Macnsqueeze. This could change
                            //if (histo[c] == 32)
                            //{
                            //    return c;
                            //}
                        }
                        else
                            pastColors.Add(c, 1);
                    }
                }
            }

            foreach (var color in pastColors.Keys)
            {
                Log.Information($"Added color {color} with value {pastColors[color]} (numArrows = {pastColors[color]/32}) to pastColors during Init");
                
            }
        }

        private static string GetDotColor(Bitmap bmp, DotColors colors)
        {
            // Store the histogram in a dictionary          
            Dictionary<string, int> histo = new Dictionary<string, int>();
            for (int x = bmp.Width - 1; x >= 0; x--)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    // Get pixel color 
                    string c = bmp.GetPixel(x, y).Name;
                    if (c == colors.Sell || c == colors.Buy)
                    {
                        // If it exists in our 'histogram' increment the corresponding value, or add new
                        if (histo.ContainsKey(c))
                        {
                            histo[c]++;
                            // TODO 16 pixels in large dot for POLY. This could change
                            // TODO 32 pixels in arrow for Macnsqueeze. This could change
                            //if (histo[c] == 32)
                            //{
                            //    return c;
                            //}
                        }
                        else
                            histo.Add(c, 1);
                    }
                }
            }

            foreach (var key in histo.Keys)
            {
                // If the cursor is over part of the study, the arrows will enlarge, so we need a mechanism to tell
                // whether this is just an enlargement or not.
                if (histo[key] % 32 != 0)
                {
                    return "";
                }

                if (!pastColors.ContainsKey(key))
                {
                    pastColors.Add(key, histo[key]);
                    Log.Information($"Adding new color {key} to pastColors with value {histo[key]} (numArrows = {(float)pastColors[key]/32.0})");
                    return key;
                }

                if (histo[key] == pastColors[key] + 32)
                {
                    Log.Information($"Color {key} at {histo[key]}:{(float)histo[key]/32.0}- greater than past value of {pastColors[key]}:{(float)pastColors[key]/32.0}");
                    pastColors[key] = histo[key];
                    return key;
                }
            }

            return "";
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
