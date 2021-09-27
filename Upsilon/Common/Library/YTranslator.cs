using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// <para>A string translation engine.</para>
    /// <para>Inherits from <c>Dictionary&lt;string, string></c>.</para>
    /// </summary>
    public sealed class YTranslator : Dictionary<string, string>
    {
        /// <summary>
        /// The language code
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// The language name
        /// </summary>
        public string LanguageName { get; private set; }

        /// <summary>
        /// Create a <c><see cref="YTranslator"/></c> from a <c><paramref name="filePath"/></c> file.
        /// </summary>
        /// <param name="filePath">The path of the file containing translations.</param>
        /// <param name="key">The key to uncipher the given file.</param>
        public YTranslator(string filePath, string key) : base()
        {
            YDebugTrace.TraceOn(new object[] { filePath, "key not logged" });
            
            JsonDocument document = JsonDocument.Parse(File.ReadAllText(filePath).Uncipher_Aes(key));
            JsonElement root = document.RootElement;
            this.LanguageCode = root.GetProperty("language_code").GetString();
            this.LanguageName = root.GetProperty("language_name").GetString();
            JsonElement dico = root.GetProperty("dictionary");

            foreach (JsonElement item in dico.EnumerateArray())
            {
                this[item.GetProperty("key").GetString()] = item.GetProperty("value").GetString();
            }

            YDebugTrace.TraceOff();
        }
    }
}
