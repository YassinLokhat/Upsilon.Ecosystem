using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        DeployedAssemblies,
        UploadTool,
    }

    public sealed class YReleaseManagementToolCore
    {
        public YAssembly[] SolutionAssemblies { get; private set; } = null;
        public Dictionary<string, List<YAssembly>> DeployedAssemblies
        {
            get
            {
                try
                {
                    string json = YStaticMethods.DownloadString(this.ConfigProvider.GetConfiguration<string>(Config.DeployedAssemblies));
                    return JsonSerializer.Deserialize<Dictionary<string, List<YAssembly>>>(json);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return new();
            }
        }
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
                this.ConfigProvider.SetConfiguration(Config.Solutions, this._solutions);
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

            this.SolutionAssemblies = assemblies.ToArray();
        }

        public YAssembly SelectAssembly(string assemblyName)
        {
            YAssembly assembly = this.SolutionAssemblies.Find(x => x.Name == assemblyName);

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

            assembly.BinaryType = YBinaryType.ClassLibrary;

            XmlNode binaryType = document.SelectSingleNode("/Project/PropertyGroup/OutputType");
            if (binaryType != null)
            {
                if (binaryType.InnerText == "WinExe")
                {
                    assembly.BinaryType = YBinaryType.WindowApplication;
                }
                else
                {
                    assembly.BinaryType = YBinaryType.ConsoleApplication;
                }
            }

            List<YDependency> dependencies = new();

            foreach (XmlNode reference in document.SelectNodes("/Project/ItemGroup/ProjectReference|/Project/ItemGroup/Reference"))
            {
                string depName = reference.Attributes["Include"].Value;

                if (depName.EndsWith(".csproj"))
                {
                    depName = Path.GetFileNameWithoutExtension(depName);
                }

                YDependency dependency = assembly.Dependencies.Find(x => x.Name == depName);

                if (dependency == null
                    && this.DeployedAssemblies.ContainsKey(depName))
                {
                    dependency = new()
                    {
                        Name = depName,
                        MinimalVersion = string.Empty,
                        MaximalVersion = string.Empty,
                    };
                }

                if (dependency == null)
                {
                    continue;
                }

                dependencies.Add(dependency);
            }

            assembly.Dependencies = dependencies.ToArray();

            return assembly.Clone();
        }

        public void Deploy(YAssembly assembly)
        {
            YAssembly asm = this.SolutionAssemblies.Find(x => x.Name == assembly.Name);

            if (asm == null)
            {
                return;
            }

            asm.Version = assembly.Version;
            asm.Description = assembly.Description;
            asm.BinaryType = assembly.BinaryType;
            asm.RequiredFiles = assembly.RequiredFiles.ToArray();

            foreach (YDependency dep in assembly.Dependencies)
            {
                YDependency dependency = asm.Dependencies.Find(x => x.Name == dep.Name);

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

            foreach (YAssembly a in this.SolutionAssemblies)
            {
                YDependency dependency = a.Dependencies.Find(x => x.Name == asm.Name);
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

        private void _startDeployProcess(YAssembly assembly)
        {
            this._build(assembly);

            this.CheckIntegrity();

            string dotfuscatedDirectory = this._dotfuscate(assembly);

            this._sign(assembly, dotfuscatedDirectory);

            assembly.DownloadDependecies(this.DeployedAssemblies, dotfuscatedDirectory);

            this._copyRequiredFiles(assembly, dotfuscatedDirectory);

            this._innoSetup(assembly, dotfuscatedDirectory);

            this._generateAssemblyInfo();

            this.ComputeDeployedAssembliesJson(dotfuscatedDirectory, assembly);

            this._clearTempFiles();

            if (this.ConfigProvider.GetConfiguration<bool>(Config.OpenOutput))
            {
                Process.Start("explorer", $"\"{dotfuscatedDirectory}\"");
            }
        }

        private void _build(YAssembly assembly)
        {
            YReleaseManagementToolCore._startProcess("dotnet", $"clean \"{assembly.Url}\" -c Release");
            YReleaseManagementToolCore._startProcess("dotnet", $"build \"{assembly.Url}\" -c Release");
            YReleaseManagementToolCore._startProcess("dotnet", $"test \"{this._solution}\" -c Release");
        }

        private string _dotfuscate(YAssembly assembly)
        {
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

            return dotfuscatedDirectory;
        }

        private void _sign(YAssembly assembly, string dotfuscatedDirectory)
        {
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
        }

        private void _copyRequiredFiles(YAssembly assembly, string outpuoDirectory)
        {
            string fileList = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", $"{assembly.Name}.list.txt");
            string[] files = null;

            if (File.Exists(fileList))
            {
                files = File.ReadAllLines(fileList);
                foreach (string file in files)
                {
                    string sourceFile = Path.Combine(Path.GetDirectoryName(assembly.Url), file).Trim();
                    string destination = outpuoDirectory;

                    if (file.Contains('\t'))
                    {
                        sourceFile = Path.Combine(Path.GetDirectoryName(assembly.Url), file[0..file.IndexOf('\t')].Trim());
                        destination = Path.Combine(outpuoDirectory, file[file.IndexOf('\t')..].Trim());
                    }

                    YStaticMethods.Copy(sourceFile, destination, true);
                }
            }

            files = assembly.RequiredFiles.ToArray();
            string sourceDirectory = Path.GetDirectoryName(assembly.Url);
            foreach (string file in files)
            {
                string sourceFile = Path.Combine(sourceDirectory, file.Trim('/').Trim('\\')).Trim();
                string destination = Path.GetDirectoryName(sourceFile.Replace(sourceDirectory, outpuoDirectory));

                if (file.Contains('\t'))
                {
                    sourceFile = Path.Combine(Path.GetDirectoryName(assembly.Url), file[0..file.IndexOf('\t')].Trim());
                    destination = Path.Combine(outpuoDirectory, file[file.IndexOf('\t')..].Trim());
                }

                YStaticMethods.Copy(sourceFile, destination, true);
            }

            if (assembly.BinaryType != YBinaryType.ClassLibrary)
            {
                YAssembly.CreateRuntimeConfigJson(assembly.BinaryType, assembly.Name, outpuoDirectory);
            }
        }

        private void _innoSetup(YAssembly assembly, string dotfuscatedDirectory)
        {
            string innoSetup = this.ConfigProvider.GetConfiguration<string>(Config.InnoSetup);

            string innoSetupIss = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", $"{assembly.Name}.iss");

            if (File.Exists(innoSetupIss))
            {
                string issScript = File.ReadAllText(innoSetupIss);
                issScript = Regex.Replace(issScript, @"^#define MyAppVersion\s.*$", $"#define MyAppVersion \"{assembly.Version}\"", RegexOptions.Multiline);
                File.WriteAllText(innoSetupIss, issScript);

                YReleaseManagementToolCore._startProcess(innoSetup, $"\"{innoSetupIss}\"");

                string setupFile = Path.Combine(dotfuscatedDirectory, $"{assembly.Name}_setup_v{assembly.Version}.exe");
                YReleaseManagementToolCore._startProcess("\"./data/signtool.exe\"", $"sign /f \"./data/UpsilonEcosystem.pfx\" /p YL-upsilonecosystem-passw0rd \"{setupFile}\"");
            }
        }

        private void _generateAssemblyInfo(string assemblyName = null)
        {
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                YAssembly assembly = this.SolutionAssemblies.Find(x => x.Name == assemblyName);

                if (assembly == null)
                {
                    return;
                }

                assembly = assembly.Clone();
                string assemblyInfoPath = Path.Combine(Path.GetDirectoryName(assembly.Url), "deploy", "assembly.info");

                assembly.Url = null;
                string jsonString = JsonSerializer.Serialize(assembly, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(assemblyInfoPath, jsonString);
            }
            else
            {
                foreach (YAssembly assembly in this.SolutionAssemblies)
                {
                    this._generateAssemblyInfo(assembly.Name);
                }
            }
        }

        public void ComputeDeployedAssembliesJson(string outputDirectory, YAssembly assembly)
        {
            Dictionary<string, List<YAssembly>> assemblies = this.DeployedAssemblies;

            if (assembly != null)
            {
                assembly = assembly.Clone();
                assembly.Url = Path.GetDirectoryName(this.ConfigProvider.GetConfiguration<string>(Config.DeployedAssemblies)).Replace("\\", "//")
                    + "/" + Path.GetFileNameWithoutExtension(this._solution)
                    + "/" + assembly.Name.Replace(".", "/")
                    + "/" + assembly.Version;

                assembly.RequiredFiles = assembly.RequiredFiles.Select(x => assembly.Url + x).ToArray();

                assembly.Url += "/" + assembly.Name;

                if (assembly.BinaryType == YBinaryType.ClassLibrary)
                {
                    assembly.Url += ".dll";
                }
                else
                {
                    assembly.Url += "_setup_v" + assembly.Version + ".exe";
                }

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
            }

            assemblies = assemblies.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

            string jsonString = JsonSerializer.Serialize(assemblies, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(Path.Combine(outputDirectory, "deployed.assemblies.json"), jsonString);
        }

        private void _clearTempFiles()
        {
            File.Delete("./data/Resources.rc");
            File.Delete("./data/Resources.res");
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

        public void CheckIntegrity()
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

            if (!this.ConfigProvider.HasConfiguration(Config.DeployedAssemblies)
                || string.IsNullOrWhiteSpace(this.ConfigProvider.GetConfiguration<string>(Config.DeployedAssemblies)))
            {
                throw new Exception($"Deployed Assemblies json not set");
            }

            if (!this.ConfigProvider.HasConfiguration(Config.UploadTool)
                || string.IsNullOrWhiteSpace(this.ConfigProvider.GetConfiguration<string>(Config.UploadTool)))
            {
                throw new Exception($"Upload Tool not set");
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

        public void OpenUploadTool()
        {
            YStaticMethods.ProcessStartUrl(this.ConfigProvider.GetConfiguration<string>(Config.UploadTool));
        }

        public void DownloadAssembly(YAssembly assembly, string outputPath)
        {
            if (assembly.BinaryType == YBinaryType.ClassLibrary)
            {
                assembly.DownloadAssembly(this.DeployedAssemblies, outputPath);
            }
            else
            {
                YStaticMethods.DownloadFile(assembly.Url, Path.Combine(outputPath, Path.GetFileName(assembly.Url)));
            }
        }
    }
}
