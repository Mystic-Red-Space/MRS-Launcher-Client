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

namespace MRSLauncherClient
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            pageManager = new PageManager();

            InitializeComponent();
        }

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
        }

        private void SideButtons_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var name = btn.Content.ToString();
            contentViewer.Content = pageManager.GetContent(name);
        }
    }
}
