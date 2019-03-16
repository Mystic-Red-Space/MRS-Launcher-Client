using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading;
using CmlLib.Launcher;
using mshtml;

namespace MRSLauncherClient.UI
{
    /// <summary>
    /// ModpackInfoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModpackInfoPage : Page
    {
        public ModpackInfoPage(ModPackInfo packname, MSession session)
        {
            Session = session;
            InitializeComponent();
            PackInfo = packname;
        }

        MSession Session;
        public ModPackInfo PackInfo;
        public ModPack Pack;
        public event EventHandler PageReturned; // 뒤로가기 이벤트

        LogWindow logWindow;

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            PageReturned?.Invoke(this, new EventArgs());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            var th = new Thread(new ThreadStart(delegate // 모드팩 로딩 스레드
            {
                Pack = ModPackLoader.GetModPack(PackInfo);

                Dispatcher.Invoke(new Action(delegate
                {
                    this.IsEnabled = true;
                    lvName.Content = PackInfo.Name;
                    string packUpdateLogURL = "https://api.mysticrs.tk/update?name=" + Uri.EscapeDataString(PackInfo.Name);
                    wbUpdateViewer.Navigate(packUpdateLogURL);
                }));
            }));
            th.Start();
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            btnReturn.IsEnabled = false;

            var th = new Thread(Start);
            th.Start();
        }

        private void Start()
        {
            try
            {
                Setting.Json.MaxRamMb = 6000;

                var patch = new GamePatch(Pack, Session);
                patch.ProgressChange += Patch_ProgressChange;
                patch.StatusChange += Patch_StatusChange;
                var process = patch.Patch(false);
                process.GameOutput += Process_GameOutput;
                process.StartDebug();

                Dispatcher.Invoke(new Action(delegate
                {
                    btnStart.IsEnabled = true;
                    btnReturn.IsEnabled = true;

                    pbPatch.Maximum = 1;
                    pbPatch.Value = 1;
                    lvStatus.Content = "게임 실행 완료";

                    if (logWindow != null)
                        logWindow.Close();

                    logWindow = new LogWindow();
                    logWindow.Show();
                }));
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                FailLaunch("WIN32 파일실행에 오류가 발생했습니다.\n" +
                    "주로 자바 실행을 실패할때 이 오류가 발생합니다. 자바를 초기화하세요.\n" + 
                    ex.ToString());
            }
            catch (Exception ex)
            {
                FailLaunch("알 수 없는 오류.\n" + ex.ToString());
            }
        }

        private void FailLaunch(string msg)
        {
            MessageBox.Show("게임을 실행할 수 없습니다.\n" + msg);

            Dispatcher.Invoke(new Action(delegate
            {
                btnStart.IsEnabled = true;
                btnReturn.IsEnabled = true;

                pbPatch.Maximum = 1;
                pbPatch.Value = 1;
                lvStatus.Content = "게임 실행 실패";
            }));
        }

        private void Process_GameOutput(object sender, string e)
        {
            if (logWindow == null)
                return;

            Dispatcher.Invoke(new Action(delegate
            {
                logWindow.AppendLog(e+"\n");
            }));
        }

        private void Patch_StatusChange(object sender, string e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                lvStatus.Content = e;
            }));
        }

        private void Patch_ProgressChange(object sender, DownloadModFileChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                pbPatch.Maximum = e.MaxFiles;
                pbPatch.Value = e.CurrentFiles;
                lvFileName.Content = e.FileName;
            }));
        }

        void wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string script = "document.documentElement.style.overflow ='hidden'";
            wbUpdateViewer.InvokeScript("execScript", new object[] { script, "JavaScript" });
            wbUpdateViewer.Opacity = 1;
            wbUpdateViewer.OpacityMask = null;
        }
    }
}
