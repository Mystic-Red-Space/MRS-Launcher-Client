using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MRSLauncherClient
{
    /// <summary>
    /// ModPackControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModPackControl : UserControl
    {
        public ModPackInfo ModPack;
        public event EventHandler Click; // 컨트롤 클릭 이벤트

        public ModPackControl(ModPackInfo pack)
        {
            InitializeComponent();

            ModPack = pack;
            lvName.Content = pack.Name;
            imgIcon.Source = new BitmapImage(new Uri(pack.Icon));
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Click?.Invoke(this, new EventArgs()); // 이벤트 발생
        }
    }
}
