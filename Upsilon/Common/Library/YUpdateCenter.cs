﻿using System;
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

                WebClient webClient = new();

                string json = webClient.DownloadString(configUrl);

                List<YAssembly> assemblies = ((List<YAssembly>)json.DeserializeObject(typeof(List<YAssembly>)))
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