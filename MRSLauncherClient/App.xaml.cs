using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using MRSLauncherClient.UI;

namespace MRSLauncherClient
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        // ENTRY POINT
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown; // 창이 꺼져도 프로그램이 종료되지 않게

            //자바 확인
            var java = new JavaDownload(Launcher.JavaPath);

            //if (false) // 주석처리 해제시 자바설치 무시
            if (!java.CheckJavaExist())
            {
                var javaWindow = new JavaDownloadWindow(java);
                javaWindow.ShowDialog();
            }

            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            App.Stop();
        }

        // 프로그램 종료
        public static void Stop()
        {
            Setting.SaveSetting(); // 설정 저장
            Environment.Exit(0);
        }
    }
}
