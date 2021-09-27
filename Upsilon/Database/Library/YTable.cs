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
    /// <summary>
    /// Represent a Table.
    /// </summary>
    public abstract class YTable
    {
        /// <summary>
        /// The <c><see cref="YDatabaseImage"/></c> containing the table.
        /// </summary>
        protected YDatabaseImage _DatabaseImage = null;

        /// <summary>
        /// The internal index of the record.
        /// </summary>
        public long InternalIndex { get; set; } = 0;

        internal XmlNode _GetXmlRecord(string key)
        {
            YDebugTrace.TraceOn(new object[] { "key not logged" });

            XmlDocument document = new();

            XmlNode record = document.CreateNode(XmlNodeType.Element, "record", string.Empty);

            Dictionary<string, string> fieldsDico = this._GetFieldsDico(key);

            foreach (var field in fieldsDico)
            {
                XmlAttribute xmlField = document.CreateAttribute(field.Key);
                xmlField.Value = field.Value;
                record.Attributes.Append(xmlField);
            }

            return YDebugTrace.TraceOff(record);
        }

        internal Dictionary<string, string> _GetFieldsDico(string key)
        {
            YDebugTrace.TraceOn(new object[] { "key not logged" });

            Dictionary<string, string> fieldsDico = new();

            fieldsDico[$"field_0"] = this.InternalIndex.ToString().Cipher_Aes(key);

            PropertyInfo[] fieldsInfo = this.GetType().GetProperties()
               .Where(x => x.CustomAttributes
                   .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any())
               .ToArray();

            foreach (PropertyInfo fieldInfo in fieldsInfo)
            {
                YField yField = this._DatabaseImage._TablesDefinition[this.GetType().Name].Find(x => x.Name == fieldInfo.Name);
                int i = this._DatabaseImage._TablesDefinition[this.GetType().Name].IndexOf(yField) + 1;

                fieldsDico[$"field_{i}"] = fieldInfo.GetValue(this).SerializeObject().Cipher_Aes(key);
            }

            return YDebugTrace.TraceOff(fieldsDico);
        }
        
        internal void _SetRecord(XmlNode node, string key)
        {
            YDebugTrace.TraceOn(new object[] { node, "key not logged" });

            PropertyInfo[] fieldsInfo = this.GetType().GetProperties()
                .Where(x => x.CustomAttributes
                    .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any())
                .ToArray();

            this.InternalIndex = (long)node.Attributes[$"field_0"].Value.Uncipher_Aes(key).DeserializeObject(typeof(long));

            foreach (PropertyInfo fieldInfo in fieldsInfo)
            {
                YField yField = this._DatabaseImage._TablesDefinition[this.GetType().Name].Find(x => x.Name == fieldInfo.Name);
                int i = this._DatabaseImage._TablesDefinition[this.GetType().Name].IndexOf(yField) + 1;

                object value = null;
                if (node.Attributes.Contains($"field_{i}"))
                {
                    value = node.Attributes[$"field_{i}"].Value.Uncipher_Aes(key).DeserializeObject(yField.Type);
                }

                fieldInfo.SetValue(this, value);
            }

            YDebugTrace.TraceOff();
        }

        /// <summary>
        /// Create a new record.
        /// </summary>
        /// <param name="databaseImage">The database image containing the table.</param>
        public YTable(YDatabaseImage databaseImage)
        {
            YDebugTrace.TraceOn(new object[] { databaseImage });
            this._DatabaseImage = databaseImage;
            YDebugTrace.TraceOff();
        }

        internal static XmlNode _GetEmptyTableNode(string tableName, string key)
        {
            YDebugTrace.TraceOn(new object[] { tableName, "key not logged" });

            XmlDocument document = new();

            XmlNode table = document.CreateNode(XmlNodeType.Element, "table", string.Empty);

            XmlAttribute xmlField = document.CreateAttribute($"name");
            xmlField.Value = tableName.Cipher_Aes(key);
            table.Attributes.Append(xmlField);

            XmlNode node = document.CreateNode(XmlNodeType.Element, "fields", string.Empty);
            table.AppendChild(node);
            node = document.CreateNode(XmlNodeType.Element, "records", string.Empty);
            table.AppendChild(node);

            return YDebugTrace.TraceOff(table);
        }

        /// <summary>
        /// Check if the current record equals to the given object.
        /// </summary>
        /// <param name="item">The object to compare with.</param>
        /// <returns><c>true</c> or <c>false</c>.</returns>
        public new bool Equals(object item)
        {
            YDebugTrace.TraceOn(new object[] { item });

            if (item is not YTable yTable)
            {
                return YDebugTrace.TraceOff(base.Equals(item));
            }

            return YDebugTrace.TraceOff(yTable.InternalIndex == this.InternalIndex);
        }
    }
}
