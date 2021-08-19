using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Common.Library;

namespace Upsilon.Common.Forms
{
    public static class YUpdateForms
    {
        public static YAssembly CheckForUpdate(string serverUrl, string assemblyName, YVersion localVersion, string message = null, string title = null)
        {
            Dictionary<string, List<YAssembly>> deployedAssemblies = YUpdateCenter.CheckForUpdate(serverUrl, assemblyName, out YAssembly onlineAssembly);

            if (String.IsNullOrWhiteSpace(message))
            {
                message = $"A new version of {assemblyName} is available : '{onlineAssembly.Version}'.\nPlease update the software since the version '{localVersion}' is depreciated.";
            }

            if (String.IsNullOrWhiteSpace(title))
            {
                title = "New version";
            }

            if (onlineAssembly != null
                && onlineAssembly.YVersion > localVersion)
            {
                if (!string.IsNullOrWhiteSpace(onlineAssembly.Url)
                    && MessageBox.Show(message,
                    title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    YUpdateCenter.DownloadUpdate(onlineAssembly, deployedAssemblies);
                }

                Environment.Exit(0);
            }

            return onlineAssembly;
        }

        public static YAssembly CheckForUpdate(string serverUrl, string message = null, string title = null)
        {
            Assembly local = Assembly.GetCallingAssembly();

            string assemblyName = local.GetName().Name;

            YVersion localVersion = new(local.GetName().Version);

            return YUpdateForms.CheckForUpdate(serverUrl, assemblyName, localVersion, message, title);
        }
    }
}
