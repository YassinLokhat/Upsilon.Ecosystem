using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upsilon.Common.MetaHelper;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YUpdateCenter_UnitTests
    {
        [TestMethod]
        public void Test_01_CheckForUpdate_OK()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202106181373",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.Library", out YAssembly assembly);

            // Then
            deployedList.Should().BeNull();
            assembly.Version.Should().Be("1.1.0.0");
        }

        [TestMethod]
        public void Test_02_CheckForUpdate_OK_ConfigMissing()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "NotExists",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, false), "Upsilon.Common.Library", out YAssembly assembly);

            // Then
            deployedList.Should().BeNull();
            assembly.Should().BeNull();
        }

        [TestMethod]
        public void Test_03_CheckForUpdate_KO_PrivateAssembly()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202106181373",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            Action act = new(() => 
            {
                var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.UnitTestsHelper", out YAssembly assembly);
            });

            // Then
            act.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Test_04_CheckForUpdate_KO_AssemblyBecamePrivate()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202106181373",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            Action act = new(() =>
            {
                var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "SigmaShell", out YAssembly assembly);
            });

            // Then
            act.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Test_05_CheckForUpdate_KO_MissingAssembly()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202106181373",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Database.Library", out YAssembly assembly);

            // Then
            deployedList.Should().BeNull();
            assembly.Should().BeNull();
        }

        [TestMethod]
        public void Test_06_CheckForUpdate_OK_MoreFieldsInTheJson()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202107130901",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.Library", out YAssembly assembly);

            // Then
            deployedList.Should().BeNull();
            assembly.Version.Should().Be("1.0.0.0");
            assembly.Description.Should().Be("Common features library");
        }

        [TestMethod]
        public void Test_07_CheckForUpdate_DictionnaryJson_OK()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202108091545",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.Library", out YAssembly assembly);

            // Then
            deployedList.ContainsKey("Upsilon.Common.Forms").Should().BeTrue();
            deployedList.ContainsKey("Upsilon.Common.Library").Should().BeTrue();
            deployedList.ContainsKey("Upsilon.Database.Forms").Should().BeFalse();
            deployedList["Upsilon.Common.Library"].Count.Should().Be(5);
            assembly.Version.Should().Be("1.0.4");
            assembly.Url.Should().Be("\\BinaryServer\\Binaries\\UpsilonEcosystem\\Upsilon\\Common\\Library\\1.0.4\\Upsilon.Common.Library.dll");
            assembly.Description.Should().Be("Common features library.");
        }

        [TestMethod]
        public void Test_08_CheckForUpdate_DictionnaryJson_KO_AssemblyPrivate()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202108091545",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            Action act = new(() =>
            {
                var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.Forms", out YAssembly assembly);
            });

            // Then
            act.Should().Throw<Exception>();
        }

        [Ignore]
        [TestMethod]
        public void Test_09_CheckForUpdate_Online_OK()
        {
            // Given
            string url = "http://api.upsilon-ecosystem.xyz/deployed.assemblies.json";

            // When
            var deployedList = YUpdateCenter.CheckForUpdate(url, "Upsilon.Common.Forms", out YAssembly assembly);

            // Then
            deployedList.Should().NotBeNull();
            assembly.Should().NotBeNull();
        }

        [TestMethod]
        public void Test_10_CheckForUpdate_ConfigFile_OK()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202202031601",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // Given
            YHelperConfiguration server1Config = new()
            {
                Reference = "202106181373",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            var config = new YConfigurationProvider<bool>(YHelper.GetTestFilePath(configuration, true, false), string.Empty);
            config.SetConfiguration(true, new string[] 
            {
                "http://api.upsilon-ecosystem.xyz/deployed.assemblies.json",
                YHelper.GetTestFilePath(server1Config, false, true),
            });

            // When
            var deployedList = YUpdateCenter.CheckForUpdate(YHelper.GetTestFilePath(configuration, true, false), string.Empty, "Upsilon.Common.Library", out YAssembly assembly);

            // Then
            deployedList.Should().BeNull();
            assembly.Version.Should().Be("1.1.0.0");

            YHelper.ClearTestFile(configuration);
        }
    }
}
