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
    public class YAssembly
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public bool Depreciated { get; set; }
        public YDependency[] Dependencies { get; set; }
        public YVersion YVersion { get { return new(this.Version); } }
    }

    public class YDependency
    {
        public string Name { get; set; }
        public string MinimalVersion { get; set; }
        public string MaximalVersion { get; set; }
        public YVersion YMinimalVersion { get { return new(this.MinimalVersion); } }
        public YVersion YMaximalVersion { get { return new(this.MaximalVersion); } }
    }

    public static class YUpdateCentre
    {
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
