﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// This class represent a security error exception.
    /// Thrown by YSecurity.Decrypt if any error occurs
    /// </summary>
    public class YSecurityErrorException : Exception
    {
        /// <summary>
        /// Represent the source of the error
        /// </summary>
        public enum SourceCode 
        {
            /// <summary>
            /// No error
            /// </summary>
            NoError = 0,
            /// <summary>
            /// The check sign failed
            /// </summary>
            CheckSignFailed,
            /// <summary>
            /// The passwords are wrong
            /// </summary>
            WrongPasswords,
            /// <summary>
            /// The source is corrupted
            /// </summary>
            CorruptedSource,
        }

        public SourceCode Source { get; set; }
        public int ErrorLevel { get; set; }
    }

    /// <summary>
    /// This static class contains the security functions.
    /// </summary>
    public static class YSecurity
    {
        private static string _alphabet = "BT2Cp4oOU-DqinLjy0HWxk8wI9rY1QgXblaef5RtdFE3sGm6PSzMJvKVhu7+NcZA";
        private static string _hexadecimal = "0123456789ABCDEF";

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

            byte[] bytes = _cipher_Aes(plainText, _key, IV);

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

            return _uncither_Aes(bytes, _key, IV);
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

        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="source">The string to encrypt.</param>
        /// <param name="passwords">The set of passwords.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(string source, string[] passwords)
        {
            source = _encrypt(source, passwords);
            source = _stringToUpsilonBase(source);
            source = source.Sign();

            return source;
        }

        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="source">The string to decrypt.</param>
        /// <param name="passwords">The set of passwords.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(string source, string[] passwords)
        {
            source = source.CheckSign();
            source = _upsilonBaseToString(source);
            source = _decrypt(source, passwords);

            return source;
        }

        private static string _encrypt(string source, string[] passwords)
        {
            passwords = passwords.Select(x => x.GetUpsilonHash()).ToArray();

            for (int i = 0; i < passwords.Length; i++)
            {
                source = Cipher_Aes(source.Sign(), passwords[i]);
            }

            source = Cipher_Aes(source.Sign(), string.Empty.GetUpsilonHash());

            return source;
        }

        private static string _decrypt(string source, string[] passwords)
        {
            try
            {
                source = Uncipher_Aes(source, string.Empty.GetUpsilonHash());
                source = source.CheckSign();
            }
            catch
            {
                throw new YSecurityErrorException
                {
                    Source = YSecurityErrorException.SourceCode.CorruptedSource,
                    ErrorLevel = 0,
                };
            }

            passwords = passwords.Select(x => x.GetUpsilonHash()).Reverse().ToArray();

            for (int i = 0; i < passwords.Length; i++)
            {
                try
                {
                    source = Uncipher_Aes(source, passwords[i]);
                    source = source.CheckSign();
                }
                catch
                {
                    throw new YSecurityErrorException
                    {
                        Source = YSecurityErrorException.SourceCode.WrongPasswords,
                        ErrorLevel = passwords.Length - i - 1,
                    };
                }
            }

            return source;
        }

        private static string _stringToUpsilonBase(string source)
        {
            StringBuilder hexaHigh = new StringBuilder(), hexaLow = new StringBuilder();
            var bytes = Encoding.UTF8.GetBytes(source);
            var seed = 0;

            foreach (var b in bytes)
            {
                var hexa = b.ToString("X2");
                var index = _hexadecimal.IndexOf(hexa[0]) + _hexadecimal.Length * seed;
                hexaHigh.Append(_alphabet[index]);
                index = _hexadecimal.IndexOf(hexa[1]) + _hexadecimal.Length * seed;
                hexaLow.Append(_alphabet[index]);
                seed = b % 3;
            }

            return hexaHigh.ToString() + hexaLow.ToString();
        }

        private static string _upsilonBaseToString(string source)
        {
            var bytes = new List<byte>();
            var bytesCount = source.Length / 2;

            for (int i = 0; i < bytesCount; i++)
            {
                var indexHigh = _alphabet.IndexOf(source[i]) % _hexadecimal.Length;
                var indexLow = _alphabet.IndexOf(source[i + bytesCount]) % _hexadecimal.Length;

                if (indexLow == -1 || 
                    indexHigh == -1)
                {
                    throw new YSecurityErrorException
                    {
                        Source = YSecurityErrorException.SourceCode.CorruptedSource,
                        ErrorLevel = bytesCount + 1,
                    };
                }

                string hexa = $"{_hexadecimal[indexHigh]}{_hexadecimal[indexLow]}";
                bytes.Add(Convert.ToByte(hexa, 16));
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// The size of an Upsilon Hash
        /// </summary>
        public static int UpsilonHashLength
        {
            get
            {
                return "".GetUpsilonHash().Length;
            }
        }

        /// <summary>
        /// Get tha Upsilon Hash of a string
        /// </summary>
        /// <param name="source">The source to hash.</param>
        /// <returns>The Upsilon Hash</returns>
        public static string GetUpsilonHash(this string source)
        {
            MD5 md5 = MD5.Create();
            var md5Hash = md5.ComputeHash(Encoding.UTF8.GetBytes(source));

            SHA1 sha1 = SHA1.Create();
            var sha1Hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));

            StringBuilder hash = new StringBuilder();
            var seed = 0;

            for (int i = 0; i < md5Hash.Length; i++)
            {
                var b = (md5Hash[i] ^ sha1Hash[i]);
                var hexa = b.ToString("X2");
                var indexHigh = _hexadecimal.IndexOf(hexa[0]) + _hexadecimal.Length * seed;
                var indexLow = _hexadecimal.IndexOf(hexa[1]) + _hexadecimal.Length * seed;
                seed = b % 3;

                hash.Append($"{_alphabet[indexHigh]}{_alphabet[indexLow]}");
            }

            return hash.ToString();
        }

        /// <summary>
        /// Sign a string.
        /// </summary>
        /// <param name="source">The string to sign.</param>
        /// <returns>The signed string.</returns>
        public static string Sign(this string source)
        {
            return source.GetUpsilonHash() + source;
        }

        /// <summary>
        /// Check and unsign a string.
        /// </summary>
        /// <param name="source">The string to check.</param>
        /// <returns>The unsigned string.</returns>
        public static string CheckSign(this string source)
        {
            try
            {
                var hashSource = source[..UpsilonHashLength];
                var hashCheck = source[UpsilonHashLength..].GetUpsilonHash();

                if (hashSource != hashCheck)
                {
                    throw new Exception();
                }

                source = source[UpsilonHashLength..];
            }
            catch
            {
                throw new YSecurityErrorException
                {
                    Source = YSecurityErrorException.SourceCode.CheckSignFailed, 
                    ErrorLevel = 0,
                };
            }

            return source;
        }
    }
}
