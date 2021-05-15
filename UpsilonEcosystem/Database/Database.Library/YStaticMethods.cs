using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;
using Upsilon.Database.Library;

namespace Upsilon.Database.Library
{
    public static class YStaticMethods
    {
        public static void EncryptXml(this XmlDocument document, string filename, string key)
        {
            var root = document.SelectSingleNode("//tables");

            if (root == null)
            {
                throw new YDatabaseXmlCorruptionException(filename, key, "'tables' node is missing.");
            }

            if (root.Attributes.IsNullOrWhiteSpace("key"))
            {
                throw new YDatabaseXmlCorruptionException(filename, key, "'key' attribute is missing.");
            }

            string hash = key.GetMD5HashCode();

            if (hash != root.Attributes["key"].Value)
            {
                throw new YWrongDatabaseKeyException(filename, key);
            }

            if (String.IsNullOrWhiteSpace(key))
            {
                return;
            }

            var tables = root.SelectNodes("./table");
            foreach (XmlNode table in tables)
            {
                for (int i = 0; i < table.Attributes.Count; i++)
                {
                    table.Attributes[i].Value = YCryptography.Encrypt_Aes(table.Attributes[i].Value, key);
                }

                var fields = table.SelectNodes("./fields/field");
                foreach (XmlNode field in fields)
                {
                    for (int i = 0; i < field.Attributes.Count; i++)
                    {
                        field.Attributes[i].Value = YCryptography.Encrypt_Aes(field.Attributes[i].Value, key);
                    }
                }

                var records = table.SelectNodes("./records/record");
                foreach (XmlNode record in records)
                {
                    for (int i = 0; i < record.Attributes.Count; i++)
                    {
                        record.Attributes[i].Value = YCryptography.Encrypt_Aes(record.Attributes[i].Value, key);
                    }
                }
            }
        }

        public static void DecryptXml(this XmlDocument document, string filename, string key)
        {
            var root = document.SelectSingleNode("//tables");

            if (root == null)
            {
                throw new YDatabaseXmlCorruptionException(filename, key, "'tables' node is missing.");
            }

            if (root.Attributes.IsNullOrWhiteSpace("key"))
            {
                throw new YDatabaseXmlCorruptionException(filename, key, "'key' attribute is missing.");
            }

            string hash = key.GetMD5HashCode();

            if (hash != root.Attributes["key"].Value)
            {
                throw new YWrongDatabaseKeyException(filename, key);
            }

            if (String.IsNullOrWhiteSpace(key))
            {
                return;
            }

            var tables = root.SelectNodes("./table");
            foreach (XmlNode table in tables)
            {
                for (int i = 0; i < table.Attributes.Count; i++)
                {
                    table.Attributes[i].Value = YCryptography.Decrypt_Aes(table.Attributes[i].Value, key);
                }

                var fields = table.SelectNodes("./fields/field");
                foreach (XmlNode field in fields)
                {
                    for (int i = 0; i < field.Attributes.Count; i++)
                    {
                        field.Attributes[i].Value = YCryptography.Decrypt_Aes(field.Attributes[i].Value, key);
                    }
                }

                var records = table.SelectNodes("./records/record");
                foreach (XmlNode record in records)
                {
                    for (int i = 0; i < record.Attributes.Count; i++)
                    {
                        record.Attributes[i].Value = YCryptography.Decrypt_Aes(record.Attributes[i].Value, key);
                    }
                }
            }
        }
        public static Type GetRealType(this YFieldType fieldType)
        {
            return fieldType switch
            {
                YFieldType.Raw => typeof(short[]),
                YFieldType.Integer => typeof(long),
                YFieldType.Decimal => typeof(decimal),
                YFieldType.String => typeof(string),
                YFieldType.DateTime => typeof(DateTime),
                _ => null,
            };
        }
    }
}
