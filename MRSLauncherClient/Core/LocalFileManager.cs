using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TESTCSHARP
{
    class LocalFileManager
    {
        public LocalFileManager(string rootPath, string[] filterDir)
        {
            this.RootPath = rootPath;
            filterDirs = new HashSet<string>();
            foreach (var item in filterDir)
            {
                filterDirs.Add(item);
            }
        }

        string RootPath = "";
        HashSet<string> filterDirs;

        public Dictionary<string, string> GetLocalFiles()
        {
            var result = new Dictionary<string, string>();
            GetLocalFiles("", result);
            return result;
        }

        private Dictionary<string, string> GetLocalFiles(string path, Dictionary<string, string> result)
        {
            if (filterDirs.Contains(path))
                return result;

            var absolutePath = RootPath + path;

            var files = Directory.GetFiles(absolutePath);
            foreach (var item in files)
            {
                result.Add(item, GetFileHash(item));
            }

            foreach (var item in new DirectoryInfo(absolutePath).GetDirectories("*.*", SearchOption.TopDirectoryOnly))
            {
                GetLocalFiles(path + "\\" + item.Name, result);
            }

            return result;
        }

        public static string GetFileHash(string path)
        {
            var binaryHash = GetFileHashArray(path);
            return ByteArrayToHex(binaryHash);
        }

        public static byte[] GetFileHashArray(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var binaryHash = md5.ComputeHash(stream);
                    return binaryHash;
                }
            }
        }

        private static string ByteArrayToHex(byte[] arr)
        {
            return BitConverter.ToString(arr).Replace("-", "").ToLower();
        }
    }
}
