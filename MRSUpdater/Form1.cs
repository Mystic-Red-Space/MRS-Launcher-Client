using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace MRSUpdater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var th = new Thread(Start);
            th.Start();
        }

        const string RootPath = "";
        const string LauncherFile = "launcher.exe";

        void Start()
        {
            try
            {
                var files = UpdateLoader.GetUpdateFiles();

                var updater = new Updater(RootPath);
                updater.FileChanged += Updater_FileChanged;
                updater.ProgressChanged += Updater_ProgressChanged;
                updater.Update(files);

                Process.Start(RootPath + LauncherFile);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("업데이트 실패\n" + ex.ToString());
                Environment.Exit(0);
            }
        }

        private void Updater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(delegate
            {
                progressBar1.Value = e.ProgressPercentage;
            }));
        }

        private void Updater_FileChanged(object sender, FileChangeEventArgs e)
        {
            Invoke(new Action(delegate
            {
                lvCount.Text = $"{e.NowCount} / {e.MaxCount}";
            }));
        }
    }
}
