﻿using CmlLib.Launcher;
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
    /// SettingsPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtRam.Text = ramSlider.Value.ToString();
            Setting.Json.MaxRamMb = (int)ramSlider.Value;
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

    }
}
