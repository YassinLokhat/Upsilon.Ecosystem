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
    /// This class represent an assembly.
    /// </summary>
    public sealed class YAssembly
    {
        /// <summary>
        /// The name of the assembly.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the assembly.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Get or Set if the assembly is depreciated.
        /// </summary>
        public bool Depreciated { get; set; }

        /// <summary>
        /// The assembly's dependencies.
        /// <seealso cref="YDependency"/>
        /// </summary>
        public YDependency[] Dependencies { get; set; }

        /// <summary>
        /// Get the assembly's version as a <c><see cref="YVersion"/></c>.
        /// </summary>
        public YVersion YVersion { get { return new(this.Version); } }
    }

    /// <summary>
    /// This class represents an assembly used by another assembly.
    /// </summary>
    public sealed class YDependency
    {
        /// <summary>
        /// The name of the assembly.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimal version needed by the using assembly.
        /// </summary>
        public string MinimalVersion { get; set; }

        /// <summary>
        /// The maximal version needed by the using assembly.
        /// </summary>
        public string MaximalVersion { get; set; }

        /// <summary>
        /// Get the minimal version needed by the using assembly as a <c><see cref="YVersion"/></c>.
        /// </summary>
        public YVersion YMinimalVersion { get { return new(this.MinimalVersion); } }

        /// <summary>
        /// Get the maximal version needed by the using assembly as a <c><see cref="YVersion"/></c>.
        /// </summary>
        public YVersion YMaximalVersion { get { return new(this.MaximalVersion); } }
    }

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
        /// <returns>Returns the last <c><see cref="YVersion"/></c> of that assembly or <c>null</c> if that assembly is missing in the source.</returns>
        public static YVersion CheckForUpdate(string configUrl, string assemblyName)
        {
            YVersion version = null;
            YAssembly assembly = null;
            
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                ServicePointManager.DefaultConnectionLimit = 9999;

                WebClient webClient = new WebClient();

                string json = webClient.DownloadString(configUrl);
                
                List<YAssembly> assemblies = JsonSerializer.Deserialize<List<YAssembly>>(json)
                    .Where(x => x.Name == assemblyName)
                    .ToList();

                if (assemblies.Count != 0)
                {
                    version = assemblies.Select(x => x.YVersion).Max();
                    assembly = assemblies.Find(x => x.YVersion == version);
                }
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

            return version;
        }
    }
}
