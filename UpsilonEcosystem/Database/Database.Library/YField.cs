using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Upsilon.Database.Library
{
    public enum YFieldType
    {
        Raw = 0,
        Integer = 1,
        Decimal = 2,
        String = 3,
        DateTime = 4,
    }

    public class YField
    {
        public string Name { get; set; }
        public YFieldType Type { get; set; }
        public object Default { get; set; }

        public YField(XmlNode node)
        {
            this.Name = node.Attributes["name"].Value;
            this.Type = (YFieldType)Enum.Parse(typeof(YFieldType), node.Attributes["type"].Value);
            this.Default = YField.GetObjectFromString(this.Type, node.Attributes["default"].Value);
        }

        public XmlNode GetXmlNode()
        {
            XmlDocument document = new ();

            XmlNode node = document.CreateNode(XmlNodeType.Element, "field", "");

            XmlAttribute attribute = document.CreateAttribute("name");
            attribute.Value = this.Name;
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("type");
            attribute.Value = this.Type.ToString();
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("default");
            attribute.Value = YField.GetStringFromObject(this.Default);
            node.Attributes.Append(attribute);

            return node;
        }

        public static Type GetRealType(YFieldType type)
        {
            return type switch
            {
                YFieldType.Raw => typeof(short[]),
                YFieldType.Integer => typeof(long),
                YFieldType.Decimal => typeof(decimal),
                YFieldType.String => typeof(string),
                YFieldType.DateTime => typeof(DateTime),
                _ => null,
            };
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
            else
            {
                return YFieldType.Raw;
            }
        }

        public static object GetObjectFromString(Type type, string str)
        {
            if (type == typeof(long))
            {
                return long.Parse(str);
            }
            else if (type == typeof(decimal))
            {
                return decimal.Parse(str);
            }
            else if (type == typeof(string))
            {
                return str;
            }
            else if (type == typeof(DateTime))
            {
                _ = long.TryParse(str, out long datetimeVal);
                return new DateTime(datetimeVal);
            }
            else if (type == typeof(short[]))
            {
                return str.ToCharArray().Select(x => (short)x).ToArray();
            }

            return null;
        }

        public static object GetObjectFromString(YFieldType type, string str)
        {
            Type realType = YField.GetRealType(type);
            return YField.GetObjectFromString(realType, str);
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
            else if (type == typeof(DateTime))
            {
                return ((DateTime)obj).Ticks.ToString();
            }
            else
            {
                return new string(((short[])obj).Select(x => (char)x).ToArray());
            }
        }
    }
}
