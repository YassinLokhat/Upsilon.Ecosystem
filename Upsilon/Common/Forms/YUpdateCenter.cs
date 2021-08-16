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
    public static class YUpdateCenter
    {
        public static YAssembly CheckForUpdate(string serverUrl, string message = null, string title = null)
        {
            Assembly local = Assembly.GetCallingAssembly();

            string assemblyName = local.GetName().Name;

            YVersion localVersion = new(local.GetName().Version);
            Dictionary<string, List<YAssembly>> deployedAssemblies = YUpdateCentre.CheckForUpdate(serverUrl, assemblyName, out YAssembly onlineAssembly);

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
                    YUpdateCenter._downloadUpdate(onlineAssembly, deployedAssemblies, local.Location);
                }

                Environment.Exit(0);
            }

            return onlineAssembly;
        }

        private static void _downloadUpdate(YAssembly onlineAssembly, Dictionary<string, List<YAssembly>> deployedAssemblies, string outputPath)
        {
            string tempDir = Path.Combine(Assembly.GetExecutingAssembly().Location, DateTime.Now.Ticks.ToString());

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);

            onlineAssembly.DownloadAssembly(deployedAssemblies, tempDir);
            string[] files = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);

            string args = "timeout 5"
                + " & move -Y"
                + $" {string.Join(',', files.Select(x => $"\"{x}\""))}"
                + $" {outputPath}";

            ProcessStartInfo processStartInfo = new()
            {
                FileName = "cmd",
                Arguments = args,
                CreateNoWindow = true,
            };

            Process.Start(processStartInfo);
        }
    }
}
