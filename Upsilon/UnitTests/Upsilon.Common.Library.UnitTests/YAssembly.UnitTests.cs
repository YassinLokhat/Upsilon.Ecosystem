using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;
using System.IO;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YAssembly_UnitTests
    {
        [TestMethod]
        public void Test_01_YAssembly_DownloadAssembly_OK()
        {
            // Given
            string tempDir = "temp";
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            var deployedAssemblies = YUpdateCenter.CheckForUpdate("http://api.upsilon-ecosystem.xyz/deployed.assemblies.json", "Upsilon.Tools.ReleaseManagementTool.GUI", out var assembly);

            // When
            assembly.DownloadAssembly(deployedAssemblies, tempDir);

            // Then
            File.Exists(@"temp\Upsilon.Common.Forms.dll").Should().BeTrue();
            File.Exists(@"temp\Upsilon.Common.Library.dll").Should().BeTrue();
            File.Exists(@"temp\Upsilon.Common.MetaHelper.dll").Should().BeTrue();
            File.Exists(@"temp\Upsilon.Database.Library.dll").Should().BeTrue();
            File.Exists(@"temp\Upsilon.Tools.ReleaseManagementTool.Core.dll").Should().BeTrue();
            File.Exists(@"temp\Upsilon.Tools.ReleaseManagementTool.GUI.dll").Should().BeTrue();
            File.Exists(@"temp\Upsilon.Tools.ReleaseManagementTool.GUI.exe").Should().BeTrue();
            File.Exists(@"temp\Upsilon.Tools.ReleaseManagementTool.GUI.runtimeconfig.json").Should().BeTrue();
            File.Exists(@"temp\data\GoRC.exe").Should().BeTrue();
            File.Exists(@"temp\data\ResourceHacker.exe").Should().BeTrue();
            File.Exists(@"temp\data\ResourceHacker.ini").Should().BeTrue();
            File.Exists(@"temp\data\Resources.template.rc").Should().BeTrue();
            File.Exists(@"temp\data\signtool.exe").Should().BeTrue();
            File.Exists(@"temp\data\UpsilonEcosystem.pfx").Should().BeTrue();

            // Finaly
            Directory.Delete(tempDir, true);
        }
    }
}
