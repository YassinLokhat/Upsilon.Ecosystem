using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;
using Upsilon.Common.UnitTestsHelper;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YTranslator_UnitTests
    {
        private readonly string _languageDirectory = @"\UpsilonEcosystem\UnitTests\Tests\Language\";

        [TestMethod]
        public void Test_01_YTranslator_OK()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106051015",
                DatabaseDirectory = _languageDirectory,
                Key = string.Empty,
            };

            string filePath = YHelper.GetTestFilePath(configuration, "ulf", false, true);

            // When
            YTranslator translator = new YTranslator(filePath, configuration.Key);

            // Then
            translator.Count.Should().Be(6);
            translator.LanguageCode.Should().Be("en-EN");
            translator.LanguageName.Should().Be("English");
            translator["database_key"].Should().Be("Database key");
            translator["upsilon_database_file"].Should().Be("Upsilon Database file");
            translator[""].Should().Be("");
        }

        [TestMethod]
        public void Test_02_YTranslator_WrongKey()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106051015",
                DatabaseDirectory = _languageDirectory,
                Key = "key",
            };

            string filePath = YHelper.GetTestFilePath(configuration, "ulf", false, true);

            // When
            Action act = () =>
            {
                YTranslator translator = new YTranslator(filePath, configuration.Key);
            };

            // Then
            act.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Test_03_YTranslator_FileCorruption()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106051023",
                DatabaseDirectory = _languageDirectory,
                Key = string.Empty,
            };

            string filePath = YHelper.GetTestFilePath(configuration, "ulf", false, true);

            // When
            Action act = () =>
            {
                YTranslator translator = new YTranslator(filePath, configuration.Key);
            };

            // Then
            act.Should().Throw<Exception>();
        }
    }
}
