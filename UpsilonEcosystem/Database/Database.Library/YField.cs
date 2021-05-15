using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool AutoIncrement { get; set; }

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
            attribute.Value = GetStringFromObject(this.Type, this.Default);
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("autoincrement");
            attribute.Value = this.AutoIncrement.ToString();
            node.Attributes.Append(attribute);

            return node;
        }

        public static object GetObjectFromString(YFieldType type, string str)
        {
            switch (type)
            {
                case YFieldType.Raw:
                    return str.ToCharArray().Select(x => (short)x).ToArray();
                case YFieldType.String:
                    return str;
                case YFieldType.Integer:
                    if (long.TryParse(str, out long longVal))
                    {
                        return longVal;
                    }
                    break;
                case YFieldType.Decimal:
                    if (decimal.TryParse(str, out decimal decimalVal))
                    {
                        return decimalVal;
                    }
                    break;
                case YFieldType.DateTime:
                    if (long.TryParse(str, out long datetimeVal))
                    {
                        return new DateTime(datetimeVal);
                    }
                    break;
            }

            return null;
        }

        public static string GetStringFromObject(YFieldType type, object obj)
        {
            if (obj.GetType() != type.GetRealType())
            {
                throw new YInconsistentRecordFieldTypeException(obj.GetType(), type.GetRealType());
            }

            return type switch
            {
                YFieldType.Raw => new string(((short[])obj).Select(x => (char)x).ToArray()),
                YFieldType.Integer or YFieldType.Decimal or YFieldType.String => obj.ToString(),
                YFieldType.DateTime => ((DateTime)obj).Ticks.ToString(),
                _ => null,
            };
        }
    }
}
