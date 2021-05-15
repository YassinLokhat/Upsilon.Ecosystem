using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    public abstract class YTable
    {
        protected readonly List<YField> _Fields = new ();
        protected YDatabaseImage _Database = null;

        public YTable() { }

        public YTable(YDatabaseImage database)
        {
            this.SetDatabase(database);
        }

        public XmlNode XmlFields
        {
            get
            {
                XmlDocument document = new ();
                XmlNode fields = document.CreateNode(XmlNodeType.Element, "fields", "");

                foreach (YField field in this._Fields)
                {
                    fields.AppendChild(document.ImportNode(field.GetXmlNode(), true));
                }

                return fields;
            }
        }

        protected XmlNode _ComputeRecord(params object[] fields)
        {
            if (fields.Length != this._Fields.Count)
            {
                throw new YWrongRecordFieldCountException(this._Fields.Count);
            }

            XmlDocument document = new ();

            XmlNode node = document.CreateNode(XmlNodeType.Element, "record", string.Empty);

            for (int i = 0; i < this._Fields.Count; i++)
            {
                if (fields[i].GetType() != this._Fields[i].Type.GetRealType())
                {
                    throw new YInconsistentRecordFieldTypeException(fields[i].GetType(), this._Fields[i].Type.GetRealType());
                }
                XmlAttribute attribute = document.CreateAttribute($"field_{i}");
                attribute.Value = YField.GetStringFromObject(this._Fields[i].Type, fields[i]);
                
                node.Attributes.Append(attribute);
            }

            return node;
        }
        
        protected object[] _FillFields(XmlNode node)
        {
            List<object> fields = new ();

            for (int i = 0; i < this._Fields.Count; i++)
            {
                fields.Add(node.Attributes.IsNullOrWhiteSpace($"field_{i}") ? 
                    this._Fields[i].Default : 
                    YField.GetObjectFromString(this._Fields[i].Type, node.Attributes[$"field_{i}"].Value));
            }

            return fields.ToArray();
        }
       
        public void SetDatabase(YDatabaseImage database)
        {
            this._Database = database;
        }

        public abstract void InitializeToDefaultValues();

        public abstract XmlNode GetRecord();
       
        public abstract void SetRecord(XmlNode node);
    }
}
