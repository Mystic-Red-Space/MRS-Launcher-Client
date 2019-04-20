using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using MRSLauncherClient.UI;
using log4net;

namespace MRSLauncherClient
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        // ENTRY POINT

        private static ILog log = LogManager.GetLogger("App");

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // MUTEX Check
            if (!MutexManager.CreateMutex())
            {
                MessageBox.Show(
                    "이미 런처가 실행중입니다.",
                    "MRSLauncher",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);

                Environment.Exit(0);
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown; // 창이 꺼져도 프로그램이 종료되지 않게

            log4net.Config.XmlConfigurator.Configure();

            log.Info(" ### Start Launcher ### ");
            log.Info("Launcher Version : " + Launcher.LauncherVersion);

            //자바 확인
            log.Info("Check Java");
            var java = new JavaDownload(Launcher.JavaPath);
            var javaWorking = java.CheckJavaExist() && java.CheckJavaWork();

            //if (false) // 주석처리 해제시 자바설치 무시
            if (!javaWorking)
            {
                log.Info("Start Java Download");
                var javaWindow = new JavaDownloadWindow(java);
                javaWindow.ShowDialog();
            }

            log.Info("Start LoginWindow");
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error("UnhandledException. IsTerm : " + e.IsTerminating, (Exception)e.ExceptionObject);
        }

        // 프로그램 종료
        public static void Stop()
        {
            Setting.SaveSetting(); // 설정 저장

            log.Info("Stopping Program");
            Environment.Exit(0);
        }
    }
}
