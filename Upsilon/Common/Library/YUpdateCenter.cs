using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public static class YUpdateCentre
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

            return deployedAssemblies;
        }
    }
}
