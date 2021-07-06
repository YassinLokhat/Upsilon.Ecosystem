using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upsilon.Common.UnitTestsHelper;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YStringManagement_UnitTests
    {
        private readonly string _directory = "Files";

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
        public void Test_03_StringManagement_GetNextClosureOf_OK_1()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100700",
                Directory = _directory,
            };

            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            int index = str.GetNextClosureOf("{", "}");

            // Then
            index.Should().Be(235);
        }

        [TestMethod]
        public void Test_04_StringManagement_GetNextClosureOf_OK_2()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100700",
                Directory = _directory,
            };

            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));
            str = str[str.IndexOf('{')..];

            // When
            int index = str.GetNextClosureOf("{", "}");

            // Then
            index.Should().Be(170);
        }

        [TestMethod]
        public void Test_05_StringManagement_GetNextClosureOf_OK_3()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = _directory,
            };

            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            int index = str.GetNextClosureOf("<node>", "</node>");

            // Then
            index.Should().Be(60);
        }

        [TestMethod]
        public void Test_06_StringManagement_GetNextClosureOf_OK_4()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = _directory,
            };

            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            int index = str.GetNextClosureOf("<node>", "</node>", 70);

            // Then
            index.Should().Be(121);
        }

        [TestMethod]
        public void Test_07_StringManagement_GetNextClosureOf_OK_5()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = _directory,
            };

            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            int index = str.GetNextClosureOf("<node>", "</node>", 80);

            // Then
            index.Should().Be(90);
        }

        [TestMethod]
        public void Test_08_StringManagement_GetNextClosureOf_KO_1()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = _directory,
            };

            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            int index = str.GetNextClosureOf("<node>", "</node>", 111);

            // Then
            index.Should().Be(-1);
        }

        [TestMethod]
        public void Test_09_StringManagement_GetNextClosureOf_KO_2()
        {
            // Given
            YHelperDatabaseConfiguration configuration = new()
            {
                Reference = "202106100712",
                Directory = _directory,
            };

            string str = File.ReadAllText(YHelper.GetTestFilePath(configuration, "txt", false, true));

            // When
            int index = str.GetNextClosureOf("<root>", "</root>");

            // Then
            index.Should().Be(-1);
        }

        [TestMethod]
        public void Test_10_StringManagement_IndexOfPrevious()
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
    }
}
