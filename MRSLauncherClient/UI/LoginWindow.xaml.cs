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
        //e-mail과 password 변수
        public static string email = null;
        private string pw = null;         

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //email = tbEmail.Text;
            email = "lapis0875@gmail.com";
            pw = tbPassword.Text;
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            MLogin login = new MLogin();
            MSession session = null;

            session = login.TryAutoLogin();                     //자동로그인 시도
            if (session.Result != MLoginResult.Success)         //자동로그인 실패시 이메일과 패스워드를 얻어 로그인 시도
            {
                session = login.Authenticate(email, pw);
            }
            if (session.Result != MLoginResult.Success)         //로그인 실패시 잘못된 계정이라는 오류 던짐.
            {
                throw new Exception("[LoginWindow.xaml.cs] You tried to log in using wrong account!");
            }

            Console.WriteLine("Hello, " + session.Username);
        }
        private void BtnForgotPW_Click(object sender, RoutedEventArgs e)           //Forgot Password? 버튼 클릭.
        {
            Console.WriteLine("[LoginWindow.xaml.cs] Redirecting into Minecraft Account Page for finding password.");
        }
    }
}
