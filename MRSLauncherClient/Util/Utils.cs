using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using log4net;

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

        public static void ProcessStart(string path)
        {
            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Utils").Info("Process Start Exception" , ex);
            }
        }

        public static void ProcessStart(string path, string argument)
        {
            try
            {
                System.Diagnostics.Process.Start(path, argument);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Utils").Info("Process Start Exception", ex);
            }
        }
    }
}
