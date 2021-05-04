using System;
using System.Linq;
using System.Xml;
using Common.Library;

namespace UDatabase.Library
{
    public enum FieldType
    {
        Raw = 0,
        Integer = 1,
        Decimal = 2,
        String = 3,
        DateTime = 4,
    }

    public class Field
    {
        public string Name { get; private set; }
        public FieldType Type { get; private set; }
        public object Default { get; private set; }
        public bool AutoIncrement { get; private set; }

        public Field(XmlNode xmlNode)
        {
            if (!xmlNode.Attributes.Contains("name")
                || !xmlNode.Attributes["name"].Value.IsIdentifiant())
            {
                throw new Exception("Database Xml error : Invalid Field name");
            }
            this.Name = xmlNode.Attributes["name"].Value;

            FieldType type = FieldType.Raw;
            if (!xmlNode.Attributes.Contains("type")
                || !Enum.TryParse(xmlNode.Attributes["type"].Value, out type))
            {
                throw new Exception("Database Xml error : Invalid Field type");
            }
            this.Type = type;

            bool autoIncrement = false;
            if (xmlNode.Attributes.Contains("autoincrement"))
            {
                bool.TryParse(xmlNode.Attributes["autoincrement"].Value, out autoIncrement);
            }
            this.AutoIncrement = autoIncrement;

            if (!xmlNode.Attributes.Contains("default"))
            {
                throw new Exception("Database Xml error : Missing Field default value");
            }

            this.Default = null;
            string defaukt = xmlNode.Attributes["default"].Value;
            switch (this.Type)
            {
                case FieldType.Raw:
                    this.Default = defaukt.Select(x => (short)x).ToArray();
                    break;
                case FieldType.String:
                    this.Default = defaukt;
                    break;
                case FieldType.Integer:
                    if (long.TryParse(defaukt, out long longVal))
                    {
                        this.Default = longVal;
                    }
                    break;
                case FieldType.Decimal:
                    if (decimal.TryParse(defaukt, out decimal decimalVal))
                    {
                        this.Default = decimalVal;
                    }
                    break;
                case FieldType.DateTime:
                    if (long.TryParse(defaukt, out long datetimeVal))
                    {
                        this.Default = new DateTime(datetimeVal);
                    }
                    break;
            }
            if (this.Default == null)
            {
                throw new Exception("Database Xml error : Invalid Field default value");
            }
        }

        public Field(string name, FieldType type, object defaultValue, bool autoIncrement)
        {
            if (!name.IsIdentifiant())
            {
                throw new Exception("Field definition not valid.");
            }

            this.Name = name;
            this.Type = type;
            this.Default = defaultValue;
            this.AutoIncrement = autoIncrement;
        }

        public object GetObject(string value)
        {
            object defaukt = null;

            switch (this.Type)
            {
                case FieldType.Raw:
                    defaukt = value.ToCharArray().Select(x => (short)x).ToArray();
                    break;
                case FieldType.String:
                    defaukt = value;
                    break;
                case FieldType.Integer:
                    if (long.TryParse(value, out long longVal))
                    {
                        defaukt = longVal;
                    }
                    break;
                case FieldType.Decimal:
                    if (decimal.TryParse(value, out decimal decimalVal))
                    {
                        defaukt = decimalVal;
                    }
                    break;
                case FieldType.DateTime:
                    if (long.TryParse(value, out long datetimeVal))
                    {
                        defaukt = new DateTime(datetimeVal);
                    }
                    break;
            }

            return defaukt;
        }

        public XmlNode GetXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateNode(XmlNodeType.Element, "field", "");

            XmlAttribute attribute = document.CreateAttribute("name");
            attribute.Value = this.Name;
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("type");
            attribute.Value = this.Type.ToString();
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("default");
            switch (this.Type)
            {
                case FieldType.Raw:
                    attribute.Value = new string(((short[])this.Default).Select(x => (char)x).ToArray());
                    break;
                case FieldType.Integer:
                case FieldType.Decimal:
                case FieldType.String:
                    attribute.Value = this.Default.ToString();
                    break;
                case FieldType.DateTime:
                    attribute.Value = ((DateTime)this.Default).Ticks.ToString();
                    break;
            }
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("autoincrement");
            attribute.Value = this.AutoIncrement.ToString();
            node.Attributes.Append(attribute);

            return node;
        }

        public bool IsValid()
        {
            return this.Name.IsIdentifiant()
                && this.Default != null;
        }
    }
}
