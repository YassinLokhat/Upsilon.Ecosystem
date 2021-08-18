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
        public static YAssembly CheckForUpdate(string serverUrl, string assemblyName, YVersion localVersion, string message = null, string title = null)
        {
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
                    YUpdateCenter._downloadUpdate(onlineAssembly, deployedAssemblies, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
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

            return YUpdateCenter.CheckForUpdate(serverUrl, assemblyName, localVersion, message, title);
        }

        private static void _downloadUpdate(YAssembly onlineAssembly, Dictionary<string, List<YAssembly>> deployedAssemblies, string outputPath)
        {
            string tempDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DateTime.Now.Ticks.ToString());

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);

            onlineAssembly.DownloadAssembly(deployedAssemblies, tempDir);
            var newFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
            var oldFiles = newFiles.Select(x => x.Replace(tempDir, outputPath)).ToArray();

            string args = "/C timeout 5"
                + " & copy -Y"
                + $" {string.Join(',', newFiles.Select(x => $"\"{x}\""))}"
                + $" \"{outputPath}\""
                + $" & rmdir \"{tempDir}\" /S /Q";

            args = "/C timeout 4";

            string delCommand = "";
            string moveCommand = "";
            for (int i = 0; i < newFiles.Length; i++)
            {
                delCommand += $" & del /F /Q \"{oldFiles[i]}\"";
                moveCommand += $" & move /Y \"{newFiles[i]}\" \"{oldFiles[i]}\"";
            }

            args += delCommand 
                + moveCommand
                + $" \"{outputPath}\""
                + $" & rmdir \"{tempDir}\" /S /Q";

            if (onlineAssembly.BinaryType != YBinaryType.ClassLibrary)
            {
                args += $" & \"{Path.Combine(outputPath, onlineAssembly.Name + ".exe")}\"";
            }

            ProcessStartInfo processStartInfo = new()
            {
                FileName = "cmd",
                Arguments = args,
                CreateNoWindow = true,
            };

            Process.Start(processStartInfo);
            Environment.Exit(0);
        }
    }
}
