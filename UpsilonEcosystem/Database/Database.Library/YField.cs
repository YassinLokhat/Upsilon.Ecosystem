using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    public enum YFieldType
    {
        Raw = 0,
        Boolean = 1,
        Integer = 2,
        Decimal = 3,
        String = 4,
        DateTime = 5,
    }

    public class YField
    {
        public string Name { get; set; }
        public YFieldType Type { get; set; }
        public object Default { get; set; }

        public YField(string filename, string tablename, XmlNode node, string key)
        {
            try
            {
                this.Name = YCryptography.Uncipher_Aes(node.Attributes["name"].Value, key);
                this.Type = (YFieldType)Enum.Parse(typeof(YFieldType), YCryptography.Uncipher_Aes(node.Attributes["type"].Value, key));
                this.Default = YField.GetObjectFromString(this.Type, YCryptography.Uncipher_Aes(node.Attributes["default"].Value, key));
                this.Default.ToString();

                if (!this.Name.IsIdentifiant())
                {
                    throw new();
                }
            }
            catch
            {
                throw new YDatabaseXmlCorruptionException(filename, $"A field definition is not valid in '{tablename}' table node.");
            }
        }

        public static XmlNode GetXmlNode(PropertyInfo field, string key)
        {
            XmlDocument document = new ();
            string fieldName = (string)field.CustomAttributes.First().ConstructorArguments[0].Value;
            string fieldDefault = (string)field.CustomAttributes.First().ConstructorArguments[1].Value;

            XmlNode node = document.CreateNode(XmlNodeType.Element, "field", "");

            XmlAttribute attribute = document.CreateAttribute("name");
            attribute.Value = YCryptography.Cither_Aes(fieldName, key);
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("type");
            attribute.Value = YCryptography.Cither_Aes(YField.GetYFieldType(field.PropertyType).ToString(), key);
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("default");
            attribute.Value = YCryptography.Cither_Aes(YField.GetStringFromObject(fieldDefault), key);
            node.Attributes.Append(attribute);

            return node;
        }

        public static object GetObjectFromString(YFieldType type, string str)
        {
            switch (type)
            {
                case YFieldType.Raw:
                    return str.ToCharArray().Select(x => (short)x).ToArray();
                case YFieldType.Boolean:
                    return !string.IsNullOrEmpty(str);
                case YFieldType.Integer:
                    return long.Parse(str);
                case YFieldType.Decimal:
                    return decimal.Parse(str);
                case YFieldType.String:
                    return str;
                case YFieldType.DateTime:
                    _ = long.TryParse(str, out long datetimeVal);
                    return new DateTime(datetimeVal);
                default:
                    return null;
            }
        }

        public static string GetStringFromObject(object obj)
        {
            Type type = obj.GetType();

            if (type == typeof(long)
                || type == typeof(decimal)
                || type == typeof(string))
            {
                return obj.ToString();
            }
            else if (type == typeof(bool))
            {
                return ((bool)obj) ? "true" : string.Empty;
            }
            else if (type == typeof(DateTime))
            {
                return ((DateTime)obj).Ticks.ToString();
            }
            else
            {
                return new string(((short[])obj).Select(x => (char)x).ToArray());
            }
        }

        public static YFieldType GetYFieldType(Type type)
        {
            if (type == typeof(long))
            {
                return YFieldType.Integer;
            }
            else if (type == typeof(decimal))
            {
                return YFieldType.Decimal;
            }
            else if (type == typeof(string))
            {
                return YFieldType.String;
            }
            else if (type == typeof(DateTime))
            {
                return YFieldType.DateTime;
            }
            else if (type == typeof(bool))
            {
                return YFieldType.Boolean;
            }
            else
            {
                return YFieldType.Raw;
            }
        }
    }
}
