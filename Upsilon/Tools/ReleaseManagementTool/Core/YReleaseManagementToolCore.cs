using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml;
using Upsilon.Common.Library;
using Upsilon.Common.MetaHelper;

namespace Upsilon.Tools.ReleaseManagementTool.Core
{
    public enum Config
    {
        Solutions,
        OpenOutput,
        Dotfuscaor,
        InnoSetup,
        ServerUrl,
    }

    public sealed class YReleaseManagementToolCore
    {
        public YAssembly[] Assemblies { get; private set; } = null;
        public string[] Solutions
        {
            get
            {
                return _solutions.ToArray();
            }
        }

        private string _solution = string.Empty;
        private readonly List<string> _solutions;
        private readonly string _configFile = "config.json";
        public readonly YConfigurationProvider<Config> ConfigProvider;

        public YReleaseManagementToolCore()
        {
            this.ConfigProvider = new(this._configFile);

            try
            {
                this._solutions = this.ConfigProvider.GetConfiguration<List<string>>(Config.Solutions);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            if (this._solutions == null)
            {
                this._solutions = new();
            }

            while (this._solutions.Count != 0
                && !File.Exists(this._solutions.First()))
            {
                this._solutions.RemoveAt(0);
            }

            if (this._solutions.Count != 0)
            {
                this.LoadSolution(this._solutions.First());
            }
        }

        public void LoadSolution(string solution)
        {
            this._solutions.Remove(solution);
            
            if (!File.Exists(solution))
            {
                throw new Exception($"'{solution}' not found.");
            }

            this._solutions.Insert(0, solution);
            this._solution = solution;

            this.ConfigProvider.SetConfiguration(Config.Solutions, this._solutions);

            string[] projects = Directory.GetFiles(YHelper.GetSolutionDirectory(this._solution), "assembly.info", SearchOption.AllDirectories);

            List<YAssembly> assemblies = new();

            foreach (string csproj in projects)
            {
                YAssembly assembly = (YAssembly)File.ReadAllText(csproj).DeserializeObject(typeof(YAssembly));
                assembly.Url = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(csproj)), $"{assembly.Name}.csproj");

                assemblies.Add(assembly);
            }

            this.Assemblies = assemblies.ToArray();
        }

        public YAssembly SelectAssembly(string assemblyName)
        {
            YAssembly assembly = this.Assemblies.Where(x => x.Name == assemblyName).FirstOrDefault();

            if (assembly == null)
            {
                return assembly;
            }
            
            XmlDocument document = new();
            document.Load(assembly.Url);

            assembly.Version = "1.0.0";

            XmlNode version = document.SelectSingleNode("/Project/PropertyGroup/Version");
            if (version != null)
            {
                assembly.Version = version.InnerText;
            }

            assembly.BinaryType = "dll";

            XmlNode binaryType = document.SelectSingleNode("/Project/PropertyGroup/OutputType");
            if (binaryType != null
                && binaryType.InnerText.ToLower().Contains("exe"))
            {
                assembly.BinaryType = "exe";
            }

            List<YDependency> dependencies = new();

            foreach (XmlNode projectReference in document.SelectNodes("/Project/ItemGroup/ProjectReference"))
            {
                YDependency dependency = assembly.Dependencies.Where(x => x.Name == Path.GetFileNameWithoutExtension(projectReference.Attributes["Include"].Value)).FirstOrDefault();

                if (dependency == null)
                {
                    dependency = new()
                    {
                        Name = Path.GetFileNameWithoutExtension(projectReference.Attributes["Include"].Value),
                        MinimalVersion = string.Empty,
                        MaximalVersion = string.Empty,
                    };
                }

                dependencies.Add(dependency);
            }

            assembly.Dependencies = dependencies.ToArray();

            return (YAssembly)assembly.Clone();
        }

        public void Deploy(YAssembly assembly)
        {
            YAssembly asm = this.Assemblies.Where(x => x.Name == assembly.Name).FirstOrDefault();

            if (asm == null)
            {
                return;
            }

            asm.Version = assembly.Version;
            asm.Description = assembly.Description;
            asm.BinaryType = assembly.BinaryType;

            foreach (YDependency dep in assembly.Dependencies)
            {
                YDependency dependency = asm.Dependencies.Where(x => x.Name == dep.Name).FirstOrDefault();

                if (dependency == null)
                {
                    continue;
                }

                dependency.MinimalVersion = dep.MinimalVersion;
                dependency.MaximalVersion = dep.MaximalVersion;
            }

            XmlDocument document = new();
            document.Load(asm.Url);

            XmlNode propertyGroup = document.SelectSingleNode("/Project/PropertyGroup");
            if (propertyGroup == null)
            {
                throw new Exception($"'{asm.Url}' does not contain '/Project/PropertyGroup' node.");
            }

            XmlNode v = propertyGroup.SelectSingleNode("./Version");
            if (v == null)
            {
                v = document.CreateNode(XmlNodeType.Element, "Version", "");
                propertyGroup.AppendChild(v);
            }
            v.InnerText = asm.Version;

            XmlNode bin = propertyGroup.SelectSingleNode("./Description");

            if (bin == null)
            {
                bin = document.CreateNode(XmlNodeType.Element, "Description", "");
                propertyGroup.AppendChild(bin);
            }
            bin.InnerText = asm.Description;

            foreach (YAssembly a in this.Assemblies)
            {
                YDependency dependency = a.Dependencies.Where(x => x.Name == asm.Name).FirstOrDefault();
                if (dependency != null)
                {
                    dependency.MaximalVersion = asm.Version;
                    if (dependency.MinimalVersion == string.Empty)
                    {
                        dependency.MinimalVersion = dependency.MaximalVersion;
                    }
                }
            }

            document.Save(asm.Url);

            this._startDeployProcess(asm);
        }

