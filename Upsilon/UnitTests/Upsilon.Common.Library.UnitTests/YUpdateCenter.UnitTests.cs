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
        private readonly string _directory = "Files";

        [TestMethod]
        public void Test_01_CheckForUpdate_OK()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106181373",
                Directory = _directory,
            };

            // When
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, "json", false, true), "Upsilon.Common.Library");

            // Then
            assembly.Version.Should().Be("1.1.0.0");
        }

        [TestMethod]
        public void Test_02_CheckForUpdate_OK_ConfigMissing()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106181375",
                Directory = _directory,
            };

            // When
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, "json", false, false), "Upsilon.Common.Library");

            // Then
            assembly.Should().BeNull();
        }

        [TestMethod]
        public void Test_03_CheckForUpdate_KO_PrivateAssembly()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106181373",
                Directory = _directory,
            };

            // When
            Action act = new(() => 
            {
                YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, "json", false, true), "Upsilon.Common.UnitTestsHelper");
            });

            // Then
            act.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Test_04_CheckForUpdate_KO_AssemblyBecamePrivate()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106181373",
                Directory = _directory,
            };

            // When
            Action act = new(() =>
            {
                YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, "json", false, true), "SigmaShell");
            });

            // Then
            act.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Test_05_CheckForUpdate_KO_MissingAssembly()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106181373",
                Directory = _directory,
            };

            // When
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, "json", false, true), "Upsilon.Database.Library");

            // Then
            assembly.Should().BeNull();
        }

        [TestMethod]
        public void Test_06_CheckForUpdate_OK_moreFieldsInTheJson()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202107130901",
                Directory = _directory,
            };

            // When
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, "json", false, true), "Upsilon.Common.Library");

            // Then
            assembly.Version.Should().Be("1.0.0.0");
            assembly.Description.Should().Be("Common features library");
        }
    }
}
