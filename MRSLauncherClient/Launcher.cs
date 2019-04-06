using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    class Launcher // 런처 전역에서 사용되는 각종 상수들을 모아둔 클래스
    {
        public static readonly string
            LauncherName = "MRS Minecraft Launcher",
            LauncherVersion = "beta_build2",
            UpdateVersion = "8",

            LauncherPath = Environment.CurrentDirectory, // 모든 파일이 저장되는 기본 경로
            GamePath = LauncherPath + "\\games\\", // saves, mods, screenshots 등이 저장되는 폴더
            JavaPath = LauncherPath + "\\runtime", // 자바가 설치되는 경로
            SettingPath = LauncherPath + "\\launcher\\launchersetting.json",
            LisencePath = LauncherPath + "\\license.txt",
            UpdaterPath = LauncherPath + "\\updater.exe",

            ModPackListUrl = "https://api.mysticrs.tk/list", // 모드팩 리스트
            ModPackDataUrl = "https://api.mysticrs.tk/modpack", // 모드팩 정보 (모드파일 등)
            WhiteListUrl = "https://api.mysticrs.tk/whitelist";
    }
}
