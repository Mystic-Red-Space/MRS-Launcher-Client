using CmlLib.Launcher;
using log4net;
using MRSLauncherClient.Core;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MRSLauncherClient
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Singleton
        private static MainWindow instance;
        public static MainWindow Window { get => instance; }
        #endregion

        private static ILog log = LogManager.GetLogger("MainWindow");

        public MainWindow(MSession s)
        {
            instance = this;
            this.Session = s;

            log.Info("Load Page");
            pageManager = new PageManager();
            pageManager.PageList.AddRange(new Page[]
            {
                new HomePage(),
                new ModpacksPage(s),
                new SettingsPage(),
                new AboutPage()
            });

            log.Info("Load Component");
            InitializeComponent();
        }

        bool userClose = true;

        MSession Session;
        PageManager pageManager;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            log.Info("Window Loaded");

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
            imgProfile.Source = MAvatar.GetAvatarImage(Session.UUID);
            Discord.App.Presence.State = Session.Username + " 으로 플레이 중";
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

            log.Info("Delete Token File");
            var login = new MLogin();
            login.DeleteTokenFile();

            LoginCache.ClearPassword();

            log.Info("Show LoginWindow");
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
