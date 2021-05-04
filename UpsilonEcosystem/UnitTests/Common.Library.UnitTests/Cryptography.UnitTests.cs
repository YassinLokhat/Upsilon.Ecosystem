using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class Cryptography_UnitTests
    {
        [TestMethod]
        public void Test_01_Cryptography_Encrypt_Decrypt_0_OK()
        {
            // Given
            string plainText = Upsilon.Common.UnitTestsHelper.Helper.GetRandomString();
            string key = Upsilon.Common.UnitTestsHelper.Helper.GetRandomString();

            // When
            string cipherText = Cryptography.Encrypt_Aes(plainText, key);
            string result = Cryptography.Decrypt_Aes(cipherText, key);

            // Then
            result.Should().Be(plainText);
        }
        
        [TestMethod]
        public void Test_02_Cryptography_Encrypt_Decrypt_1_CipherTextCorrupted()
        {
            // Given
            string plainText = Upsilon.Common.UnitTestsHelper.Helper.GetRandomString();
            string key = Upsilon.Common.UnitTestsHelper.Helper.GetRandomString();

            // When
            string cipherText = Cryptography.Encrypt_Aes(plainText, key);
            Upsilon.Common.UnitTestsHelper.Helper.CorruptString(ref cipherText);
            string result = Cryptography.Decrypt_Aes(cipherText, key);

            // Then
            result.Should().NotBe(plainText);
        }
        
        [TestMethod]
        public void Test_03_Cryptography_Encrypt_Decrypt_2_KeyCorrupted()
        {
            for (int i = 0; i < 1000; i++)
            {
                // Given
                string plainText = Upsilon.Common.UnitTestsHelper.Helper.GetRandomString();
                string key = Upsilon.Common.UnitTestsHelper.Helper.GetRandomString();
                string corruptedKey;
                do
                {
                    corruptedKey = Upsilon.Common.UnitTestsHelper.Helper.GetRandomString();
                }
                while (key == corruptedKey);

                // When
                string cipherText = Cryptography.Encrypt_Aes(plainText, key);
                string result = Cryptography.Decrypt_Aes(cipherText, corruptedKey);

                // Then
                result.Should().NotBe(plainText);
            }
        }

        [TestMethod]
        public void Test_04_Cryptography_Encrypt_0_KeyEmpty()
        {
            // Given
            string plainText = "plainText";
            string cipherText = plainText;
            string key = string.Empty;

            // When
            string result = Cryptography.Encrypt_Aes(plainText, key);

            // Then
            result.Should().Be(cipherText);
        }

        [TestMethod]
        public void Test_05_Cryptography_Decrypt_0_KeyEmpty()
        {
            // Given
            string plainText = "plainText";
            string cipherText = plainText;
            string key = string.Empty;

            // When
            string result = Cryptography.Decrypt_Aes(cipherText, key);

            // Then
            result.Should().Be(plainText);
        }
    }
}
