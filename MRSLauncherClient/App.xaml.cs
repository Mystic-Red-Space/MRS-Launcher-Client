﻿using System;
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

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown; // 창이 꺼져도 프로그램이 종료되지 않게

            //업데이트 확인
            var updateTh = new Thread(new ThreadStart(delegate
            {
                var updater = new Updater();
                if (updater.CheckHasUpdate())
                {
                    var r = MessageBox.Show("업데이트할까요", "ㅇㅇ", MessageBoxButton.YesNo);

                    if (r == MessageBoxResult.Yes)
                    {
                        updater.StartUpdater();
                        Environment.Exit(0);
                    }
                }
            }));
            updateTh.IsBackground = true;
            updateTh.Start();

            //자바 확인
            var java = new JavaDownload(Launcher.JavaPath);
            var javaWorking = java.CheckJavaExist() && java.CheckJavaWork();

            //if (false) // 주석처리 해제시 자바설치 무시
            if (!javaWorking)
            {
                var javaWindow = new JavaDownloadWindow(java);
                javaWindow.ShowDialog();
            }

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        // 프로그램 종료
        public static void Stop()
        {
            Setting.SaveSetting(); // 설정 저장
            Environment.Exit(0);
        }
    }
}
