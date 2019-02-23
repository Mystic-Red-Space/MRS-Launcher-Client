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
using System.Windows.Shapes;
using CmlLib.Launcher;

namespace MRSLauncherClient.UI
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        


        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Launcher.email= tbEmail.Text;
            Launcher.password = tbPassword.Text;
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            //윈도우 전환
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void BtnForgotPW_Click(object sender, RoutedEventArgs e)           //Forgot Password? 버튼 클릭.
        {
            Console.WriteLine("[LoginWindow.xaml.cs] Redirecting into Minecraft Account Page for finding password.");
        }
    }
}
