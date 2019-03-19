using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    public class Utils
    {
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] directories = Directory.GetDirectories(target_dir);
            foreach (string path in files)
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
            foreach (string target_dir1 in directories)
                DeleteDirectory(target_dir1);
            Directory.Delete(target_dir, true);
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void SetWindowBorderNone(IntPtr hwnd)
        {
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}