        public void GenerateAssemblyInfo(string assemblyName = null)
        {
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                YAssembly assembly = this.Assemblies.Where(x => x.Name == assemblyName).FirstOrDefault();
                
                if (assembly == null)
                {
                    return;
                }

                assembly = (YAssembly)assembly.Clone();
                string assemblyInfoPath = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", "assembly.info");
                
                assembly.Url = null;
                string jsonString = JsonSerializer.Serialize(assembly, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(assemblyInfoPath, jsonString);
            }
            else
            {
                foreach (YAssembly assembly in this.Assemblies)
                {
                    GenerateAssemblyInfo(assembly.Name);
                }
            }
        }

        private void _startDeployProcess(YAssembly assembly)
        {
            YReleaseManagementToolCore._startProcess("dotnet", $"clean \"{assembly.Url}\" -c Release");
            YReleaseManagementToolCore._startProcess("dotnet", $"build \"{assembly.Url}\" -c Release");
            YReleaseManagementToolCore._startProcess("dotnet", $"test \"{this._solution}\" -c Release");

            this._checkIntegrity();

            string dotfuscator = this.ConfigProvider.GetConfiguration<string>(Config.Dotfuscaor);

            string dotfuscatorXml = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", $"{assembly.Name}.xml");

            if (!File.Exists(dotfuscatorXml))
            {
                throw new Exception($"'{dotfuscatorXml}' not found.");
            }

            string dotfuscatedDirectory = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", "Dotfuscated");
            if (Directory.Exists(dotfuscatedDirectory))
            {
                Directory.Delete(dotfuscatedDirectory, true);
            }

            YReleaseManagementToolCore._startProcess(dotfuscator, $"\"{dotfuscatorXml}\"");

            string ressourceTemplate = File.ReadAllText("./data/Resources.template.rc")
                .Replace("¤Major¤", assembly.YVersion.Major.ToString())
                .Replace("¤Minor¤", assembly.YVersion.Minor.ToString())
                .Replace("¤Build¤", assembly.YVersion.Build.ToString())
                .Replace("¤Revision¤", assembly.YVersion.Revision.ToString())
                .Replace("¤FileDescription¤", assembly.Description)
                .Replace("¤OriginalFilename¤", assembly.Name + "." + assembly.BinaryType)
                .Replace("¤ProductName¤", assembly.Name);
            File.WriteAllText("./data/Resources.rc", ressourceTemplate);

            YReleaseManagementToolCore._startProcess("\"./data/GoRC.exe\"", $"/fo ./data/Resources.res ./data/Resources.rc");

            string dotfuscatedAssembly = Path.Combine(dotfuscatedDirectory, assembly.Name + "." + assembly.BinaryType);

            if (!File.Exists(dotfuscatedAssembly))
            {
                dotfuscatedAssembly = Path.Combine(dotfuscatedDirectory, assembly.Name + ".dll");
            }

            if (!File.Exists(dotfuscatedAssembly))
            {
                throw new Exception($"'{dotfuscatedAssembly}' not found.");
            }

            YReleaseManagementToolCore._startProcess("\"./data/ResourceHacker.exe\"", $"-open \"{dotfuscatedAssembly}\" -save \"{dotfuscatedAssembly}\" -action addoverwrite -resource ./data/Resources.res");

            YReleaseManagementToolCore._startProcess("\"./data/signtool.exe\"", $"sign /f \"./data/UpsilonEcosystem.pfx\" /p YL-upsilonecosystem-passw0rd \"{dotfuscatedAssembly}\"");

            string fileList = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", $"{assembly.Name}.list.txt");
            if (File.Exists(fileList))
            {
                string[] files = File.ReadAllLines(fileList); 
                foreach (string file in files)
                {
                    string sourceFile = Path.Combine(Path.GetDirectoryName(assembly.Url), file).Trim();
                    string destination = dotfuscatedDirectory;

                    if (file.Contains('\t'))
                    {
                        sourceFile = Path.Combine(Path.GetDirectoryName(assembly.Url), file[0..file.IndexOf('\t')].Trim());
                        destination = Path.Combine(dotfuscatedDirectory, file[file.IndexOf('\t')..].Trim());
                    }

                    YStaticMethods.Copy(sourceFile, destination, true);
                }
            }

            string innoSetup = this.ConfigProvider.GetConfiguration<string>(Config.InnoSetup);

            string innoSetupIss = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", $"{assembly.Name}.iss");

            if (File.Exists(innoSetupIss))
            {
                string setupFile = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy"), "*_setup_*.exe").FirstOrDefault();
                if (File.Exists(setupFile))
                {
                    File.Delete(dotfuscatedDirectory);
                }

                YReleaseManagementToolCore._startProcess(innoSetup, $"\"{innoSetupIss}\"");
            }


            this.GenerateAssemblyInfo();

            this.ComputeAssembliesJson(assembly.Name);

            /*
                Upload Release binary to the Mega repository : https://mega.nz/
                Create Release/{AssemblyName}/{AssemblyVersion} branch from master branch
            */

            File.Delete("./data/Resources.rc");
            File.Delete("./data/Resources.res");

            if (this.ConfigProvider.GetConfiguration<bool>(Config.OpenOutput))
            {
                Process.Start("explorer", $"\"{dotfuscatedDirectory}\"");
            }
        }

