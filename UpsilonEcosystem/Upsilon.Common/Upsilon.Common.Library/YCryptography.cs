﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// This static class contains the Cryptographic functions.
    /// </summary>
    public static class YCryptography
    {
        /// <summary>
        /// Cipher the <c><paramref name="plainText"/></c> string using the <c><paramref name="key"/></c>.
        /// </summary>
        /// <param name="plainText">The plain text to cipher.</param>
        /// <param name="key">The password key.</param>
        /// <returns></returns>
        public static string Cipher_Aes(this string plainText, string key)
        {
            if (String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            key = Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(key)));
            key += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(key)));
            key += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(key)));

            byte[] _key = Encoding.ASCII.GetBytes(key.Substring(0, 32));
            byte[] IV = Encoding.ASCII.GetBytes(key.Substring(32, 16));

            byte[] bytes = YCryptography._cipher_Aes(plainText, _key, IV);

            return new string(bytes.Select(x => (char)x).ToArray());
        }

        private static byte[] _cipher_Aes(string plainText, byte[] key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));
            byte[] encrypted = null;

            try
            {
                // Create an AesManaged object
                // with the specified key and IV.
                using AesManaged aesAlg = new();
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// Uncipher the <c><paramref name="cipherText"/></c> string using the <c><paramref name="key"/></c>.
        /// </summary>
        /// <param name="cipherText">The cither text to uncipher.</param>
        /// <param name="key">The password key.</param>
        /// <returns></returns>
        public static string Uncipher_Aes(this string cipherText, string key)
        {
            if (String.IsNullOrWhiteSpace(key) || cipherText.Length == 0)
            {
                return cipherText;
            }

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            key = Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(key)));
            key += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(key)));
            key += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(key)));

            byte[] _key = Encoding.ASCII.GetBytes(key.Substring(0, 32));
            byte[] IV = Encoding.ASCII.GetBytes(key.Substring(32, 16));

            byte[] bytes = cipherText.Select(x => (byte)x).ToArray();

            return YCryptography._uncither_Aes(bytes, _key, IV);
        }

        private static string _uncither_Aes(byte[] cipherText, byte[] key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create an AesManaged object
                // with the specified key and IV.
                using AesManaged aesAlg = new();
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using MemoryStream msDecrypt = new(cipherText);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return plaintext;
        }
    }
}
