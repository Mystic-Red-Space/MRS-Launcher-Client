using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MRSLauncherClient
{
    class Updater
    {
        public bool CheckHasUpdate()
        {
            var webver = Web.Request(Launcher.VersionUrl);
            return webver != Launcher.UpdateVersion; 
        }

        public void StartUpdater()
        {
            if (!File.Exists(Launcher.UpdaterPath))
            {
                System.Windows.MessageBox.Show("updater.exe 를 찾을 수 없습니다.\n" +
                    "런처를 다시 설치해 주세요.");

                return;
            }

            Process.Start(Launcher.UpdaterPath);
        }
    }
}
