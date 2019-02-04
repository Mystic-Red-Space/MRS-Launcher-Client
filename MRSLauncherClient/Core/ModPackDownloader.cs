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

        public void DownloadFiles() // 로컬에 없는 파일 혹은 해쉬 다른 파일 다운로드
        {
            var webDownload = new WebDownload();

            int fileCount = modPack.ModFiles.Length;
            for (int i = 0; i < fileCount; i++)
            {
                var item = modPack.ModFiles[i];
                var filepath = RootPath + "\\" + item.FileName;

                if (!File.Exists(filepath) || !CheckHash(filepath, item.MD5))
                    webDownload.DownloadFile(item.Url, filepath);

                FireDownloadModFileChanged(fileCount, i + 1); // 이벤트 발생
            }
        }

        private void FireDownloadModFileChanged(int max, int current)
        {
            DownloadModFileChanged?.Invoke(this, new DownloadModFileChangedEventArgs(max, current));
        }

        public void DeleteInvalidFiles() // 서버에 없는 파일 삭제
        {
            var localFiles = Directory.GetFiles(RootPath, "*.*", SearchOption.AllDirectories);
            var serverFiles = modPack.ModFiles.Select(x => RootPath + "\\" + x.FileName);

            var invalidFile = localFiles
                              .Except(serverFiles) // 허용되지 않은 파일 목록
                              .ToArray();

            foreach (var item in invalidFile)
            {
                File.Delete(item);
            }
        }

        private bool CheckHash(string filepath, string compareHash)
        {
            var hash = "";

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filepath))
                {
                    var binaryHash = md5.ComputeHash(stream);
                    hash = BitConverter.ToString(binaryHash).Replace("-", "").ToLower();
                }
            }

            return hash == compareHash;
        }
    }

    // (진행률 표시용) 이벤트데이터 클래스
    public class DownloadModFileChangedEventArgs : EventArgs
    {
        public DownloadModFileChangedEventArgs(int maxfiles, int currentfiles)
        {
            this.MaxFiles = maxfiles;
            this.CurrentFiles = currentfiles;
        }

        public int MaxFiles { get; private set; }
        public int CurrentFiles { get; private set; }
    }
}
