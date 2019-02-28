using Ionic.Zip;
using System;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;

namespace MRSLauncherClient
{
    public class JavaDownload
    {
        static readonly string JavaUrl = "https://files.mysticrs.tk/jre.zip";

        public JavaDownload(string path) // 자바 경로 설정
        {
            this.JavaPath = path;
        }

        public string JavaPath { get; private set; } // 자바경로
        public event ProgressChangedEventHandler ProgressChanged; // 진행률 바뀔때
        public event EventHandler DownloadCompleted; // 다운로드 완료됬을때
        public event EventHandler UnzipCompleted; // 압축해제 완료됬을때

        public bool CheckJavaExist() // 자바 설치됬는지 확인
        {
            var exePath = JavaPath + "\\bin\\java.exe";
            return File.Exists(exePath);
        }

        public bool CheckJavaWork() // 잘 작동하는지 확인
        {
            var javaproc = new Process();
            javaproc.StartInfo = new ProcessStartInfo
            {
                FileName = JavaPath + "\\bin\\java.exe",
                Arguments = "-version",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };

            javaproc.Start();

            var exited = javaproc.WaitForExit(2);
            if (exited)
                return javaproc.ExitCode == 0;
            else
                return false;
        }

        public void InstallJava() // 자바 설치
        {
            var tempJavaPath = Path.GetTempPath() + "\\javazip\\jre.zip"; // zip 파일 저장될 경로
            Directory.CreateDirectory(Path.GetDirectoryName(tempJavaPath)); // 임시폴더생성

            if (Directory.Exists(JavaPath))
                DeleteDirectory(JavaPath);
            Directory.CreateDirectory(JavaPath); // 자바가 설치될 폴더 생성

            var webDownload = new WebDownload(); // zip 파일 다운로드
            webDownload.DownloadProgressChangedEvent += WebDownload_DownloadProgressChangedEvent;
            webDownload.DownloadFile(JavaUrl, tempJavaPath);

            DownloadCompleted?.Invoke(this, new EventArgs());

            using (var zip = ZipFile.Read(tempJavaPath)) // 압축해제
            {
                zip.ExtractProgress += Zip_ExtractProgress;
                zip.ExtractAll(JavaPath, ExtractExistingFileAction.OverwriteSilently);
            }

            UnzipCompleted?.Invoke(this, new EventArgs());
        }

        private void WebDownload_DownloadProgressChangedEvent(object sender, ProgressChangedEventArgs e) // 다운로드 진행률
        {
            ProgressChanged?.Invoke(this, e);
        }

        int nowValue, maxValue; // 압축해제 진행률 계산
        void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
            {
                nowValue = e.EntriesExtracted * 100;
                maxValue = e.EntriesTotal * 100;
            }
            else if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
            {
                int eValue = 0;
                var rate = ((float)e.BytesTransferred / e.TotalBytesToTransfer) * 100;
                eValue = nowValue + (int)rate;

                // 이벤트 발생
                var percent = ((float)eValue / maxValue) * 100;
                var eventArgs = new ProgressChangedEventArgs((int)percent, null);
                ProgressChanged?.Invoke(this, eventArgs);
            }
        }

        private void DeleteDirectory(string target_dir)
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
    }
}