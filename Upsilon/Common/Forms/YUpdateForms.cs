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
    /// <summary>
    /// A static class giving some updating methods.
    /// </summary>
    public static class YUpdateForms
    {
        /// <summary>
        /// Download and update the given assembly if available.
        /// </summary>
        /// <param name="serverUrl">The deployed assemblies json url.</param>
        /// <param name="assemblyName">The name of the assembly to update.</param>
        /// <param name="localVersion">The current version of the assembly.</param>
        /// <param name="message">The message to show when new version is available.</param>
        /// <param name="title">The title of the message box.</param>
        /// <returns>Return the lastest <c><see cref="YAssembly"/></c>.</returns>
        public static YAssembly CheckForUpdate(string serverUrl, string assemblyName, YVersion localVersion, string message = null, string title = null)
        {
            YDebugTrace.TraceOn(new object[] { serverUrl, assemblyName, localVersion, message, title });

            Dictionary<string, List<YAssembly>> deployedAssemblies = YUpdateCenter.CheckForUpdate(serverUrl, assemblyName, out YAssembly onlineAssembly);

            if (onlineAssembly != null
                && onlineAssembly.YVersion > localVersion)
            {
                if (String.IsNullOrWhiteSpace(message))
                {
                    message = $"A new version of {assemblyName} is available : '{onlineAssembly.Version}'.\nPlease update the software since the version '{localVersion}' is depreciated.";
                }

                if (String.IsNullOrWhiteSpace(title))
                {
                    title = "New version";
                }

                if (!string.IsNullOrWhiteSpace(onlineAssembly.Url)
                    && MessageBox.Show(message,
                    title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    YUpdateCenter.DownloadUpdate(onlineAssembly, deployedAssemblies);
                }

                Environment.Exit(0);
            }

            return YDebugTrace.TraceOff(onlineAssembly);
        }

        /// <summary>
        /// Download and update the calling assembly if available.
        /// </summary>
        /// <param name="serverUrl">The deployed assemblies json url.</param>
        /// <param name="message">The message to show when new version is available.</param>
        /// <param name="title">The title of the message box.</param>
        /// <returns>Return the lastest <c><see cref="YAssembly"/></c>.</returns>
        public static YAssembly CheckForUpdate(string serverUrl, string message = null, string title = null)
        {
            YDebugTrace.TraceOn(new object[] { serverUrl, message, title });

            Assembly local = Assembly.GetCallingAssembly();

            string assemblyName = local.GetName().Name;

            YVersion localVersion = new(local.GetName().Version);

            return YDebugTrace.TraceOff(YUpdateForms.CheckForUpdate(serverUrl, assemblyName, localVersion, message, title));
        }
    }
}
