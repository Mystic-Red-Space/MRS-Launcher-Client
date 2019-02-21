﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CmlLib.Launcher;
using System.Diagnostics;

namespace MRSLauncherClient
{
    public class GamePatch
    {
        public GamePatch(ModPack server, MSession s)
        {
            Session = s;
            this.Pack = server;
        }

        public event EventHandler<string> StatusChange;
        public event ProgressChangedEventHandler ProgressChange;

        MSession Session;
        ModPack Pack;

        public GameProcess Patch()
        {
            statusChange("모드 패치중");

            var packDownloader = new ModPackDownloader(Pack, Launcher.GamePath + Pack.Name);
            packDownloader.DownloadModFileChanged += PackDownloader_DownloadModFileChanged;
            packDownloader.DownloadFiles();
            packDownloader.DeleteInvalidFiles();

            statusChange("게임 다운로드 준비중");

            SetGamePath();

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

        private void PackDownloader_DownloadModFileChanged(object sender, DownloadModFileChangedEventArgs e)
        {
            progressChange(e.MaxFiles, e.CurrentFiles);
        }

        private void SetGamePath()
        {
            var root = Launcher.GamePath + Pack.Name;

            Minecraft.init(root);
            //Minecraft.Library = Launcher.CommonPath + "libraries\\";
            //Minecraft._Library = Launcher.CommonPath + "libraries";
            //Minecraft.Versions = Launcher.CommonPath + "versions\\";
            //Minecraft._Versions = Launcher.CommonPath + "versions";
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

        int maxfile, nowfile;

        private void Downloader_ChangeProgress(object sender, ProgressChangedEventArgs e)
        {
            progressChange(maxfile, nowfile + e.ProgressPercentage);
        }

        private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        {
            if (e.FileKind == MFile.Resource)
            {
                statusChange("리소스 파일 다운로드 중...");
                progressChange(e.MaxValue, e.CurrentValue);
            }
            else
            {
                statusChange($"{e.FileKind.ToString()} 파일 다운로드 중... ({e.FileName})");
                maxfile = e.MaxValue * 100;
                nowfile = e.CurrentValue * 100;
            }
        }

        private void progressChange(int max, int value)
        {
            var progress = 0;

            if (max == 100)
                progress = value;
            else
                progress = (int)((float)value / max * 100);

            ProgressChange?.Invoke(this, new ProgressChangedEventArgs(progress, null));
        }

        private void statusChange(string msg)
        {
            StatusChange?.Invoke(this, msg);
        }
    }
}