        public void ComputeAssembliesJson(string assemblyName)
        {
            YAssembly assembly = this.Assemblies.Where(x => x.Name == assemblyName).FirstOrDefault();

            if (assembly == null)
            {
                return;
            }

            assembly = (YAssembly)assembly.Clone();
            assembly.Url = null;

            string assembliesJsonFile = Path.Combine(YHelper.GetSolutionDirectory(this._solution), "deployed.assemblies.json");
            if (!File.Exists(assembliesJsonFile))
            {
                File.WriteAllText(assembliesJsonFile, "{}");
            }

            Dictionary<string, List<YAssembly>> assemblies = (Dictionary<string, List<YAssembly>>)File.ReadAllText(assembliesJsonFile).DeserializeObject(typeof(Dictionary<string, List<YAssembly>>));

            if (!assemblies.ContainsKey(assembly.Name))
            {
                assemblies[assembly.Name] = new();
            }

            YAssembly asm = assemblies[assembly.Name].Find(x => x.Name == assembly.Name && x.YVersion < assembly.YVersion && x.Depreciated == false);
            
            if (asm != null)
            {
                asm.Depreciated = true;
            }

            asm = assemblies[assembly.Name].Find(x => x.Name == assembly.Name && x.YVersion == assembly.YVersion);

            if (asm == null)
            {
                assemblies[assembly.Name].Insert(0, assembly);
            }

            string jsonString = JsonSerializer.Serialize(assemblies, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(assembliesJsonFile, jsonString);
        }

        private static void _startProcess(string command, string arguments)
        {
            Process process = new();
            process.StartInfo = new()
            {
                FileName = command,
                Arguments = arguments,
            };
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception($"'{command} {arguments}' returns {process.ExitCode}");
            }
        }

        private void _checkIntegrity()
        {
            if (!this.ConfigProvider.HasConfiguration(Config.Dotfuscaor)
                || !File.Exists(this.ConfigProvider.GetConfiguration<string>(Config.Dotfuscaor)))
            {
                throw new Exception($"Dotfuscator : '{this.ConfigProvider.GetConfiguration<string>(Config.Dotfuscaor)}' not found");
            }

            if (!this.ConfigProvider.HasConfiguration(Config.InnoSetup)
                || !File.Exists(this.ConfigProvider.GetConfiguration<string>(Config.InnoSetup)))
            {
                throw new Exception($"InnoSetup : '{this.ConfigProvider.GetConfiguration<string>(Config.InnoSetup)}' not found");
            }

            if (!this.ConfigProvider.HasConfiguration(Config.ServerUrl)
                || string.IsNullOrWhiteSpace(this.ConfigProvider.GetConfiguration<string>(Config.ServerUrl)))
            {
                throw new Exception($"Server URL not set");
            }

            if (!File.Exists("./data/GoRC.exe"))
            {
                throw new Exception("'./data/GoRC.exe' not found");
            }

            if (!File.Exists("./data/ResourceHacker.exe"))
            {
                throw new Exception("'./data/ResourceHacker.exe' not found");
            }

            if (!File.Exists("./data/Resources.template.rc"))
            {
                throw new Exception("'./data/Resources.template.rc' not found");
            }

            if (!File.Exists("./data/signtool.exe"))
            {
                throw new Exception("'./data/signtool.exe' not found");
            }

            if (!File.Exists("./data/UpsilonEcosystem.pfx"))
            {
                throw new Exception("'./data/UpsilonEcosystem.pfx' not found");
            }
        }

        public static void OpenRepository()
        {
            YStaticMethods.ProcessStartUrl("https://mega.nz/");
        }
    }
}
