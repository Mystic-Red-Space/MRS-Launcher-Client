using CmlLib.Launcher;
using log4net;
using MRSLauncherClient.Core;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MRSLauncherClient
{
    /// <summary>
    /// ModpackInfoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModpackInfoPage : Page
    {
        private static ILog log = LogManager.GetLogger("ModPackInfoPage");

        public ModpackInfoPage(ModPackInfo packname, MSession session)
        {
            Session = session;
            InitializeComponent();
            PackInfo = packname;
        }

        MSession Session;
        ModPackInfo PackInfo;
        public event EventHandler PageReturned; // 뒤로가기 이벤트

        GamePatch Patcher;

        LogWindow logWindow;

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            PageReturned?.Invoke(this, new EventArgs());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            log.Info("Start ModPackLoader Thread");
            var th = new Thread(new ThreadStart(delegate // 모드팩 로딩 스레드
            {
                var pack = ModPackLoader.GetModPack(PackInfo);
                Patcher = new GamePatch(pack, Session);
                Patcher.ProgressChange += Patch_ProgressChange;
                Patcher.StatusChange += Patch_StatusChange;

                Dispatcher.Invoke(new Action(delegate
                {
                    this.IsEnabled = true;
                    lvName.Content = PackInfo.Name;

                    try
                    {
                        string packUpdateLogURL = "https://api.mysticrs.tk/update?name=" + Uri.EscapeDataString(PackInfo.Name);
                        wbUpdateViewer.Navigate(packUpdateLogURL);
                    }
                    catch (Exception ex)
                    {
                        log.Info("Cannot load update log", ex);
                    }
                }));
            }));
            th.Start();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            SetButtonsEnable(false);

            log.Info("Start GameStart Thread");
            var th = new Thread(new ThreadStart(delegate
            {
                Start(false);
            }));
            th.Start();
        }

        private void BtnForceUpdate_Click(object sender, RoutedEventArgs e)
        {
            SetButtonsEnable(false);

            log.Info("Start ForceUpdate Thread");
            var th = new Thread(new ThreadStart(delegate
            {
                Start(true);
            }));
            th.Start();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            SetButtonsEnable(false);
            lvStatus.Content = "제거 중";

            log.Info("Start Remove Thread");
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    Patcher.RemovePack();
                }
                catch (Exception ex)
                {
                    log.Info("Remove Exception", ex);
                    MessageBox.Show("제거를 실패했습니다.\n" + ex.ToString());
                }
                finally
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        SetButtonsEnable(true);
                        lvStatus.Content = "제거 완료";
                    }));
                }
            })).Start();
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Patcher.OpenFolder();
        }

        private void SetButtonsEnable(bool value)
        {
            btnStart.IsEnabled = value;
            btnForceUpdate.IsEnabled = value;
            btnRemove.IsEnabled = value;
            btnReturn.IsEnabled = value;
        }

        private void Start(bool forceUpdate)
        {
            try
            {
                log.Info("Start Patch");
                var process = Patcher.Patch(forceUpdate);

                process.GameOutput += Process_GameOutput;
                process.GameExited += Process_GameExited;

                log.Info("Start Game Process");
                process.Start();

                Dispatcher.Invoke(new Action(delegate
                {
                    SetButtonsEnable(true);

                    pbPatch.Maximum = 1;
                    pbPatch.Value = 1;
                    lvStatus.Content = "게임 실행 완료";

                    Discord.App.Presence.Details = PackInfo.Name + " 플레이 중";

                    if (Setting.Json.ShowLogWindow)
                    {
                        if (logWindow != null)
                            logWindow.Close();

                        logWindow = new LogWindow();
                        logWindow.Show();
                    }
                }));

                Thread.Sleep(3000);

                if (!process.process.HasExited)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {

                        if (MainWindow.Window.Visibility == Visibility.Visible && Setting.Json.HideLauncher)
                            MainWindow.Window.Visibility = Visibility.Hidden;
                    }));
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                log.Info("GameStart Win32Exception", ex);

                FailLaunch("WIN32 파일실행에 오류가 발생했습니다.\n" +
                    "주로 자바 실행을 실패할때 이 오류가 발생합니다. 자바를 초기화하세요.\n" + 
                    ex.ToString());
            }
            catch (Exception ex)
            {
                log.Info("GameStart Exception", ex);
                FailLaunch("알 수 없는 오류.\n" + ex.ToString());
            }
        }

        private void FailLaunch(string msg)
        {
            MessageBox.Show("게임을 실행할 수 없습니다.\n" + msg);

            Dispatcher.Invoke(new Action(delegate
            {
                SetButtonsEnable(true);
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

        private void Process_GameExited(object sender, int e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (Setting.Json.HideLauncher)
                    MainWindow.Window.Visibility = Visibility.Visible;

                if (e != 0)
                {
                    var r = MessageBox.Show(
                        "게임이 비정상적으로 종료되었습니다.\n" +
                        "'예' 버튼을 누르면 로그파일이 저장된 폴더가 열립니다. " +
                        "log.txt 파일을 개발자에게 보내주세요.",
                        "MRS Launcher",
                        MessageBoxButton.YesNo);

                    if (r == MessageBoxResult.Yes)
                        Utils.ProcessStart(Launcher.LauncherPath);
                }
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
            string scripty = "document.documentElement.style.overflowY ='auto'";
            string scriptx = "document.documentElement.style.overflowX ='hidden'";
            wbUpdateViewer.InvokeScript("execScript", new object[] { scriptx, "JavaScript" });
            wbUpdateViewer.InvokeScript("execScript", new object[] { scripty, "JavaScript" });
            wbUpdateViewer.Opacity = 1;
            wbUpdateViewer.OpacityMask = null;
        }

        private void BtnOption_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
