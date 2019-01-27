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
using System.Windows.Shapes;
using System.Threading;

namespace MRSLauncherClient
{
    /// <summary>
    /// JavaDownloadWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JavaDownloadWindow : Window
    {
        JavaDownload javaDownload;

        public JavaDownloadWindow(JavaDownload java)
        {
            this.javaDownload = java;

            InitializeComponent();
            this.pbProgress.Maximum = 100;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var th = new Thread(InstallJava);
            th.Start();
        }

        private void InstallJava()
        {
            try
            {
                javaDownload.ProgressChanged += Java_ProgressChanged;
                javaDownload.DownloadCompleted += Java_DownloadCompleted;
                javaDownload.UnzipCompleted += Java_UnzipCompleted;
                javaDownload.InstallJava();
            }
            catch (System.Net.WebException webEx)
            {
                MessageBox.Show("자바 다운로드 서버에 연결할 수 없습니다.");

                App.Stop(); // 프로그램 종료
            }
            catch (Exception ex)
            {
                MessageBox.Show("자바를 다운로드 할 수 없습니다.");
                App.Stop();
            }
        }

        private void Java_UnzipCompleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                lvStatus.Content = "다운로드가 완료되었습니다.";
                this.Close();
            }));
        }

        private void Java_DownloadCompleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                lvStatus.Content = "압축 해제 중";
            }));
        }

        private void Java_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                pbProgress.Value = e.ProgressPercentage;
            }));
        }
    }
}
