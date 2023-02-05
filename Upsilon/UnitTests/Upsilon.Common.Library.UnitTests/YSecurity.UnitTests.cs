using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using Upsilon.Common.MetaHelper;
using System.Collections.Generic;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YYSecurity_UnitTests
    {
        [TestMethod]
        public void Test_01_Security_Encrypt_Decrypt_AES_0_OK()
        {
            // Given
            string plainText = YHelper.GetRandomString();
            string key = YHelper.GetRandomString();

            // When
            string cipherText = plainText.Cipher_Aes(key);
            string result = cipherText.Uncipher_Aes(key);

            // Then
            result.Should().Be(plainText);
        }
        
        [TestMethod]
        public void Test_02_Security_Encrypt_Decrypt_AES_1_CipherTextCorrupted()
        {
            // Given
            string plainText = YHelper.GetRandomString();
            string key = YHelper.GetRandomString();

            // When
            string cipherText = plainText.Cipher_Aes(key);
            YHelper.CorruptString(ref cipherText);
            string result = cipherText.Uncipher_Aes(key);

            // Then
            result.Should().NotBe(plainText);
        }
        
        [TestMethod]
        public void Test_03_Security_Encrypt_Decrypt_AES_2_KeyCorrupted()
        {
            for (int i = 0; i < 1000; i++)
            {
                // Given
                string plainText = YHelper.GetRandomString();
                string key = YHelper.GetRandomString();
                string corruptedKey;
                do
                {
                    corruptedKey = YHelper.GetRandomString();
                }
                while (key == corruptedKey);

                // When
                string cipherText = plainText.Cipher_Aes(key);
                string result = cipherText.Uncipher_Aes(corruptedKey);

                // Then
                result.Should().NotBe(plainText);
            }
        }

        [TestMethod]
        public void Test_04_Security_Encrypt_AES_0_KeyEmpty()
        {
            // Given
            string plainText = "plainText";
            string cipherText = plainText;
            string key = string.Empty;

            // When
            string result = plainText.Cipher_Aes(key);

            // Then
            result.Should().Be(cipherText);
        }

        [TestMethod]
        public void Test_05_Security_Decrypt_AES_0_KeyEmpty()
        {
            // Given
            string plainText = "plainText";
            string cipherText = plainText;
            string key = string.Empty;

            // When
            string result = cipherText.Uncipher_Aes(key);

            // Then
            result.Should().Be(plainText);
        }

        [TestMethod]
        public void Test_06_Security_Encrypt_Decrypt_0_OK()
        {
            for (int i = 0; i < 100; i++)
            {   
                // Given
                string plainText = YHelper.GetRandomString();
                string[] keys = YHelper.GetRandomSetOfString();

                // When
                string cipherText = YSecurity.Encrypt(plainText, keys);
                string result = YSecurity.Decrypt(cipherText, keys);

                // Then
                result.Should().Be(plainText);
            }
        }

        [TestMethod]
        public void Test_07_Security_Encrypt_Decrypt_1_CipherTextCorrupted()
        {
            // Given
            string plainText = YHelper.GetRandomString();
            string key = YHelper.GetRandomString();
            string[] keys = new string[] { key };

            // When
            string cipherText = YSecurity.Encrypt(plainText, keys);
            YHelper.CorruptString(ref cipherText);
            string result = YSecurity.Encrypt(cipherText, keys);

            // Then
            result.Should().NotBe(plainText);
        }

        [TestMethod]
        public void Test_08_Security_Encrypt_Decrypt_2_KeyCorrupted()
        {
            for (int i = 0; i < 1000; i++)
            {
                // Given
                string plainText = YHelper.GetRandomString();
                string key = YHelper.GetRandomString();
                string corruptedKey;
                do
                {
                    corruptedKey = YHelper.GetRandomString();
                }
                while (key == corruptedKey);

                // When
                string cipherText = YSecurity.Encrypt(plainText, new string[] { key });
                string result = YSecurity.Encrypt(cipherText, new string[] { corruptedKey });

                // Then
                result.Should().NotBe(plainText);
            }
        }
    }
}
