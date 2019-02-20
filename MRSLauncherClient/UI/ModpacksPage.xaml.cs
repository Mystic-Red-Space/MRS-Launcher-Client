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
using MRSLauncherClient.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MRSLauncherClient
{
    /// <summary>
    /// ModpacksPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModpacksPage : Page
    {
        public ModpacksPage()
        {
            InitializeComponent();
        }

        bool IsPackLoaded = false;

        private void Modpacks_Loaded(object sender, RoutedEventArgs e) // 모드팩 로딩
        {
            if (IsPackLoaded) return;
            IsPackLoaded = true;

            var th = new Thread(new ThreadStart(delegate
            {
                ModPackInfo[] list = null;

                try
                {
                    list = ModPackLoader.GetModPackList(); // API 서버 요청
                }
                catch (System.Net.WebException)
                {
                    list = new ModPackInfo[] { };
                }

                Dispatcher.Invoke(new Action(delegate
                {
                    if (list.Length == 0)
                        lvLoading.Content = "No Modpacks";
                    else
                    {
                        foreach (var item in list) // 모드팩 컨트롤 만들어서 화면에 표시
                        {
                            var control = new ModPackControl(item);
                            control.Margin = new Thickness(100);
                            control.Click += Control_Click;
                            stPacks.Children.Add(control);
                        }

                        lvLoading.Visibility = Visibility.Collapsed;
                    }
                }));
            }));
            th.Start();
        }

        private void Control_Click(object sender, EventArgs e) // 모드팩 클릭했을때
        {
            var control = (ModPackControl)sender;
            var page = new ModpackInfoPage(control.ModPack);
            page.PageReturned += Page_PageReturned;
            frmContent.Content = page;
        }

        private void Page_PageReturned(object sender, EventArgs e)
        {
            frmContent.Content = null;
        }
    }
}
