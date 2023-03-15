using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using Upsilon.Common.MetaHelper;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YYSecurity_UnitTests
    {
        const int TEST_ROBUSTNESS_LEVEL = 100;
        const string ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+-";

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
            for (int i = 0; i < TEST_ROBUSTNESS_LEVEL; i++)
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
                result.Should().NotBe(plainText, $"{i}");
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
            for (int i = 0; i < TEST_ROBUSTNESS_LEVEL; i++)
            {   
                // Given
                string plainText = YHelper.GetRandomString();
                string[] keys = YHelper.GetRandomSetOfString();

                // When
                string cipherText = YSecurity.Encrypt(plainText, keys);
                string result = YSecurity.Decrypt(cipherText, keys);

                // Then
                result.Should().Be(plainText, $"{i}");
            }
        }

        [TestMethod]
        public void Test_07_Security_Encrypt_Decrypt_1_CipherCheckSignFailed()
        {
            // Given
            string plainText = YHelper.GetRandomString();
            string key = YHelper.GetRandomString();
            string[] keys = new string[] { key };

            string cipherText = YSecurity.Encrypt(plainText, keys);
            YHelper.CorruptString(ref cipherText);
            YSecurityErrorException exception = null;

            // When
            Action act = new(() =>
            {
                try
                {
                    string result = YSecurity.Decrypt(cipherText, keys);
                }
                catch (YSecurityErrorException ex)
                {
                    exception = ex;
                    throw;
                }
            });

            // Then
            act.Should().Throw<YSecurityErrorException>();
            exception.Source.Should().Be(YSecurityErrorException.SourceCode.CheckSignFailed);
            exception.ErrorLevel.Should().Be(0);
        }

        [TestMethod]
        public void Test_08_Security_Encrypt_Decrypt_1_CipherTextCorrupted_WithValidChar()
        {
            for (int i = 0; i < TEST_ROBUSTNESS_LEVEL; i++)
            {
                // Given
                string plainText = YHelper.GetRandomString();
                string key = YHelper.GetRandomString();
                string[] keys = new string[] { key };

                string cipherText = YSecurity.Encrypt(plainText, keys);
                cipherText = YSecurity.CheckSign(cipherText);

                for (int j = 0; j < 10; j++)
                {
                    int corruptionIndex = YHelper.GetRandomInt(0, cipherText.Length);
                    char corruptedChar = cipherText[corruptionIndex];
                    while (corruptedChar == cipherText[corruptionIndex])
                    {
                        corruptedChar = ALPHABET[YHelper.GetRandomInt(0, ALPHABET.Length)];
                    }
                    cipherText = cipherText[..corruptionIndex] + corruptedChar + cipherText[(corruptionIndex + 1)..];
                }

                cipherText = YSecurity.Sign(cipherText);
                YSecurityErrorException exception = null;

                // When
                Action act = new(() =>
                {
                    try
                    {
                        string result = YSecurity.Decrypt(cipherText, keys);
                        ;
                    }
                    catch (YSecurityErrorException ex)
                    {
                        exception = ex;
                        throw;
                    }
                });

                // Then
                act.Should().Throw<YSecurityErrorException>($"{i}");
                exception.Source.Should().Be(YSecurityErrorException.SourceCode.CorruptedSource, $"{i}");
                exception.ErrorLevel.Should().Be(0, $"{i}");
            }
        }

        [TestMethod]
        public void Test_09_Security_Encrypt_Decrypt_1_CipherTextCorrupted_WithInvalidChar()
        {
            for (int i = 0; i < TEST_ROBUSTNESS_LEVEL; i++)
            {
                // Given
                string plainText = YHelper.GetRandomString();
                string key = YHelper.GetRandomString();
                string[] keys = new string[] { key };

                string cipherText = YSecurity.Encrypt(plainText, keys);
                cipherText = YSecurity.CheckSign(cipherText);

                int corruptionIndex = YHelper.GetRandomInt(0, cipherText.Length);
                cipherText = cipherText[..corruptionIndex] + "." + cipherText[(corruptionIndex + 1)..];

                cipherText = YSecurity.Sign(cipherText);
                YSecurityErrorException exception = null;

                // When
                Action act = new(() =>
                {
                    try
                    {
                        string result = YSecurity.Decrypt(cipherText, keys);
                    }
                    catch (YSecurityErrorException ex)
                    {
                        exception = ex;
                        throw;
                    }
                });

                // Then
                act.Should().Throw<YSecurityErrorException>($"{i}");
                exception.Source.Should().Be(YSecurityErrorException.SourceCode.CorruptedSource, $"{i}");
                exception.ErrorLevel.Should().NotBe(0, $"{i}");
            }
        }

        [TestMethod]
        public void Test_10_Security_Encrypt_Decrypt_2_KeyCorrupted()
        {
            for (int i = 0; i < TEST_ROBUSTNESS_LEVEL; i++)
            {
                // Given
                string plainText = YHelper.GetRandomString();
                string[] keys = YHelper.GetRandomSetOfString();

                string[] corruptedKeys = keys.ToArray();
                int corruptionIndex = YHelper.GetRandomInt(0, corruptedKeys.Length);
                corruptedKeys[corruptionIndex] = YHelper.GetRandomString();

                string cipherText = YSecurity.Encrypt(plainText, keys);
                YSecurityErrorException exception = null;

                // When
                Action act = new(() =>
                {
                    try
                    {
                        string result = YSecurity.Decrypt(cipherText, corruptedKeys);
                    }
                    catch (YSecurityErrorException ex)
                    {
                        exception = ex;
                        throw;
                    }
                });

                // Then
                act.Should().Throw<YSecurityErrorException>($"{i}");
                exception.Source.Should().Be(YSecurityErrorException.SourceCode.WrongPasswords, $"{i}");
                exception.ErrorLevel.Should().Be(corruptionIndex, $"{i}");
            }
        }
    }
}
