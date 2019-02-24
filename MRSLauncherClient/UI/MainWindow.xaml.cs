using CmlLib.Launcher;
using MRSLauncherClient.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace MRSLauncherClient
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MSession s)
        {
            this.Session = s;
            pageManager = new PageManager();
            pageManager.PageList.AddRange(new Page[]
            {
                new HomePage(),
                new ModpacksPage(s),
                new SettingsPage(),
                new AboutPage()
            });
            InitializeComponent();
        }

        bool userClose = true;

        MSession Session;
        PageManager pageManager;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in pageManager.PageList)
            {
                var btn = new Button();
                btn.Style = (Style)FindResource("flatButton");
                btn.Content = item.Name;
                btn.Click += SideButtons_Click;
                btn.Padding = new Thickness(10, 0, 0, 0);
                sideButtons.Children.Add(btn);
            }

            contentViewer.Content = pageManager.GetContent(0);

            txtUsername.Text = Session.Username;
            var uri = "https://crafatar.com/avatars/" + Session.UUID + "?size=" + 30 + "&default=MHF_Steve" + "&overlay";
            imgProfile.Source = new BitmapImage(new Uri(uri));
        }

        private void SideButtons_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var name = btn.Content.ToString();
            contentViewer.Content = pageManager.GetContent(name);
        }

        private void BtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("정말로 로그아웃을 하시겠습니까?", "주의", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var login = new MLogin();
            login.DeleteTokenFile();

            userClose = false;
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (userClose)
                App.Stop();
        }
    }
}
