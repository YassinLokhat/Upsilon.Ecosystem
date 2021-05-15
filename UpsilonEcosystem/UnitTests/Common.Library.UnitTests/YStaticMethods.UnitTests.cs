using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YStaticMethods_UnitTests
    {
        [TestMethod]
        public void Test_01_StaticMethods_IsIdentifiant_0_OK()
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
        public void Test_02_StaticMethods_IsIdentifiant_1_KO()
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
    }
}
