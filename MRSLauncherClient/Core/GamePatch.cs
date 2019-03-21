using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CmlLib.Launcher;
using System.Diagnostics;
using System.IO;

namespace MRSLauncherClient
{
    public class GamePatch
    {
        public GamePatch(ModPack server, MSession s)
        {
            Session = s;
            this.Pack = server;

            RootPath = Launcher.GamePath + Pack.Name;
        }

        public event EventHandler<string> StatusChange;
        public event DownloadModFileChangedEventHandler ProgressChange;

        MSession Session;
        ModPack Pack;
        string RootPath;

        public void RemovePackJson()
        {
            var localPack = Launcher.GamePath + Pack.Name + "\\launcher\\pack.json";
            if (File.Exists(localPack))
                File.Delete(localPack);
        }

        public GameProcess Patch(bool forceUpdate)
        {
            statusChange("모드 패치 준비중");

            // 게임 폴더 만들기
            Minecraft.init(RootPath);

            var whitelist = WhiteListLoader.GetWhiteList(Pack.Name);

            bool isPackEqual = CompareLocalTempFile("pack.json", Pack.RawResponse);
            bool isWhitelistEqual = CompareLocalTempFile("whitelist.json", whitelist.RawResponse);

            if (forceUpdate || !isPackEqual || !isWhitelistEqual)
            {
                statusChange("파일 검사 중");

                // 화이트 리스트 목록 가져오기
                var whitelistManager = new WhiteListManager(whitelist);
                whitelistManager.ParseWhiteList();
                
                // 화이트 리스트 DIRS 에 없는 파일만 가져옴
                var localFileManager = new LocalFileManager(RootPath, whitelistManager.WhiteDirs);
                var localFiles = localFileManager.GetLocalFiles();

                statusChange("모드 패치 중");

                // 모드파일 다운로드
                var packDownloader = new ModPackDownloader(Pack);
                packDownloader.DownloadModFileChanged += PackDownloader_DownloadModFileChanged;
                packDownloader.DownloadFiles(RootPath, localFiles);

                statusChange("마무리 중");

                whitelistManager.Filtering(RootPath, localFiles);

                SaveLocalTempFile("pack.json", Pack.RawResponse);
                SaveLocalTempFile("whitelist.json", whitelist.RawResponse);
            }

            statusChange("게임 다운로드 준비중");

            var profileList = MProfileInfo.GetProfiles();

            MProfile startProfile = FindProfile(profileList, Pack.StartProfile);
            MProfile baseProfile = null;

            if (startProfile.IsForge)
            {
                baseProfile = FindProfile(profileList, startProfile.InnerJarId);
                DownloadProfile(baseProfile);
            }
  
            DownloadProfile(startProfile);

            var option = new MLaunchOption()
            {
                StartProfile = startProfile,
                BaseProfile = baseProfile,
                JavaPath = Launcher.JavaPath + "\\bin\\javaw.exe",
                MaximumRamMb = Setting.Json.MaxRamMb,
                Session = this.Session
            };

            if (Setting.Json.UseCustomJVM)
                option.CustomJavaParameter = Setting.Json.CustomJVMArguments;

            var launch = new MLaunch(option);
            return new GameProcess(launch.GetProcess());
        }

        public void RemovePack()
        {
            if (Directory.Exists(RootPath))
                Utils.DeleteDirectory(RootPath);
        }

        public void OpenFolder()
        {
            try
            {
                Process.Start("explorer.exe", $"\"{RootPath}\"");
            }
            catch
            {

            }
        }

        bool CompareLocalTempFile(string name, string content)
        {
            var localCheckFile = Launcher.GamePath + Pack.Name + "\\launcher\\";

            var localContent = "";
            var localFile = localCheckFile + name;
            if (File.Exists(localFile))
                localContent = File.ReadAllText(localFile, Encoding.UTF8);

            return localContent == content;
        }

        void SaveLocalTempFile(string name, string content)
        {
            var path = Launcher.GamePath + Pack.Name + "\\launcher\\" + name;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content, Encoding.UTF8);
        }

        private void PackDownloader_DownloadModFileChanged(object sender, DownloadModFileChangedEventArgs e)
        {
            progressChange(e.MaxFiles, e.CurrentFiles, e.FileName);
        }

        private MProfile FindProfile(MProfileInfo[] infos, string id)
        {
            MProfileInfo startInfo = null;
            foreach (var item in infos)
            {
                if (item.Name == id)
                {
                    startInfo = item;
                    break;
                }
            }

            if (startInfo == null)
                throw new NullReferenceException("설정한 프로파일을 찾을 수 없습니다. " + id);

            return MProfile.Parse(startInfo);
        }

        private void DownloadProfile(MProfile profile)
        {
            var downloader = new MDownloader(profile);
            downloader.ChangeFile += Downloader_ChangeFile;
            downloader.ChangeProgress += Downloader_ChangeProgress;
            downloader.DownloadAll();
        }

        #region Events

        int maxfile, nowfile;
        string filename;

        private void Downloader_ChangeProgress(object sender, ProgressChangedEventArgs e)
        {
            progressChange(maxfile, nowfile + e.ProgressPercentage, filename);
        }

        private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        {
            if (e.FileKind == MFile.Resource)
            {
                statusChange("리소스 파일 다운로드 중...");
                filename = "resources";
                progressChange(e.MaxValue, e.CurrentValue, filename);
            }
            else
            {
                filename = e.FileName;
                statusChange($"{e.FileKind.ToString()} 파일 다운로드 중...");
                maxfile = e.MaxValue * 100;
                nowfile = e.CurrentValue * 100;
            }
        }

        private void progressChange(int max, int value, string name)
        {
            ProgressChange?.Invoke(this, new DownloadModFileChangedEventArgs(max, value, name));
        }

        private void statusChange(string msg)
        {
            StatusChange?.Invoke(this, msg);
        }

        #endregion

    }
}
