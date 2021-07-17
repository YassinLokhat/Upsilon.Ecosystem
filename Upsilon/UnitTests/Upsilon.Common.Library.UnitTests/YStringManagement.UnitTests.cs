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
    public class YStringManagement_UnitTests
    {
        [TestMethod]
        public void Test_01_StringManagement_IsIdentifiant_0_OK()
        {
            // Given
            string[] identifiants = { "test", "Test", "test_", "_test", "test0" };

            foreach (string identifiant in identifiants)
            {
                // When
                bool isId = identifiant.IsIdentifiant();

                // Then
                isId.Should().BeTrue("'" + identifiant + "' is an identifiant");
            }
        }

        [TestMethod]
        public void Test_02_StringManagement_IsIdentifiant_1_KO()
        {
            // Given
            string[] identifiants = { "", "T?est", "0test_", "_te st", "test0\n" };

            foreach (string identifiant in identifiants)
            {
                // When
                bool isId = identifiant.IsIdentifiant();

                // Then
                isId.Should().BeFalse("'" + identifiant + "' is not an identifiant");
            }
        }

        [TestMethod]
        public void Test_03_StringManagement_IndexOfPrevious()
        {
            // Given
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // When
            int index0 = string.Empty.IndexOfPrevious("ABC");
            int index1 = str.IndexOfPrevious("ABC");
            int index2 = str.IndexOfPrevious("ABC", 26);
            int index3 = str.IndexOfPrevious("ABC", 25);
            int index4 = str.IndexOfPrevious("ABC", 2);
            int index5 = str.IndexOfPrevious("ABC", 0);
            int index6 = str.IndexOfPrevious("AZ");

            // Then
            index0.Should().Be(-1);
            index1.Should().Be(26);
            index2.Should().Be(26);
            index3.Should().Be(0);
            index4.Should().Be(0);
            index5.Should().Be(0);
            index6.Should().Be(-1);
        }

        [TestMethod]
        public void Test_04_StringManagement_GetNextTextBlock_NoIgnore()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202107091504",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "\"",
                BlockEnd = "\"",
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.StartIndex.Should().Be(13);
            textBlock.EndIndex.Should().Be(18);
            textBlock.InnerText.Should().Be("test");
            textBlock.OuterText.Should().Be("\"test\"");
        }

        [TestMethod]
        public void Test_05_StringManagement_GetNextTextBlock_InlineIgnore()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202107091504",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "\"",
                BlockEnd = "\"",
                InlineIgnore = "//",
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.StartIndex.Should().Be(162);
            textBlock.EndIndex.Should().Be(176);
            textBlock.InnerText.Should().Be("comment block");
            textBlock.OuterText.Should().Be("\"comment block\"");
        }

        [TestMethod]
        public void Test_06_StringManagement_GetNextTextBlock_BlockIgnore()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202107091504",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "\"",
                BlockEnd = "\"",
                InlineIgnore = "//",
                BlockIgnoreStart = "/*",
                BlockIgnoreEnd = "*/",
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.StartIndex.Should().Be(210);
            textBlock.EndIndex.Should().Be(224);
            textBlock.InnerText.Should().Be("Hello World !");
            textBlock.OuterText.Should().Be("\"Hello World !\"");
        }

        [TestMethod]
        public void Test_07_StringManagement_GetNextTextBlock_Escape()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202107091504",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "\"",
                BlockEnd = "\"",
                Escape = "\\",
                StartIndex = 230,
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.StartIndex.Should().Be(237);
            textBlock.EndIndex.Should().Be(257);
            textBlock.InnerText.Should().Be("\\n\\\"Hello World !\\\"");
            textBlock.OuterText.Should().Be("\"\\n\\\"Hello World !\\\"\"");
        }

        [TestMethod]
        public void Test_08_StringManagement_GetNextTextBlock_OK_1()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100700",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "{",
                BlockEnd = "}",
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.EndIndex.Should().Be(235);
        }

        [TestMethod]
        public void Test_09_StringManagement_GetNextTextBlock_OK_2()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100700",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "{",
                BlockEnd = "}",
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));
            str = str[str.IndexOf('{')..];

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.EndIndex.Should().Be(170);
        }

        [TestMethod]
        public void Test_10_StringManagement_GetNextTextBlock_OK_3()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "<node>",
                BlockEnd = "</node>",
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.EndIndex.Should().Be(60);
        }

        [TestMethod]
        public void Test_11_StringManagement_GetNextTextBlock_OK_4()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "<node>",
                BlockEnd = "</node>",
                StartIndex = 70,
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.EndIndex.Should().Be(121);
        }

        [TestMethod]
        public void Test_12_StringManagement_GetNextTextBlock_OK_5()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "<node>",
                BlockEnd = "</node>",
                StartIndex = 80,
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.EndIndex.Should().Be(90);
        }

        [TestMethod]
        public void Test_13_StringManagement_GetNextTextBlock_KO_1()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "<node>",
                BlockEnd = "</node>",
                StartIndex = 111,
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.EndIndex.Should().Be(-1);
        }

        [TestMethod]
        public void Test_14_StringManagement_GetNextTextBlock_KO_2()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = YUnitTestFilesDirectory.Files,
            };

            YTextBlockSearchConfiguration searchConf = new()
            {
                BlockStart = "<root>",
                BlockEnd = "</root>",
            };
            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            YTextBlock textBlock = str.GetNextTextBlock(searchConf);

            // Then
            textBlock.EndIndex.Should().Be(-1);
        }
    }
}
