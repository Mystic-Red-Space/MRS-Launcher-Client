using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRSUpdater
{
    public enum NetFrameworkVersion
    {
        v45  = 378389,
        v451 = 378675,
        v452 = 379893,
        v46  = 393295,
        v461 = 394254,
        v462 = 394802,
        v47  = 460798,
        v471 = 461308,
        v472 = 461808,
        Unknown = 0
    }

    class NetFramework
    {
        public static int GetVersion()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return (int)ndpKey.GetValue("Release");
                }
                else
                {
                    return (int)NetFrameworkVersion.Unknown;
                }
            }
        }

        public const string v461_DownloadLink = "https://www.microsoft.com/ko-KR/download/details.aspx?id=49981";
    }
}
