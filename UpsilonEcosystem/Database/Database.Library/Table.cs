using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    public class Table
    {
        #region Public Properties
        public string Name { get; private set; }
        public List<Field> Fields { get; private set; }
        public List<Record> Records { get; private set; }
        #endregion

        #region Private Properties
        private readonly DatabaseImage _Database = null;
        #endregion

        public Table(DatabaseImage database, XmlNode xmlNode)
        {
            this._Database = database;
            if (this._Database == null)
            {
                _ThrowException("Database should not be null.");
            }

            this.Fields = new List<Field>();
            this.Records = new List<Record>();

            if (!xmlNode.Attributes.Contains("name")
                || !xmlNode.Attributes["name"].Value.IsIdentifiant())
            {
                _ThrowException("Database Xml error : Invalid Table name");
            }

            this.Name = xmlNode.Attributes["name"].Value;

            var fields = xmlNode.SelectNodes(".//fields/field");
            foreach (XmlNode xmlField in fields)
            {
                Field field = new Field(xmlField);
                this.Fields.Add(field);
            }

            var records = xmlNode.SelectNodes(".//records/record");
            foreach (XmlNode xmlRecords in records)
            {
                Record record = new Record(xmlRecords, this.Fields);
                this.Records.Add(record);
            }
        }

        public Table(DatabaseImage database, string name, List<Field> fields, List<Record> records = null)
        {
            this._Database = database;
            if (this._Database == null)
            {
                _ThrowException("Database should not be null.");
            }

            if (!name.IsIdentifiant()
                || fields == null)
            {
                _ThrowException("Table definition not valid.");
            }

            this.Name = name;
            this.Fields = fields;
            this.Records = records;

            if (this.Records == null)
            {
                this.Records = new List<Record>();
            }
        }

        #region Public Methods
        public XmlNode GetXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateNode(XmlNodeType.Element, "table", "");
            
            XmlAttribute attribute = document.CreateAttribute("name");
            attribute.Value = this.Name;
            node.Attributes.Append(attribute);

            XmlNode fields = document.CreateNode(XmlNodeType.Element, "fields", "");
            foreach (var field in this.Fields)
            {
                fields.AppendChild(field.GetXmlNode(document));
            }
            node.AppendChild(fields);

            XmlNode records = document.CreateNode(XmlNodeType.Element, "records", "");
            foreach (var record in this.Records)
            {
                records.AppendChild(record.GetXmlNode(document));
            }
            node.AppendChild(records);

            return node;
        }

        public bool IsValid()
        {
            return this.Name.IsIdentifiant()
                && this.Fields != null
                && this.Records != null; 
        }

        public Field AddField(Field field)
        {
            if (field == null
                || !field.IsValid())
            {
                _ThrowException("Adding a field whitch is not valid.");
            }

            if (this.Fields.Where(x => x.Name == field.Name).Count() != 0)
            {
                _ThrowException("Field '" + field.Name + "' already exists.");
            }

            this.Fields.Add(field);

            foreach (Record record in this.Records)
            {
                record[field] = field.Default;
            }

            return field;
        }

        public Field AddField(string name, FieldType type, object defaultValue, bool autoIncrement = false)
        {
            Field field = new Field(name, type, defaultValue, autoIncrement);

            return this.AddField(field);
        }

        public void RemoveField(string fieldName)
        {
            Field field = this.Fields.Find(x => x.Name == fieldName);
            if (field == null)
            {
                _ThrowException("Field '" + field + "' not exists.");
            }

            this.Fields.Remove(field);
        }

        public Record AddRecord(Record record)
        {
            if (record == null)
            {
                _ThrowException("Adding a record whitch is not valid.");
            }

            if (this.Records.Contains(record))
            {
                _ThrowException("Record already exists.");
            }

            this.Records.Add(record);

            return record;
        }

        public Record AddRecord(params object[] fields)
        {
            Record record = new Record();

            if (fields.Length != this.Fields.Count)
            {
                _ThrowException("Wrong record field count. " + this.Fields.Count + " fields expected.");
            }

            for (int i = 0; i < this.Fields.Count; i++)
            {
                Field field = this.Fields[i];

                if (fields[i] == null)
                {
                    fields[i] = field.Default;

                    if (field.AutoIncrement
                        && this.Records.Count != 0)
                    {
                        string lastIndex = this.Records.Select(x => x[field]).Max().ToString();
                        switch (field.Type)
                        {
                            case FieldType.Integer:
                                long.TryParse(lastIndex, out long integer);
                                fields[i] = integer + 1;
                                break;
                            case FieldType.Decimal:
                                decimal.TryParse(lastIndex, out decimal dec);
                                fields[i] = dec + 1;
                                break;
                            case FieldType.DateTime:
                                fields[i] = DateTime.Now;
                                break;
                            default:
                                _ThrowException("" + field.Type + " type cannot be AutoIncrement.");
                                break;
                        }
                    }
                }
                else if (((FieldType)fields[i].GetType().GetFieldType()) != field.Type)
                {
                    _ThrowException("Wrong record field type. " + field.Name + " expect " + field.Type + " type.");
                }

                record[field] = fields[i];
            }

            return this.AddRecord(record);
        }

        public void RemoveRecord(Record record)
        {
            if (!this.Records.Contains(record))
            {
                throw new Exception("Record not exists.");
            }

            this.Records.Remove(record);
        }
        #endregion

        #region Private Methods
        private void _ThrowException(string message)
        {
            if (this._Database != null)
            {
                this._Database.Close();
            }

            throw new Exception(message);
        }
        #endregion
    }
}
