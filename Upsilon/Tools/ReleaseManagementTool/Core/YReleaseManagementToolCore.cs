using System;
using System.Collections.Generic;
using System.IO;
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
            string solutionDir = YHelper.GetSolutionDirectory();

            string[] projects = Directory.GetFiles(solutionDir, "*.csproj", SearchOption.AllDirectories);

            List<YAssembly> assemblies = new();

            foreach (string csproj in projects)
            {
                XmlDocument document = new();
                document.Load(csproj);

                YAssembly assembly = new()
                {
                    Name = Path.GetFileNameWithoutExtension(csproj),
                    Version = this._getVersionFromCsproj(document),
                    Dependencies = this._getDependenciesFromCsproj(document),
                    Depreciated = false,
                };

                assemblies.Add(assembly);
            }

            this.Assemblies = assemblies.ToArray();

            for (int i = 0; i < this.Assemblies.Length; i++)
            {
                this._fillDependencies(ref this.Assemblies[i]);
            }
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

        private YDependency[] _getDependenciesFromCsproj(XmlDocument document)
        {
            List<YDependency> dependencies = new();

            foreach (XmlNode projectReference in document.SelectNodes("/Project/ItemGroup/ProjectReference"))
            {
                YDependency dependency = new()
                {
                    Name = Path.GetFileNameWithoutExtension(projectReference.Attributes["Include"].Value),
                    MinimalVersion = "",
                    MaximalVersion = "",
                };

                dependencies.Add(dependency);
            }

            return dependencies.ToArray();
        }

        private void _fillDependencies(ref YAssembly assembly)
        {

        }
    }
}
