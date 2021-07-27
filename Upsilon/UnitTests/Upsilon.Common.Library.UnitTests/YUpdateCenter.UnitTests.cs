﻿using FluentAssertions;
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
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.Library");

            // Then
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
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, false), "Upsilon.Common.Library");

            // Then
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
                YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.UnitTestsHelper");
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
                YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "SigmaShell");
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
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Database.Library");

            // Then
            assembly.Should().BeNull();
        }

        [TestMethod]
        public void Test_06_CheckForUpdate_OK_moreFieldsInTheJson()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202107130901",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            // When
            YAssembly assembly = YUpdateCentre.CheckForUpdate(YHelper.GetTestFilePath(configuration, false, true), "Upsilon.Common.Library");

            // Then
            assembly.Version.Should().Be("1.0.0.0");
            assembly.Description.Should().Be("Common features library");
        }
    }
}
