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

namespace MRSLauncherClient.UI
{
    /// <summary>
    /// ModpackInfoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModpackInfoPage : Page
    {
        public ModpackInfoPage(string packname)
        {
            InitializeComponent();

            PackName = packname;
        }

        public string PackName;
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
                Pack = ModPackLoader.GetModPack(PackName);

                Dispatcher.Invoke(new Action(delegate
                {
                    this.IsEnabled = true;
                    lvName.Content = PackName;

                    //wbUpdateViewer : 업데이트 로그 뷰어
                    wbUpdateViewer.Navigate("https://files.mysticrs.tk/update_test.html");
                }));
            }));
            th.Start();
        }
    }
}
