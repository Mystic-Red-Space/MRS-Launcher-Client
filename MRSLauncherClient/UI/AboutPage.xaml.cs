using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace MRSLauncherClient
{
    /// <summary>
    /// AboutPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void About_Loaded(object sender, RoutedEventArgs e)
        {
            lvLauncherVersion.Content = "런처 버전 : " + Launcher.LauncherVersion;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Launcher.LisencePath))
            {
                MessageBox.Show("라이센스 파일을 찾을 수 없습니다.");
                return;
            }

            Process.Start("notepad.exe", $"\"{Launcher.LisencePath}\"");
        }
    }
}
