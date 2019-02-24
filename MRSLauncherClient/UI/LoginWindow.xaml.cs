using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private void HyperlinkForgotPW_Click(object sender, RoutedEventArgs e)           //Forgot Password? 버튼 클릭.
        {
            Console.WriteLine("[LoginWindow.xaml.cs] Redirecting into Minecraft Account Page for finding password.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetPanelEnable(false);
            var loginth = new Thread(new ThreadStart(delegate
            {
                var login = new MLogin();
                var result = login.TryAutoLogin();
                Dispatcher.Invoke(new Action(delegate
                {
                    SetPanelEnable(true);
                    if (result.Result == MLoginResult.Success)
                    {
                        ShowWelcome(result);
                    }
                }));
            }));
            loginth.Start();
        }

        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text == "email")
                tbEmail.Clear();
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text == "")
                tbEmail.Text = "email";
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            LvPwHind.Visibility = Visibility.Collapsed;
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Password == "")
                LvPwHind.Visibility = Visibility.Visible;
        }

        private void LvPwHind_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LvPwHind.Visibility = Visibility.Collapsed;
            tbPassword.Focus();
        }

        Thread th;

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = tbEmail.Text;
            var pw = tbPassword.Password;

            SetPanelEnable(false);

            th = new Thread(new ThreadStart(delegate
            {
                var login = new MLogin();
                var result = login.Authenticate(email, pw);

                Dispatcher.Invoke(new Action(delegate
                {
                    tbPassword.Clear();
                    SetPanelEnable(true);

                    if (result.Result == MLoginResult.Success)
                    {
                        Setting.Json.Email = email;
                        ShowWelcome(result);
                    }
                    else
                    {
                        var errMsg = "";
                        switch (result.Result)
                        {
                            case MLoginResult.WrongAccount:
                                errMsg = "잘못된 계정";
                                break;
                            case MLoginResult.BadRequest:
                            case MLoginResult.UnknownError:
                            default:
                                errMsg = "잘못된 요청. 아래 내용을 캡처해서 런처 개발자에게 문의하세요.";
                                break;
                        }
                        MessageBox.Show("로그인 실패 : " + errMsg);
                    }
                }));
            }));
            th.Start();
        }

        private void SetPanelEnable(bool value)
        {
            tbEmail.IsEnabled = value;
            tbPassword.IsEnabled = value;
            btnLogin.IsEnabled = value;
        }

        private void ShowWelcome(MSession s)
        {
            //윈도우 전환
            var mainWindow = new MainWindow(s);
            mainWindow.RenderSize = this.RenderSize;
            mainWindow.Show();

            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Stop();
            th.Abort();
        }
    }
}
