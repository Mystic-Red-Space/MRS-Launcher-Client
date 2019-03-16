using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    public class WhiteListManager
    {
        public WhiteListManager(WhiteList whitelist)
        {
            this.whiteList = whitelist.Files;
        }

        WhiteListFile[] whiteList;

        public Dictionary<string, string> WhiteFiles { get; private set; }
        public string[] WhiteDirs { get; private set; }

        public void ParseWhiteList()
        {
            var initCount = whiteList.Length;
            WhiteFiles = new Dictionary<string, string>();
            var whitedir = new List<string>();

            foreach (var item in whiteList)
            {
                var path = item.Path + item.Name;

                if (item.IsDirectory)
                    whitedir.Add(path);
                else
                    WhiteFiles.Add(path, item.Hash);
            }

            WhiteDirs = whitedir.ToArray();
        }

        public void Filtering(string root, Dictionary<string, string> compareLocal)
        {
            foreach (var item in WhiteFiles)
            {
                var path = root + item.Key;
                var hash = "";
                var hasFile = compareLocal.TryGetValue(path, out hash);

                if (hasFile && hash == item.Value)
                    compareLocal.Remove(path);
            }

            foreach (var item in compareLocal)
            {
                Console.WriteLine(item);
                File.Delete(item.Key);
            }
        }
    }
}
