using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    public class YTranslator : Dictionary<string, string>
    {
        public string LanguageCode { get; private set; }
        public string LanguageName { get; private set; }

        public YTranslator(string file, string key) : base()
        {
            JsonDocument document = JsonDocument.Parse(File.ReadAllText(file).Uncipher_Aes(key));
            JsonElement root = document.RootElement;
            this.LanguageCode = root.GetProperty("language_code").GetString();
            this.LanguageName = root.GetProperty("language_name").GetString();
            JsonElement dico = root.GetProperty("dictionary");

            foreach (JsonElement item in dico.EnumerateArray())
            {
                this[item.GetProperty("key").GetString()] = item.GetProperty("value").GetString();
            }
        }
    }
}
