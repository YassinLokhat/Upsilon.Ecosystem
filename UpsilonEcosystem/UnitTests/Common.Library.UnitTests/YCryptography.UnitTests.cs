using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YCryptography_UnitTests
    {
        [TestMethod]
        public void Test_01_Cryptography_Encrypt_Decrypt_0_OK()
        {
            // Given
            string plainText = Upsilon.Common.UnitTestsHelper.YHelper.GetRandomString();
            string key = Upsilon.Common.UnitTestsHelper.YHelper.GetRandomString();

            // When
            string cipherText = YCryptography.Encrypt_Aes(plainText, key);
            string result = YCryptography.Decrypt_Aes(cipherText, key);

            // Then
            result.Should().Be(plainText);
        }
        
        [TestMethod]
        public void Test_02_Cryptography_Encrypt_Decrypt_1_CipherTextCorrupted()
        {
            // Given
            string plainText = Upsilon.Common.UnitTestsHelper.YHelper.GetRandomString();
            string key = Upsilon.Common.UnitTestsHelper.YHelper.GetRandomString();

            // When
            string cipherText = YCryptography.Encrypt_Aes(plainText, key);
            Upsilon.Common.UnitTestsHelper.YHelper.CorruptString(ref cipherText);
            string result = YCryptography.Decrypt_Aes(cipherText, key);

            // Then
            result.Should().NotBe(plainText);
        }
        
        [TestMethod]
        public void Test_03_Cryptography_Encrypt_Decrypt_2_KeyCorrupted()
        {
            for (int i = 0; i < 1000; i++)
            {
                // Given
                string plainText = Upsilon.Common.UnitTestsHelper.YHelper.GetRandomString();
                string key = Upsilon.Common.UnitTestsHelper.YHelper.GetRandomString();
                string corruptedKey;
                do
                {
                    corruptedKey = Upsilon.Common.UnitTestsHelper.YHelper.GetRandomString();
                }
                while (key == corruptedKey);

                // When
                string cipherText = YCryptography.Encrypt_Aes(plainText, key);
                string result = YCryptography.Decrypt_Aes(cipherText, corruptedKey);

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
            string result = YCryptography.Encrypt_Aes(plainText, key);

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
            string result = YCryptography.Decrypt_Aes(cipherText, key);

            // Then
            result.Should().Be(plainText);
        }
    }
}
