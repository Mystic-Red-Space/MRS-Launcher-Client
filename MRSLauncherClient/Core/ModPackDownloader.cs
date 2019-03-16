using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace MRSLauncherClient
{
    // 진행률 표시용 델리게이트
    public delegate void DownloadModFileChangedEventHandler(object sender, DownloadModFileChangedEventArgs e);

    public class ModPackDownloader
    {
        // 진행률 표시용 이벤트
        public event DownloadModFileChangedEventHandler DownloadModFileChanged;

        ModPack modPack;
        string RootPath = "";

        public ModPackDownloader(ModPack pack, string downloadPath)
        {
            this.RootPath = downloadPath;
            this.modPack = pack;
        }

        public void DownloadFiles(Dictionary<string, string> localFiles) // 로컬에 없는 파일 혹은 해쉬 다른 파일 다운로드
        {
            var webDownload = new WebDownload(); // 파일 다운로더
            webDownload.DownloadProgressChangedEvent += WebDownload_DownloadProgressChangedEvent;

            var count = modPack.ModFiles.Length; // 서버에 있는 파일들 반복 
            for (int i = 0; i < count; i++)
            {
                var item = modPack.ModFiles[i];
                var filepath = RootPath + item.Path + item.FileName;
                FireDownloadModFileChanged(count, i, item.FileName);

                var localHash = "";
                var hasFile = localFiles.TryGetValue(filepath, out localHash);

                if (!hasFile || localHash != item.MD5) // 로컬에 없으면 다운로드
                {
                    Directory.CreateDirectory(RootPath + item.Path);
                    webDownload.DownloadFile(item.Url, filepath);
                }

                localFiles.Remove(filepath);
            }
        }

        int nowValue, maxValue;
        string filename = "";

        private void WebDownload_DownloadProgressChangedEvent(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            int eValue = 0;
            eValue = nowValue + e.ProgressPercentage;

            DownloadModFileChanged?.Invoke(this, new DownloadModFileChangedEventArgs(maxValue, eValue, filename));
        }

        private void FireDownloadModFileChanged(int max, int current, string fName)
        {
            nowValue = current * 100;
            maxValue = max * 100;
            filename = fName;
            DownloadModFileChanged?.Invoke(this, new DownloadModFileChangedEventArgs(maxValue, nowValue, fName));
        }
    }

    // (진행률 표시용) 이벤트데이터 클래스
    public class DownloadModFileChangedEventArgs : EventArgs
    {
        public DownloadModFileChangedEventArgs(int maxfiles, int currentfiles, string filename)
        {
            this.MaxFiles = maxfiles;
            this.CurrentFiles = currentfiles;
            this.FileName = filename;
        }

        public int MaxFiles { get; private set; }
        public int CurrentFiles { get; private set; }

        public string FileName { get; private set; }
    }
}
