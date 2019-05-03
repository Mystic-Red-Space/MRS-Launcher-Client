using CmlLib.Launcher;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using log4net;

namespace MRSLauncherClient
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static ILog log = LogManager.GetLogger("LoginWindow");

        public LoginWindow()
        {
            InitializeComponent();
        }

        bool userClose = true;

        private void btnForgotPW_MouseDown(object sender, MouseButtonEventArgs e) // 비밀번호 잊었을때
        {
            Utils.ProcessStart(Launcher.MojangForgotPassword);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            log.Info("Auto Login");

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
            if (tbEmail.Text == tbEmail.Tag.ToString())
                tbEmail.Clear();
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text == "")
                tbEmail.Text = tbEmail.Tag.ToString();
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = tbEmail.Text;
            var pw = tbPassword.Password;

            SetPanelEnable(false);

            log.Info("Start Login Thread");
            var th = new Thread(new ThreadStart(delegate
            {
                var login = new MLogin();
                var result = login.Authenticate(email, pw);

                Dispatcher.Invoke(new Action(delegate
                {
                    tbPassword.Clear();
                    SetPanelEnable(true);

                    if (result.Result == MLoginResult.Success)
                    {
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
                                errMsg = "잘못된 요청";
                                break;
                        }
                        log.Info("Failed login : " + errMsg);
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
            log.Info("Login Success : " + s.Username);
            var mainWindow = new MainWindow(s);
            mainWindow.RenderSize = this.RenderSize;
            mainWindow.Show();

            userClose = false;
            this.Close();
        }

        //창 닫기
        private void Window_Closed(object sender, EventArgs e)
        {
            if (userClose)
                App.Stop();
        }

        //엔터키로 tbEmail -> tbPassword, tbPassword -> BtnLogin 으로 전환함.
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (sender == tbEmail)
                {
                    tbPassword.Focus();
                }
                else if (sender == tbPassword)
                {
                    BtnLogin_Click(sender, e);
                }
            }
        }
    }
}
