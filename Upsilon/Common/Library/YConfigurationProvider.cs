﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// Represents a Configuration provider on the given enum.
    /// </summary>
    /// <typeparam name="T">The enum type of the configuration file. Each element of the enum will have its own configuration object.</typeparam>
    public class YConfigurationProvider<T> where T : struct
    {
        /// <summary>
        /// The configuration file path.
        /// </summary>
        public string ConfigurationFile { get; private set; }

        private string _key = string.Empty;
        private Dictionary<string, string> _configurations = null;

        /// <summary>
        /// Create a new configuration provider from the given <paramref name="configurationFile"/>.
        /// </summary>
        /// <param name="configurationFile">The configuration file paht. If it does not exist, it will be created.</param>
        /// <param name="key">The encryption key. Leave empty to disable encryption.</param>
        public YConfigurationProvider(string configurationFile, string key)
        {
            if (!File.Exists(configurationFile))
            {
                File.CreateText(configurationFile).Close();
            }

            this.ConfigurationFile = configurationFile;
            this._key = key;
            this._configurations = new();
        }

        /// <summary>
        /// Set the configuration value of the given configuration key.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="configurationValue">The configuration value.</param>
        public void SetConfiguration(T configurationKey, string configurationValue)
        {
            this._configurations[configurationKey.ToString()] = configurationValue.Cipher_Aes(this._key);
            this._saveConfigFile();
        }

        /// <summary>
        /// Get the configuration value of the given configuration key.
        /// </summary>
        /// <typeparam name="U">The type of the configuration value.</typeparam>
        /// <param name="configurationKey">The configuration key.</param>
        /// <returns>The configuration value or the default value of <typeparamref name="U"/> if the given <paramref name="configurationKey"/> is missing.</returns>
        public string GetConfiguration(T configurationKey)
        {
            this._loadConfigFile();
            if (this._configurations.ContainsKey(configurationKey.ToString()))
            {
                return this._configurations[configurationKey.ToString()].Uncipher_Aes(this._key);
            }

            return null;
        }

        private void _saveConfigFile()
        {
            File.WriteAllText(this.ConfigurationFile, this._configurations.SerializeObject());
        }

        private void _loadConfigFile()
        {
            this._configurations.Clear();

            try
            {
                this._configurations = File.ReadAllText(this.ConfigurationFile).DeserializeObject<Dictionary<string, string>>();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}