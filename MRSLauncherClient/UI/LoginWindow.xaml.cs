using CmlLib.Launcher;
using log4net;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MRSLauncherClient.Core;

namespace MRSLauncherClient
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static ILog log = LogManager.GetLogger("LoginWindow");
        private LoginCache loginCache;
        bool isUserCheckSavePw = false;

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
            loginCache = new LoginCache();

            log.Info("Auto Login");

            isUserCheckSavePw = true;
            cbSavePw.IsChecked = Setting.Json.SavePassword;

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
                        ShowWelcome(result, false);
                    }
                    else
                    {
                        tbEmail.Text = Setting.Json.Email;

                        if (Setting.Json.SavePassword)
                            Auth(Setting.Json.Email, loginCache.ReadPassword(), false);
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

            Auth(email, pw, true);
        }

        private void Auth(string email, string pw, bool savePw)
        {
            if (email == "" || pw == "")
                return;

            SetPanelEnable(false);

            log.Info("Start Login Thread");
            var th = new Thread(new ThreadStart(delegate
            {
                var login = new MLogin();
                var result = login.Authenticate(email, pw);

                Dispatcher.Invoke(new Action(delegate
                {
                    SetPanelEnable(true);

                    if (result.Result == MLoginResult.Success)
                    {
                        ShowWelcome(result, savePw);
                    }
                    else
                    {
                        tbPassword.Clear();

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

        private void ShowWelcome(MSession s, bool savePw)
        {
            Setting.Json.Email = tbEmail.Text;
            if (savePw && Setting.Json.SavePassword)
                loginCache.SavePassword(tbPassword.Password);

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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (isUserCheckSavePw)
            {
                isUserCheckSavePw = false;
                return;
            }

            var r = MessageBox.Show(
                "이 기능을 사용할 시 사용자의 비밀번호가 암호화되어 컴퓨터에 저장됩니다. 해킹 위험이 있습니다. \n" +
                "이 기능을 사용하지 않아도 로그인 세션이 저장되어 안전하게 자동 로그인 기능을 사용할 수 있습니다.",
                "경고",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (r == MessageBoxResult.Yes)
                Setting.Json.SavePassword = true;
            else
                cbSavePw.IsChecked = false;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Setting.Json.SavePassword = false;
        }
    }
}
