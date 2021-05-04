using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common.Library;

namespace UDatabase.Library
{
    public class Record : Dictionary<Field, object>
    {
        public Record() : base() { }

        public Record(XmlNode xmlNode, List<Field> fields) : base()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                Field field = fields[i];

                this[field] = null;

                if (xmlNode.Attributes.Contains("field_" + i))
                {
                    this[field] = field.GetObject(xmlNode.Attributes["field_" + i].Value);
                }

                if (this[field] == null)
                {
                    this[field] = field.Default;
                }
            }
        }

        public XmlNode GetXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateNode(XmlNodeType.Element, "record", "");

            int i = 0;
            foreach (var field in this)
            {
                XmlAttribute attribute = document.CreateAttribute("field_" + i++);
                switch (field.Key.Type)
                {
                    case FieldType.Raw:
                        attribute.Value = new string(((short[])field.Value).Select(x => (char)x).ToArray());
                        break;
                    case FieldType.Integer:
                    case FieldType.Decimal:
                    case FieldType.String:
                        attribute.Value = field.Value.ToString();
                        break;
                    case FieldType.DateTime:
                        attribute.Value = ((DateTime)field.Value).Ticks.ToString();
                        break;
                }
                node.Attributes.Append(attribute);
            }

            return node;
        }
    }
}
