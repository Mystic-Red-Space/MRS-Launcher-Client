using CmlLib.Launcher;
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
using Microsoft.VisualBasic.Devices;

namespace MRSLauncherClient
{
    /// <summary>
    /// SettingsPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsPage : Page
    {
        ComputerInfo userPc = new ComputerInfo();
        int maxRam, minRam;

        public SettingsPage()
        {
            InitializeComponent();

            maxRam = (int)(userPc.TotalPhysicalMemory / (1024 * 1024));
            minRam = 2048;

            ramSlider.Maximum = maxRam;
            ramSlider.Minimum = minRam;

            txtRam.Maximum = maxRam;
            txtRam.Minimum = minRam;
            txtRam.Interval = 256;
            txtRam.ValueChanged += TxtRam_ValueChanged;

            lvMin.Content = minRam.ToString();
            lvMax.Content = maxRam.ToString();
        }

        private void TxtRam_ValueChanged(object sender, EventArgs e)
        {
            ramSlider.Value = txtRam.Value;
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            ramSlider.Focus();
            ramSlider.Value = Setting.Json.MaxRamMb;
            rtJavaArgs.Document = new FlowDocument(new Paragraph(new Run(Setting.Json.CustomJVMArguments))); // richtextbox 사용법이 이상해짐
            rtJavaArgs.IsEnabled = Setting.Json.UseCustomJVM;
            cbCustomJVM.IsChecked = Setting.Json.UseCustomJVM;
            cbShowLogWindow.IsChecked = Setting.Json.ShowLogWindow;
            cbHideLauncher.IsChecked = Setting.Json.HideLauncher;
        }

        private void Settings_Unloaded(object sender, RoutedEventArgs e)
        {
            Setting.Json.MaxRamMb = txtRam.Value;
            Setting.Json.CustomJVMArguments = new TextRange(rtJavaArgs.Document.ContentStart, rtJavaArgs.Document.ContentEnd).Text;
            Setting.Json.UseCustomJVM = cbCustomJVM.IsChecked ?? false; // 만약 값이 null 이라면 false 를 반환
            Setting.Json.ShowLogWindow = cbShowLogWindow.IsChecked ?? false;
            Setting.Json.HideLauncher = cbHideLauncher.IsChecked ?? false;
        }

        private void RamSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ramSlider.IsFocused)
                txtRam.Value = (int)ramSlider.Value;
        }

        private void CbCustomJVM_Checked(object sender, RoutedEventArgs e)
        {
            rtJavaArgs.IsEnabled = true;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }

        private void CbCustomJVM_Unchecked(object sender, RoutedEventArgs e)
        {
            rtJavaArgs.IsEnabled = false;
        }
    }
}
