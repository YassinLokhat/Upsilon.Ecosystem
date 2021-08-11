using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Common.Library;

namespace Upsilon.Common.Forms
{
    public static class YUpdateCenter
    {
        public static YAssembly CheckForUpdate(string assemblyName, YVersion localVersion, string serverUrl, string message = null, string title = null)
        {
            YAssembly onlineAssembly = YUpdateCentre.CheckForUpdate(serverUrl, assemblyName);

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
                    YStaticMethods.ProcessStartUrl(onlineAssembly.Url);
                }

                Environment.Exit(0);
            }

            return onlineAssembly;
        }
    }
}
