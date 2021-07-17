using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using Upsilon.Common.Library;
using Upsilon.Common.MetaHelper;

namespace Upsilon.Tools.ReleaseManagementTool.Core
{
    public sealed class YReleaseManagementToolCore
    {
        public YAssembly[] Assemblies { get; private set; } = null;

        public YReleaseManagementToolCore()
        {
            string[] projects = Directory.GetFiles(YHelper.GetSolutionDirectory(), "assembly.info", SearchOption.AllDirectories);

            List<YAssembly> assemblies = new();

            foreach (string csproj in projects)
            {
                YAssembly assembly = (YAssembly)File.ReadAllText(csproj).DeserializeObject(typeof(YAssembly));
                assembly.Url = Path.Combine(Path.GetDirectoryName(csproj), $"{assembly.Name}.csproj");

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
                assembly.Version = "exe";
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
                string assemblyInfoPath = Path.Combine(Path.GetDirectoryName(assembly.Url), "assembly.info");
                
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
    }
}
