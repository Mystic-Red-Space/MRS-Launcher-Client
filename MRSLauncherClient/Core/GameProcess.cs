﻿using System;
using System.Diagnostics;
using log4net;

namespace MRSLauncherClient
{
    public class GameProcess // 게임 프로세스와 연동해 디스코드에 뭘표시하던가 등등등 나중에 쓰일수도
    {
        private static ILog log = LogManager.GetLogger("GameProcess");

        public GameProcess(Process p)
        {
            process = p;
        }

        public event EventHandler<string> GameOutput;
        Process process; // 마인크래프트 프로세스

        public void Start()
        {
            log.Info("Start Minecraft");

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            output(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            output(e.Data);
        }

        void output(string msg)
        {
            GameOutput?.Invoke(this, msg);
        }
    }
}
