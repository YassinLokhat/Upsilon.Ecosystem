using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;
using Upsilon.Common.MetaHelper;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YConfigurationProvider_UnitTests : YUnitTestsClass
    {
        private enum _configTest1
        {
            config1,
            config2,
            config3,
        }

        private enum _configTest2
        {
            config0,
            config1,
        }

        [TestMethod]
        public void Test_01_YConfigurationProvider_FileNotExist_WithoutKey()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "NotExists",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            string filePath = YHelper.GetTestFilePath(configuration);
            DateTime dateTime = new(637629857084246144);

            // When
            YConfigurationProvider<_configTest1> configurationProvider = new(filePath, string.Empty);
            configurationProvider.SetConfiguration(_configTest1.config1, filePath);
            configurationProvider.SetConfiguration(_configTest1.config2, dateTime);

            configurationProvider = new(filePath, string.Empty);
            string config1 = configurationProvider.GetConfiguration<string>(_configTest1.config1);
            DateTime config2 = configurationProvider.GetConfiguration<DateTime>(_configTest1.config2);
            string config3 = configurationProvider.GetConfiguration<string>(_configTest1.config3);

            // Then
            config1.Should().Be(filePath);
            config2.Should().Be(dateTime);
            config3.Should().BeNull();

            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_02_YConfigurationProvider_WithKey_OK()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "NotExists",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            string filePath = YHelper.GetTestFilePath(configuration);
            DateTime dateTime = new(637629857084246144);

            // When
            YConfigurationProvider<_configTest1> configurationProvider = new(filePath, "key");
            configurationProvider.SetConfiguration(_configTest1.config1, filePath);
            configurationProvider.SetConfiguration(_configTest1.config2, dateTime);

            configurationProvider = new(filePath, "key");
            string config1 = configurationProvider.GetConfiguration<string>(_configTest1.config1);
            DateTime config2 = configurationProvider.GetConfiguration<DateTime>(_configTest1.config2);
            string config3 = configurationProvider.GetConfiguration<string>(_configTest1.config3);

            // Then
            config1.Should().Be(filePath);
            config2.Should().Be(dateTime);
            config3.Should().BeNull();

            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_03_YConfigurationProvider_WithKey_KO()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "NotExists",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            string filePath = YHelper.GetTestFilePath(configuration);
            DateTime dateTime = new(637629857084246144);

            // When
            YConfigurationProvider<_configTest1> configurationProvider = new(filePath, "key");
            configurationProvider.SetConfiguration(_configTest1.config1, filePath);
            configurationProvider.SetConfiguration(_configTest1.config2, dateTime.SerializeObject());

            configurationProvider = new(filePath, string.Empty);

            Action act = new(() => 
            {
                string config1 = configurationProvider.GetConfiguration<string>(_configTest1.config1);
            });

            // Then
            act.Should().Throw<Exception>();

            YHelper.ClearTestFile(configuration);
        }

        [TestMethod]
        public void Test_04_YConfigurationProvider_UsingDifferentEnum()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202107271229",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
                ResetTempFile = true,
            };

            string filePath = YHelper.GenerateTempFile(configuration);
            DateTime now = new(637629857084246144);

            // When
            YConfigurationProvider<_configTest2> configurationProvider2 = new(filePath, string.Empty);
            string config0 = configurationProvider2.GetConfiguration<string>(_configTest2.config0);
            string config1 = configurationProvider2.GetConfiguration<string>(_configTest2.config1);

            // Then
            config0.Should().BeNull();
            config1.Should().Be("file path");

            // When
            configurationProvider2.SetConfiguration(_configTest2.config1, "new value");
            YConfigurationProvider<_configTest1> configurationProvider1 = new(filePath, string.Empty);
            config1 = configurationProvider1.GetConfiguration<string>(_configTest1.config1);
            DateTime config2 = configurationProvider1.GetConfiguration<DateTime>(_configTest1.config2);

            // Then
            config1.Should().Be("new value");
            config2.Should().Be(now);

            YHelper.ClearTestFile(configuration);
        }
    }
}
