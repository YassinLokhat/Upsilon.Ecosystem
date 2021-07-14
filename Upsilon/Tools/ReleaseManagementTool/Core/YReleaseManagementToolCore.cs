using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string[] projects = Directory.GetFiles(YHelper.GetSolutionDirectory(), "packet.dependencies", SearchOption.AllDirectories);

            List<YAssembly> assemblies = new();

            foreach (string csproj in projects)
            {
                YAssembly assembly = (YAssembly)File.ReadAllText(csproj).DeserializeObject(typeof(YAssembly));

                assemblies.Add(assembly);
            }

            this.Assemblies = assemblies.ToArray();
        }

        public YAssembly SelectAssembly(string assemblyName)
        {
            YAssembly assembly = this.Assemblies.Where(x => x.Name == assemblyName).FirstOrDefault();

            if (assembly == null)
                return null;

            string csproj = Directory.GetFiles(YHelper.GetSolutionDirectory(), $"{assembly.Name}.csproj", SearchOption.AllDirectories).FirstOrDefault();
            
            XmlDocument document = new();
            document.Load(csproj);

            assembly.Version = this._getVersionFromCsproj(document);
            this._fillDependenciesFromCsproj(document, assembly);

            foreach (YAssembly asb in this.Assemblies)
            {
                YDependency dependency = asb.Dependencies.Where(x => x.Name == assembly.Name).FirstOrDefault();
                if (dependency != null)
                {
                    dependency.MaximalVersion = assembly.Version;
                    if (dependency.MinimalVersion == string.Empty)
                    {
                        dependency.MinimalVersion = dependency.MaximalVersion;
                    }
                }
            }

            return assembly;
        }

        private string _getVersionFromCsproj(XmlDocument document)
        {
            XmlNode version = document.SelectSingleNode("/Project/PropertyGroup/Version");

            if (version == null)
            {
                return "1.0.0";
            }

            return version.InnerText;
        }

        private void _fillDependenciesFromCsproj(XmlDocument document, YAssembly assembly)
        {
            List<YDependency> dependencies = new();

            foreach (XmlNode projectReference in document.SelectNodes("/Project/ItemGroup/ProjectReference"))
            {
                YDependency dependency = assembly.Dependencies.Where(x => x.Name == projectReference.Attributes["Include"].Value).FirstOrDefault();

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
        }
    }
}
