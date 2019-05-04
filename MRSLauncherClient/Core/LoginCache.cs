using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using log4net;

namespace MRSLauncherClient.Core
{
    public class LoginCache
    {
        private static ILog log = LogManager.GetLogger("LoginCache");

        public LoginCache()
        {
            CreateKey();
        }

        string key;

        private void CreateKey()
        {
            log.Info("Create Key");
            key = HardDiskManager.GetDiskId() + Environment.UserName;
        }

        public string ReadPassword()
        {
            log.Info("Read Data");

            if (!File.Exists(Launcher.SecurityPath))
                return "";

            var content = File.ReadAllBytes(Launcher.SecurityPath);
            var pw = AES.Decrypt(content, key);
            return Encoding.UTF8.GetString(pw);
        }

        public void SavePassword(string pw)
        {
            log.Info("Save Data");

            var content = AES.Encrypt(Encoding.UTF8.GetBytes(pw), key);
            File.WriteAllBytes(Launcher.SecurityPath, content);
        }

        public static void ClearPassword()
        {
            log.Info("Clear Data");

            if (File.Exists(Launcher.SecurityPath))
                File.Delete(Launcher.SecurityPath);
        }
    }
}
