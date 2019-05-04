using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    class HardDiskManager
    {
        public static string GetDiskId()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
            string serial = "";

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                var c = wmi_HD["SerialNumber"];
                if (c != null)
                {
                    serial = c.ToString();
                    break;
                }
            }

            if (serial == null)
                throw new NotSupportedException();

            return serial;
        }
    }
}
