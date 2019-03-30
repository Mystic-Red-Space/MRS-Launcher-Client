using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MRSUpdater
{
    public class Updater
    {
        public Updater(string root)
        {
            RootPath = root;
            
        }

        public event FileChangeEventHandler FileChanged;
        public event ProgressChangedEventHandler ProgressChanged;
        string RootPath;

        public void Update(Dictionary<string, string> local, UpdateFile[] update)
        {
            var downloader = new WebDownload();
            downloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;

            var count = update.Length; // 서버에 있는 파일들 반복 
            for (int i = 0; i < count; i++)
            {
                var item = update[i];
                var filepath = RootPath + item.FileName;
                FileChange(count, i);

                var localHash = "";
                var hasFile = local.TryGetValue(filepath, out localHash);

                if (!hasFile || localHash != item.MD5) // 로컬에 없으면 다운로드
                {
                    downloader.DownloadFile(item.Url, filepath);
                }

                local.Remove(filepath);
            }
        }

        bool FileValidate(string filepath, string hash)
        {
            if (!File.Exists(filepath))
                return false;

            var originHash = "";
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filepath))
                {
                    var binaryHash = md5.ComputeHash(stream);

                    originHash = BitConverter.ToString(binaryHash)
                                .Replace("-", "")
                                .ToLower();
                }
            }

            return originHash == hash;
        }

        void FileChange(int max, int value)
        {
            FileChanged?.Invoke(this, new FileChangeEventArgs(max, value));
        }

        private void Downloader_DownloadProgressChangedEvent(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }

    public delegate void FileChangeEventHandler(object sender, FileChangeEventArgs e);

    public class FileChangeEventArgs
    {
        public FileChangeEventArgs(int max, int value)
        {
            MaxCount = max;
            NowCount = value;
        }

        public int MaxCount { get; private set; }
        public int NowCount { get; private set; }
    }
}
