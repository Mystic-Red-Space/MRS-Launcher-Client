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
        public SettingsPage()
        {
            InitializeComponent();
            ramSlider.Maximum = userPc.TotalPhysicalMemory/(1024*1024);
            ramSlider.Minimum = 2048;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int ramValue = (int)ramSlider.Value;
            txtRam.Text = ramValue.ToString();
            lvRamViewer.Content = (txtRam.Text+"MB / "+ramSlider.Maximum+"MB");
        }
        
        private void TxtRam_GotFocus(object sender, RoutedEventArgs e)
        {
            txtRam.Clear();
        }

        private void TxtRam_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (int.Parse(txtRam.Text) > ramSlider.Maximum || int.Parse(txtRam.Text) < ramSlider.Minimum)
                {
                    txtRam.Text = ramSlider.Maximum.ToString();
                    TxtRam_LostFocus(sender,e);
                    return;
                }
                else{
                    ramSlider.Value = int.Parse(txtRam.Text);
                    lvRamViewer.Content = (ramSlider.Value + "MB / " + ramSlider.Maximum + "MB");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{txtRam.Text}'");
            }
        }

        private void TxtRam_LostFocus(object sender, RoutedEventArgs e)
        {
            int ramValue = int.Parse(txtRam.Text);
            txtRam.Text = ramValue.ToString();
            lvRamViewer.Content = (txtRam.Text + "MB / " + ramSlider.Maximum + "MB");
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            txtRam.Text = Setting.Json.MaxRamMb.ToString();
            rtJavaArgs.Document = new FlowDocument(new Paragraph(new Run(Setting.Json.CustomJVMArguments))); // richtextbox 사용법이 이상해짐
            cbCustomJVM.IsChecked = Setting.Json.UseCustomJVM;

        }

        private void Settings_Unloaded(object sender, RoutedEventArgs e)
        {
            var ram = 0;
            if (int.TryParse(txtRam.Text, out ram))
                Setting.Json.MaxRamMb = ram;

            Setting.Json.CustomJVMArguments = new TextRange(rtJavaArgs.Document.ContentStart, rtJavaArgs.Document.ContentEnd).Text;
            Setting.Json.UseCustomJVM = cbCustomJVM.IsChecked ?? false; // 만약 값이 null 이라면 false 를 반환
        }

        private void CbCustomJVM_Checked(object sender, RoutedEventArgs e)
        {
            cbCustomJVM.Content = "enabled";
        }
        private void CbCustomJVM_Unchecked(object sender, RoutedEventArgs e)
        {
            cbCustomJVM.Content = "disabled";
        }

    }
}
