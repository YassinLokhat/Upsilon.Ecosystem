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
    public abstract class YTable
    {
        protected YDatabaseImage _DatabaseImage = null;

        public long InternalIndex { get; set; } = 0;

        public XmlNode GetXmlRecord(string key)
        {
            XmlDocument document = new();

            XmlNode record = document.CreateNode(XmlNodeType.Element, "record", string.Empty);

            Dictionary<string, string> fieldsDico = this.GetFieldsDico(key);

            foreach (var field in fieldsDico)
            {
                XmlAttribute xmlField = document.CreateAttribute(field.Key);
                xmlField.Value = field.Value;
                record.Attributes.Append(xmlField);
            }

            return record;
        }

        public Dictionary<string, string> GetFieldsDico(string key)
        {
            Dictionary<string, string> fieldsDico = new();

            fieldsDico[$"field_0"] = this.InternalIndex.ToString().Cipher_Aes(key);

            PropertyInfo[] fieldsInfo = this.GetType().GetProperties()
               .Where(x => x.CustomAttributes
                   .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any())
               .ToArray();

            foreach (PropertyInfo fieldInfo in fieldsInfo)
            {
                YField yField = this._DatabaseImage.TablesDefinition[this.GetType().Name].Where(x => x.Name == fieldInfo.Name).FirstOrDefault();
                int i = this._DatabaseImage.TablesDefinition[this.GetType().Name].IndexOf(yField) + 1;

                fieldsDico[$"field_{i}"] = fieldInfo.GetValue(this).SerializeObject().Cipher_Aes(key);
            }

            return null;
        }
        
        public void SetRecord(XmlNode node, string key)
        {
            PropertyInfo[] fieldsInfo = this.GetType().GetProperties()
                .Where(x => x.CustomAttributes
                    .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any())
                .ToArray();

            this.InternalIndex = (long)node.Attributes[$"field_0"].Value.Uncipher_Aes(key).DeserializeObject(typeof(long));

            foreach (PropertyInfo fieldInfo in fieldsInfo)
            {
                YField yField = this._DatabaseImage.TablesDefinition[this.GetType().Name].Where(x => x.Name == fieldInfo.Name).FirstOrDefault();
                int i = this._DatabaseImage.TablesDefinition[this.GetType().Name].IndexOf(yField) + 1;

                object value = null;
                if (node.Attributes.Contains($"field_{i}"))
                {
                    value = node.Attributes[$"field_{i}"].Value.Uncipher_Aes(key).DeserializeObject(yField.Type);
                }

                fieldInfo.SetValue(this, value);
            }
        }

        public YTable(YDatabaseImage databaseImage)
        {
            this._DatabaseImage = databaseImage;
        }

        public static XmlNode GetEmptyTableNode(string tableName, string key)
        {
            XmlDocument document = new();

            XmlNode table = document.CreateNode(XmlNodeType.Element, "table", string.Empty);

            XmlAttribute xmlField = document.CreateAttribute($"name");
            xmlField.Value = tableName.Cipher_Aes(key);
            table.Attributes.Append(xmlField);

            XmlNode node = document.CreateNode(XmlNodeType.Element, "fields", string.Empty);
            table.AppendChild(node);
            node = document.CreateNode(XmlNodeType.Element, "records", string.Empty);
            table.AppendChild(node);

            return table;
        }
    }
}
