using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using log4net;

namespace MRSLauncherClient
{
    /// <summary>
    /// AboutPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AboutPage : Page
    {
        private static ILog log = LogManager.GetLogger("AboutPage");

        public AboutPage()
        {
            InitializeComponent();
        }

        private void About_Loaded(object sender, RoutedEventArgs e)
        {
            lvLauncherVersion.Content = "런처 버전 : " + Launcher.LauncherVersion.Replace("_","__"); // Label 에서 _ 를 표시하지 못하는것을 방지
        }

        private void Button_Click(object sender, RoutedEventArgs e) // 라이센스 파일 열기
        {
            if (!File.Exists(Launcher.LisencePath))
            {
                MessageBox.Show("라이센스 파일을 찾을 수 없습니다.");
                return;
            }

            Utils.ProcessStart("notepad.exe", $"\"{Launcher.LisencePath}\"");
        }
    }
}
