using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// This static class contains update functions.
    /// </summary>
    public static class YUpdateCenter
    {
        /// <summary>
        /// <para>Check if an update is available for the given <c><paramref name="assemblyName"/></c> assembly from the <c><paramref name="configUrl"/></c> source.</para>
        /// <para>In that case, returns that new version as a <c><see cref="YVersion"/></c>.</para>
        /// </summary>
        /// <remarks>
        /// <para>The <c><paramref name="configUrl"/></c> source should be deserializable into a <c>List&lt;<see cref="YAssembly"/>></c> by using the <c><see cref="YStaticMethods"/>.<see cref="YStaticMethods.DeserializeObject(string, Type)"/></c> method.</para>
        /// <para>Only the following fields are mandataory : <c>Name</c>, <c>Version</c> and <c>Depreciated</c>.</para>
        /// </remarks>
        /// <param name="configUrl">The url of the source.</param>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="assembly">The lastest <c><see cref="YAssembly"/></c> or <c>null</c> if that assembly is missing in the source.</param>
        /// <returns>Returns the list of <c><see cref="YAssembly"/></c> deployed on the server or <c>null</c>.</returns>
        public static Dictionary<string, List<YAssembly>> CheckForUpdate(string configUrl, string assemblyName, out YAssembly assembly)
        {
            YDebugTrace.TraceOn(new object[] { configUrl, assemblyName });

            assembly = null;

            Dictionary<string, List<YAssembly>> deployedAssemblies = null;

            try
            {
                string json = YStaticMethods.DownloadString(configUrl);

                try
                {
                    deployedAssemblies = JsonSerializer.Deserialize<Dictionary<string, List<YAssembly>>>(json);
                }
                catch { }

                List<YAssembly> assemblies = null;
                if (deployedAssemblies != null)
                {
                    assemblies = deployedAssemblies[assemblyName];
                }
                else
                { 
                    assemblies = JsonSerializer.Deserialize<List<YAssembly>>(json)
                        .Where(x => x.Name == assemblyName)
                        .ToList();
                }

                assembly = assemblies.OrderByDescending(x => x.YVersion).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            if (assembly != null
                && assembly.Depreciated)
            {
                throw new Exception();
            }

            return YDebugTrace.TraceOff(deployedAssemblies);
        }

        /// <summary>
        /// Download and update the given assembly.
        /// </summary>
        /// <param name="onlineAssembly">The new version of the assembly.</param>
        /// <param name="deployedAssemblies">The deployed assemblies list.</param>
        /// <param name="workingDirectory">The directory where the assembly to update binaries are. By default it will be the location of the calling assembly.</param>
        public static void DownloadUpdate(YAssembly onlineAssembly, Dictionary<string, List<YAssembly>> deployedAssemblies, string workingDirectory = null)
        {
            YDebugTrace.TraceOn(new object[] { onlineAssembly, deployedAssemblies, workingDirectory });

            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                workingDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            }

            string tempDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DateTime.Now.Ticks.ToString());

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);

            onlineAssembly.DownloadAssembly(deployedAssemblies, tempDir);
            var newFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
            var oldFiles = newFiles.Select(x => x.Replace(tempDir, workingDirectory)).ToArray();

            string args = "/C timeout 4";

            string delCommand = "";
            string moveCommand = "";
            for (int i = 0; i < newFiles.Length; i++)
            {
                delCommand += $" & del /F /Q \"{oldFiles[i]}\"";
                moveCommand += $" & move /Y \"{newFiles[i]}\" \"{oldFiles[i]}\"";
            }

            args += delCommand
                + moveCommand
                + $" & rmdir \"{tempDir}\" /S /Q";

            if (onlineAssembly.BinaryType != YBinaryType.ClassLibrary)
            {
                args += $" & \"{Path.Combine(workingDirectory, onlineAssembly.Name + ".exe")}\"";
            }

            ProcessStartInfo processStartInfo = new()
            {
                FileName = "cmd",
                Arguments = args,
                CreateNoWindow = true,
            };

            Process.Start(processStartInfo);

            YDebugTrace.TraceOff();
            Environment.Exit(0);
        }
    }
}
