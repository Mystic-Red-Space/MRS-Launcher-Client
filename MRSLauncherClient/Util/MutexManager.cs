using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    class MutexManager
    {
        const string GUID = "25C1D0A1B1EE4EDA935BE5293B370694";

        public static bool CreateMutex()
        {
            var mutex = new Mutex(true, GUID);

            return mutex.WaitOne(1000);
        }
    }
}
