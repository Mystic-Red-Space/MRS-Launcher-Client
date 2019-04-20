using System.Collections.Generic;
using System.IO;
using log4net;

namespace MRSLauncherClient
{
    public class WhiteListManager
    {
        private static ILog log = LogManager.GetLogger("WhiteListManager");

        public WhiteListManager(WhiteList whitelist)
        {
            this.whiteList = whitelist.Files;
        }

        WhiteListFile[] whiteList;

        public Dictionary<string, string> WhiteFiles { get; private set; }
        public string[] WhiteDirs { get; private set; }

        public void ParseWhiteList() // 화이트리스트 목록을 보고 파일과 폴더를 구분함
        {
            log.Info("ParseWhiteList");
                
            WhiteFiles = new Dictionary<string, string>();
            var whitedir = new List<string>(5);

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

        public void Filtering(string root, Dictionary<string, string> compareLocal) // 화이트리스트에 없는 파일을 제거
        {
            log.Info("Filtering Files");

            foreach (var item in WhiteFiles)
            {
                var path = root + item.Key;
                var hash = "";
                var hasFile = compareLocal.TryGetValue(path, out hash);

                if (hasFile && (hash == null || hash == "" || hash == item.Value))
                    compareLocal.Remove(path);
            }

            foreach (var item in compareLocal)
            {
                File.Delete(item.Key);
            }

            log.Info(compareLocal.Count + " of files deleted");
        }
    }
}
