using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Tools.ReleaseManagementTool.Core;

namespace Upsilon.Tools.ReleaseManagementTool.GUI
{
    static class Program
    {
        private static YReleaseManagementToolCore _core = null;
        public static YReleaseManagementToolCore Core
        {
            get
            {
                if (Program._core == null)
                {
                    Program._core = new();
                }

                return Program._core;
            }
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
