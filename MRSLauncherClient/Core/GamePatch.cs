using CmlLib.Launcher;
using log4net;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace MRSLauncherClient.Core
{
    public class GamePatch
    {
        private ILog log = LogManager.GetLogger("GamePatch");

        public GamePatch(ModPack server, MSession s)
        {
            Session = s;
            this.Pack = server;

            RootPath = Launcher.GamePath + Pack.Name;

            log.Info($"New GamePatch : {Pack.Name} ({s.Username})");
        }

        public event EventHandler<string> StatusChange;
        public event DownloadModFileChangedEventHandler ProgressChange;

        MSession Session;
        ModPack Pack;
        string RootPath;

        public void RemovePackJson()
        {
            log.Info("Remove Pack Json");

            var localPack = Launcher.GamePath + Pack.Name + "\\launcher\\pack.json";
            if (File.Exists(localPack))
                File.Delete(localPack);
        }

        public GameProcess Patch(bool forceUpdate)
        {
            d("Start ModPack Patch. forceUpdate : " + forceUpdate);
            statusChange("모드 패치 준비중");

            // 게임 폴더 만들기
            d("Initialize Game Path : " + RootPath);
            Minecraft.Initialize(RootPath);

            d("Get Whitelist");
            var whitelist = WhiteListLoader.GetWhiteList(Pack.Name);

            d("Check LocalTempFile");
            bool isPackEqual = CompareLocalTempFile("pack.json", Pack.RawResponse);
            bool isWhitelistEqual = CompareLocalTempFile("whitelist.json", whitelist.RawResponse);

            if (forceUpdate || !isPackEqual || !isWhitelistEqual)
            {
                d("Start File Patch");
                statusChange("파일 검사 중");

                // 화이트 리스트 목록 가져오기
                d("Get WhiteList Files");
                var whitelistManager = new WhiteListManager(whitelist);
                whitelistManager.ParseWhiteList();

                // 화이트 리스트 DIRS 에 없는 파일만 가져옴
                d("Get Local Files");
                var localFileManager = new LocalFileManager(RootPath, whitelistManager.WhiteDirs);
                var localFiles = localFileManager.GetLocalFiles();

                statusChange("모드 패치 중");

                // 모드파일 다운로드
                d("Start File Checking and Downloading");
                var packDownloader = new ModPackDownloader(Pack);
                packDownloader.DownloadModFileChanged += PackDownloader_DownloadModFileChanged;
                packDownloader.DownloadFiles(RootPath, localFiles);

                statusChange("마무리 중");

                d("Remove Filtered Files");
                whitelistManager.Filtering(RootPath, localFiles);

                d("Save LocalTempFile");
                SaveLocalTempFile("pack.json", Pack.RawResponse);
                SaveLocalTempFile("whitelist.json", whitelist.RawResponse);
            }

            statusChange("게임 다운로드 준비중");

            d("Get Profiles");
            var profileList = MProfileInfo.GetProfiles();

            d("Search StartProfile");
            MProfile startProfile = MProfile.GetProfile(profileList, Pack.StartProfile);

            d("Download StartProfile");
            DownloadProfile(startProfile);

            d("Create LaunchOption");
            var option = new MLaunchOption()
            {
                StartProfile = startProfile,
                JavaPath = Launcher.JavaPath + "\\bin\\javaw.exe",
                MaximumRamMb = Setting.Json.MaxRamMb,
                Session = this.Session
            };

            if (Setting.Json.UseCustomJVM)
                option.CustomJavaParameter = Setting.Json.CustomJVMArguments;

            d("Create GameProcess");
            var launch = new MLaunch(option);
            return new GameProcess(launch.GetProcess());
        }

        public void RemovePack()
        {
            d("Remove Pack");

            if (Directory.Exists(RootPath))
                Utils.DeleteDirectory(RootPath);
        }

        public void OpenFolder()
        {
            if (Directory.Exists(RootPath))
                Utils.ProcessStart("explorer.exe", $"\"{RootPath}\"");
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
            d("Download Profile : " + profile.Id);

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

        void d(string msg)
        {
            log.Info($"[{Pack.Name}] {msg}");
        }
    }
}
