﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Upsilon.Common.Library
{
    public static class Cryptography
    {
        public static string Encrypt_Aes(string plainText, string password)
        {
            if (String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            password = Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(password)));
            password += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(password)));
            password += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(password)));

            byte[] key = Encoding.ASCII.GetBytes(password.Substring(0, 32));
            byte[] IV = Encoding.ASCII.GetBytes(password.Substring(32, 16));

            byte[] bytes = Cryptography.Encrypt_Aes(plainText, key, IV);

            return new string(bytes.Select(x => (char)x).ToArray());
        }

        public static byte[] Encrypt_Aes(string plainText, byte[] key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted = null;

            try
            {
                // Create an AesManaged object
                // with the specified key and IV.
                using AesManaged aesAlg = new AesManaged();
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new MemoryStream();
                using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
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

        public static string Decrypt_Aes(string cipherText, string password)
        {
            if (String.IsNullOrWhiteSpace(password) || cipherText.Length == 0)
            {
                return cipherText;
            }

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            password = Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(password)));
            password += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(password)));
            password += Encoding.ASCII.GetString(mD5.ComputeHash(Encoding.ASCII.GetBytes(password)));

            byte[] key = Encoding.ASCII.GetBytes(password.Substring(0, 32));
            byte[] IV = Encoding.ASCII.GetBytes(password.Substring(32, 16));

            byte[] bytes = cipherText.Select(x => (byte)x).ToArray();

            return Cryptography.Decrypt_Aes(bytes, key, IV);
        }

        public static string Decrypt_Aes(byte[] cipherText, byte[] key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create an AesManaged object
                // with the specified key and IV.
                using AesManaged aesAlg = new AesManaged();
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using MemoryStream msDecrypt = new MemoryStream(cipherText);
                using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new StreamReader(csDecrypt);

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
