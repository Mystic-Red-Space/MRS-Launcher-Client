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
using System.Threading;
using CmlLib.Launcher;

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
                var patch = new GamePatch(Pack, Session);
                patch.ProgressChange += Patch_ProgressChange;
                patch.StatusChange += Patch_StatusChange;
                var process = patch.Patch();
                process.GameOutput += Process_GameOutput;
                process.StartDebug();

                Dispatcher.Invoke(new Action(delegate
                {
                    btnStart.IsEnabled = true;
                    btnReturn.IsEnabled = true;

                    pbPatch.Maximum = 1;
                    pbPatch.Value = 1;
                    lvStatus.Content = "게임 실행 완료";
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("게임을 실행할 수 없습니다.\n"+ex.ToString());
            }
        }

        private void Process_GameOutput(object sender, string e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                rtLog.AppendText(e + "\n");
                rtLog.ScrollToEnd();
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
            }));
        }
    }
}
