using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// All types of binaries.
    /// </summary>
    public enum YBinaryType
    {
        /// <summary>
        /// The assembly is a dll class library.
        /// </summary>
        ClassLibrary = 0,

        /// <summary>
        /// The assembly is a window application.
        /// </summary>
        WindowApplication = 1,

        /// <summary>
        /// The assembly is a console application.
        /// </summary>
        ConsoleApplication = 2,
    }

    /// <summary>
    /// This class represents an assembly.
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
        /// The description of the assembly.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of binary of the assembly.
        /// </summary>
        public YBinaryType BinaryType { get; set; }

        /// <summary>
        /// The url to the assembly.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Url { get; set; }

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
        /// The list of required files of the assembly.
        /// </summary>
        public string[] RequiredFiles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Get the assembly's version as a <c><see cref="YVersion"/></c>.
        /// </summary>
        [JsonIgnore]
        public YVersion YVersion { get { return new(this.Version); } }

        /// <summary>
        /// Create the runtimeconfig.json file for an assembly.
        /// </summary>
        /// <param name="binaryType">The binary type of the assembly.</param>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="fileLocation">The location of the runtimeconfig.json file.</param>
        public static void CreateRuntimeConfigJson(YBinaryType binaryType, string assemblyName, string fileLocation)
        {
            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            fileLocation = Path.Combine(fileLocation, $"{assemblyName}.runtimeconfig.json");

            switch (binaryType)
            {
                case YBinaryType.ClassLibrary:
                    return;
                case YBinaryType.WindowApplication:
                    File.WriteAllText(fileLocation, "{ \"runtimeOptions\": { \"tfm\": \"net5.0\", \"framework\": { \"name\": \"Microsoft.WindowsDesktop.App\", \"version\": \"5.0.0\" } } }");
                    return;
                case YBinaryType.ConsoleApplication:
                    return;
            }
        }

        /// <summary>
        /// Download all dependecies of the current assembly.
        /// </summary>
        /// <param name="deployedAssemblies">The list of dependencies on the server.</param>
        /// <param name="outputPath">The path where the dependecies will be downloaded.</param>
        /// <param name="downloadedDependencies">The list of dependecies already downloaded. By default this parameter is <c>null</c>.</param>
        public void DownloadDependecies(Dictionary<string, List<YAssembly>> deployedAssemblies, string outputPath, List<YAssembly> downloadedDependencies = null)
        {
            if (deployedAssemblies == null)
            {
                deployedAssemblies = new();
            }

            if (downloadedDependencies == null)
            {
                downloadedDependencies = new();
            }

            foreach (YDependency dep in this.Dependencies)
            {
                if (!deployedAssemblies.ContainsKey(dep.Name))
                {
                    throw new Exception($"'{dep.Name}' assembly not found in the deployed assemblies list.");
                }

                YAssembly dependency = deployedAssemblies[dep.Name].Find(x => x.Version == dep.MaximalVersion);

                if (dependency == null)
                {
                    throw new Exception($"'{dep.Name}' assembly found in the deployed assemblies list but no version deployed.");
                }

                dependency.DownloadDependecies(deployedAssemblies, outputPath, downloadedDependencies);

                if (!downloadedDependencies.Any(x => x.Name == dependency.Name && x.YVersion > dependency.YVersion))
                {
                    string[] urls = new[] { dependency.Url };
                    urls = urls.Union(dependency.RequiredFiles).ToArray();
                    string rootPath = dependency.Url[0..dependency.Url.LastIndexOf('/')];

                    YAssembly.CreateRuntimeConfigJson(dependency.BinaryType, dependency.Name, outputPath);

                    foreach (string url in urls)
                    {
                        if (url.Contains($"{dependency.Name}_setup_v" + dependency.Version + ".exe"))
                        {
                            continue;
                        }

                        string filePath = Path.GetDirectoryName(url.Replace(rootPath, outputPath));
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        filePath = Path.Combine(filePath, Path.GetFileName(url));
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        
                        YStaticMethods.DownloadFile(url, filePath);
                    }

                    downloadedDependencies.Add(dependency);
                }
            }
        }

        /// <summary>
        /// Download the assembly with its all dependencies.
        /// </summary>
        /// <param name="deployedAssemblies">The list of dependencies on the server.</param>
        /// <param name="outputPath">The path where the dependecies will be downloaded.</param>
        public void DownloadAssembly(Dictionary<string, List<YAssembly>> deployedAssemblies, string outputPath)
        {
            string[] urls = new[] { this.Url };
            urls = urls.Union(this.RequiredFiles).ToArray();
            string rootPath = this.Url[0..this.Url.LastIndexOf('/')];

            YAssembly.CreateRuntimeConfigJson(this.BinaryType, this.Name, outputPath);

            foreach (string url in urls)
            {
                if (url.Contains($"{this.Name}_setup_v" + this.Version + ".exe"))
                {
                    continue;
                }

                string filePath = Path.GetDirectoryName(url.Replace(rootPath, outputPath));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath = Path.Combine(filePath, Path.GetFileName(url));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                YStaticMethods.DownloadFile(url, filePath);
            }

            this.DownloadDependecies(deployedAssemblies, outputPath);
        }
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
        [JsonIgnore]
        public YVersion YMinimalVersion { get { return new(this.MinimalVersion); } }

        /// <summary>
        /// Get the maximal version needed by the using assembly as a <c><see cref="YVersion"/></c>.
        /// </summary>
        [JsonIgnore]
        public YVersion YMaximalVersion { get { return new(this.MaximalVersion); } }
    }
}
