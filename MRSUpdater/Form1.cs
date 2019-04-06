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
using System.IO;

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

        readonly string RootPath = Environment.CurrentDirectory + "\\";
        const string LauncherFile = "MRSLauncherClient.exe";

        string[] notUpdate = new string[]
        {
            "updater.exe",
            "version.dat"
        };

        void Start()
        {
            try
            {
                var frameworkVersion = NetFramework.GetVersion();
                if (frameworkVersion < (int)NetFrameworkVersion.v461)
                {
                    MessageBox.Show("닷넷 프레임워크 4.6.1을 설치해 주세요.");
                    startProcess(NetFramework.v461_DownloadLink);
                    Environment.Exit(0);
                }

                Directory.CreateDirectory(RootPath);
                var updater = new Updater(RootPath);

                if (updater.CheckHasNewVersion())
                {
                    var updateFiles = UpdateLoader.GetUpdateFiles();
                    var localFiles = new LocalFileManager(RootPath, notUpdate).GetLocalFiles();

                    updater.FileChanged += Updater_FileChanged;
                    updater.ProgressChanged += Updater_ProgressChanged;
                    updater.Update(localFiles, updateFiles);

                    foreach (var item in localFiles)
                    {
                        File.Delete(item.Key);
                    }
                }

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

        private void startProcess(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch
            {

            }
        }
    }
}
