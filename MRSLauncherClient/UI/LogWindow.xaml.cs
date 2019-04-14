using System.Windows;

namespace MRSLauncherClient
{
    /// <summary>
    /// LogWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public void AppendLog(string msg)
        {
            rt.AppendText(msg);
            rt.ScrollToEnd();
        }
    }
}
