using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;

namespace MRSUpdater
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            EmbeddedAssembly.Load("MRSUpdater._Newtonsoft.Json.dll", "_Newtonsoft.Json.dll");

            AppDomain.CurrentDomain.AssemblyResolve
                += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            System.Threading.Thread.Sleep(1000);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
