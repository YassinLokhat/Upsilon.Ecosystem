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
        /// <param name="configFile">The configuration file which contains the server url list.</param>
        /// <param name="configKey">The key of the configuration file.</param>
        /// <param name="assemblyName">The name of the assembly to update.</param>
        /// <param name="localVersion">The current version of the assembly.</param>
        /// <param name="message">The message to show when new version is available.</param>
        /// <param name="title">The title of the message box.</param>
        /// <returns>Return the lastest <c><see cref="YAssembly"/></c>.</returns>
        public static YAssembly CheckForUpdate(string configFile, string configKey, string assemblyName, YVersion localVersion, string message = null, string title = null)
        {
            Dictionary<string, List<YAssembly>> deployedAssemblies = YUpdateCenter.CheckForUpdate(configFile, configKey, assemblyName, out YAssembly onlineAssembly);

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

            return onlineAssembly;
        }

        /// <summary>
        /// Download and update the calling assembly if available.
        /// </summary>
        /// <param name="configFile">The configuration file which contains the server url list.</param>
        /// <param name="configKey">The key of the configuration file.</param>
        /// <param name="message">The message to show when new version is available.</param>
        /// <param name="title">The title of the message box.</param>
        /// <returns>Return the lastest <c><see cref="YAssembly"/></c>.</returns>
        public static YAssembly CheckForUpdate(string configFile, string configKey, string message = null, string title = null)
        {
            Assembly local = Assembly.GetCallingAssembly();

            string assemblyName = local.GetName().Name;

            YVersion localVersion = new(local.GetName().Version);

            return YUpdateForms.CheckForUpdate(configFile, configKey, assemblyName, localVersion, message, title);
        }
    }
}
