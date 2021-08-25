using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    /// <summary>
    /// Represent an image of the physical database file.
    /// </summary>
    public abstract class YDatabaseImage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private static readonly double _REBUILD_INDEX_RATE = 0.90;

        private string _filename = string.Empty;
        private string _key = string.Empty;
        
        internal readonly Dictionary<string, List<YField>> _TablesDefinition = new();

        private XmlDocument _document = null;
        private FileStream _file = null;

        /// <summary>
        /// Create a new <c><see cref="YDatabaseImage"/></c>.
        /// </summary>
        /// <param name="filename">The physical database file.</param>
        /// <param name="key">The encryption key.</param>
        public YDatabaseImage(string filename, string key)
        {
            this._filename = filename;
            this._key = key;

            this.Pull(false);
        }

        /// <summary>
        /// Pull the physical database file to the database image.
        /// </summary>
        /// <param name="lockFile">Lock the physical database file or not. If the file is locked, it will not be accessible to other clients untill the next call of <c><see cref="Push"/></c>.</param>
        public void Pull(bool lockFile = true)
        {
            if (!File.Exists(this._filename))
            {
                File.WriteAllText(this._filename, YDatabaseImage.GetEmptyXmlDocument(this._key));
            }

            try
            {
                this._pullXmlDocument(lockFile);
                this._checkXmlHeader();
                this._TablesDefinition.Clear();

                PropertyInfo[] datasetsInfo = this.GetType().GetProperties()
                .Where(x => x.CustomAttributes
                    .Where(y => y.AttributeType == typeof(YDatasetAttribute)).Any()
                    && x.PropertyType.Name == typeof(YDataSet<YTable>).Name).ToArray();

                foreach (PropertyInfo datasetInfo in datasetsInfo)
                {
                    XmlNode root = _pullTable(datasetInfo, this._document.SelectSingleNode("//tables"));
                    _pullFields(datasetInfo, root.SelectSingleNode("./fields"));
                    _pullDataset(datasetInfo, root.SelectSingleNode("./records"));
                }
            }
            catch
            {
                this.Close();
                throw;
            }
        }

        /// <summary>
        /// Push the database image to the physical database file.
        /// </summary>
        public void Push()
        {
            List<string> indexToRebuild = new();

            PropertyInfo[] datasetsInfo = this.GetType().GetProperties()
                .Where(x => x.CustomAttributes
                    .Where(y => y.AttributeType == typeof(YDatasetAttribute)).Any()
                    && x.PropertyType.Name == typeof(YDataSet<YTable>).Name).ToArray();

            foreach (PropertyInfo datasetInfo in datasetsInfo)
            {
                Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                object dataset = datasetInfo.GetValue(this);
                YTable[] yTables = (YTable[])datasetInfo.PropertyType.GetMethod("_GetYTables", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(dataset, Array.Empty<object>());

                if (yTables.Any()
                    && (yTables.Length / yTables.Select(x => x.InternalIndex).Max() < YDatabaseImage._REBUILD_INDEX_RATE
                        || yTables.Select(x => x.InternalIndex).Max() > YDatabaseImage._REBUILD_INDEX_RATE * long.MaxValue))
                {
                    indexToRebuild.Add(dataType.Name);
                }

                XmlNode xmlRecords = this._document.SelectNodes("/tables/table")
                    .Cast<XmlNode>()
                    .Find(x => x.Attributes["name"].Value.Uncipher_Aes(this._key) == dataType.Name)
                    .SelectSingleNode("./records");

                foreach (YTable yTable in yTables)
                {
                    XmlNode xmlRecord = xmlRecords.ChildNodes
                        .Cast<XmlNode>()
                        .Find(x => x.Attributes["field_0"].Value.Uncipher_Aes(this._key) == yTable.InternalIndex.ToString());

                    if (xmlRecord == null)
                    {
                        xmlRecord = this._document.ImportNode(yTable._GetXmlRecord(this._key), true);
                        xmlRecords.AppendChild(xmlRecord);
                    }
                    else
                    {
                        Dictionary<string, string> fields = yTable._GetFieldsDico(this._key);

                        foreach (var field in fields)
                        {
                            if (!xmlRecord.Attributes.Contains(field.Key))
                            {
                                XmlAttribute attribute = this._document.CreateAttribute(field.Key);
                                xmlRecord.Attributes.Append(attribute);
                            }
                            xmlRecord.Attributes[field.Key].Value = field.Value;
                        }
                    }
                }

                long[] removedIndexes = (long[])datasetInfo.PropertyType.GetMethod("PopRemovedIndexes").Invoke(dataset, Array.Empty<object>());
                foreach (long index in removedIndexes)
                {
                    XmlNode xmlRecord = xmlRecords.ChildNodes
                        .Cast<XmlNode>()
                        .Find(x => x.Attributes["field_0"].Value.Uncipher_Aes(this._key) == index.ToString());

                    if (xmlRecord == null)
                    {
                        continue;
                    }

                    xmlRecords.RemoveChild(xmlRecord);
                }
            }

            if (indexToRebuild.Any())
            {
                this.RebuildInternalIndex(indexToRebuild.ToArray());
            }

            if (this._file != null)
            {
                this._file.SetLength(0);
                this._document.Save(this._file);
            }
            else
            {
                this._document.Save(this._filename);
            }

            this.Close();
        }

        /// <summary>
        /// Rebuild the internal index of each table in the given list.
        /// </summary>
        /// <param name="tables">The list of tables to rebuild.</param>
        public void RebuildInternalIndex(string[] tables)
        {
            if (this._document == null)
            {
                return;
            }

            foreach (string table in tables)
            {
                XmlNode xmlRecords = this._document.SelectNodes("/tables/table")
                    .Cast<XmlNode>()
                    .Find(x => x.Attributes["name"].Value.Uncipher_Aes(this._key) == table)
                    .SelectSingleNode("./records");

                long i = 0;
                foreach (XmlNode record in xmlRecords.ChildNodes)
                {
                    record.Attributes["field_0"].Value = (++i).ToString().Cipher_Aes(this._key);
                }
            }

            if (this._file != null)
            {
                this._file.SetLength(0);
                this._document.Save(this._file);
            }
            else
            {
                this._document.Save(this._filename);
            }

            this.Close();
        }

        /// <summary>
        /// Save the current database image to a new physical database file.
        /// </summary>
        /// <param name="filename">The new physical database file.</param>
        /// <param name="key">The new encryption key.</param>
        public void SaveAs(string filename, string key)
        {
            this.Pull(false);

            if (string.IsNullOrEmpty(filename))
            {
                filename = this._filename;
            }

            XmlNode tables = this._document.SelectSingleNode("/tables");
            tables.Attributes["key"].Value = key.GetUpsilonHashCode();

            foreach (XmlNode table in tables.SelectNodes("./table"))
            {
                foreach (XmlAttribute attribute in table.Attributes)
                {
                    attribute.Value = attribute.Value.Uncipher_Aes(this._key).Cipher_Aes(key);
                }

                foreach (XmlNode field in table.SelectNodes("./fields/field"))
                {
                    foreach (XmlAttribute attribute in field.Attributes)
                    {
                        attribute.Value = attribute.Value.Uncipher_Aes(this._key).Cipher_Aes(key);
                    }
                }

                foreach (XmlNode record in table.SelectNodes("./records/record"))
                {
                    foreach (XmlAttribute attribute in record.Attributes)
                    {
                        attribute.Value = attribute.Value.Uncipher_Aes(this._key).Cipher_Aes(key);
                    }
                }
            }

            this._key = key;
            this._filename = filename;

            this.Push();
        }

        /// <summary>
        /// Close the database image. If the physical database file is locked, it will be unlocked but the database image will not be pushed.
        /// </summary>
        public void Close()
        {
            if (this._file != null)
            {
                this._file.Close();
                this._file = null;
            }
        }

        /// <summary>
        /// Get an Xml code with an empty physical database file structure.
        /// </summary>
        /// <param name="key">The encryption key.</param>
        /// <returns>The Xml code.</returns>
        public static string GetEmptyXmlDocument(string key)
        {
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tables key=\"{key.GetUpsilonHashCode()}\">\r\n</tables>";
        }

        private void _pullXmlDocument(bool lockFile)
        {
            string content;
            while (true)
            {
                try
                {
                    content = File.ReadAllText(this._filename);
                    if (lockFile)
                    {
                        this._file = new FileStream(this._filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                Thread.Sleep(100);
            }

            this._document = new();

            try
            {
                this._document.LoadXml(content);
            }
            catch (Exception ex)
            {
                throw new YDatabaseXmlCorruptionException(this._filename, ex.Message);
            }
        }

        private void _checkXmlHeader()
        {
            XmlNode root = this._document.SelectSingleNode("//tables");

            if (root == null
                || root.Attributes.IsNullOrWhiteSpace("key"))
            {
                throw new YDatabaseXmlCorruptionException(this._filename, "'tables' node definition is not valid.");
            }

            string hash = this._key.GetUpsilonHashCode();

            if (hash != root.Attributes["key"].Value)
            {
                throw new YWrongDatabaseKeyException(this._filename, this._key);
            }
        }

        private XmlNode _pullTable(PropertyInfo datasetInfo, XmlNode root)
        {
            Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

            XmlNode tableXml = root.SelectNodes("./table").Cast<XmlNode>()
               .Find(n => n.Attributes.Contains("name") && n.Attributes["name"].Value.Uncipher_Aes(this._key) == dataType.Name);

            if (tableXml == null)
            {
                tableXml = this._document.ImportNode(YTable._GetEmptyTableNode(dataType.Name, this._key), true);
                root.AppendChild(tableXml);
            }

            return tableXml;
        }

        private void _pullFields(PropertyInfo datasetInfo, XmlNode root)
        {
            Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

            this._TablesDefinition[dataType.Name] = new();
            PropertyInfo[] fieldsInfo = dataType.GetProperties()
            .Where(x => x.CustomAttributes
                .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any()).ToArray();

            foreach (XmlNode fieldXml in root.SelectNodes("./field"))
            {
                this._TablesDefinition[dataType.Name].Add(new(this._filename, this._key, dataType.Name, fieldXml));
            }

            foreach (PropertyInfo fieldInfo in fieldsInfo)
            {
                XmlNode fieldXml = root.SelectNodes("./field").Cast<XmlNode>()
                   .Find(n => n.Attributes.Contains("name") && n.Attributes["name"].Value.Uncipher_Aes(this._key) == fieldInfo.Name);

                if (fieldXml == null)
                {
                    fieldXml = this._document.ImportNode(YField.GetFieldNode(fieldInfo, this._key), true);
                    root.AppendChild(fieldXml);
                    this._TablesDefinition[dataType.Name].Add(new(this._filename, this._key, dataType.Name, fieldXml));
                }

                YField yField = this._TablesDefinition[dataType.Name].Find(x => x.Name == fieldInfo.Name);
                if (yField.Type != fieldInfo.PropertyType)
                {
                    throw new YDatabaseClassesDefinitionException(dataType.Name, $"Type '{fieldInfo.PropertyType}' does not match with '{yField.Type}' type for the '{fieldInfo.Name}' field.");
                }
            }
        }

        private void _pullDataset(PropertyInfo datasetInfo, XmlNode root)
        {
            Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

            object dataset = Activator.CreateInstance(datasetInfo.PropertyType, Array.Empty<object>());
            datasetInfo.SetValue(this, dataset);

            foreach (XmlNode recordXml in root.SelectNodes("./record"))
            {
                if (recordXml.Attributes.IsNullOrWhiteSpace($"field_0"))
                {
                    throw new YDatabaseXmlCorruptionException(this._filename, $"A record does not InternalIndex in the table '{dataType.Name}'");
                }

                YTable recordClass = (YTable)Activator.CreateInstance(dataType, new object[] { this });
                recordClass._SetRecord(recordXml, this._key);

                var add = datasetInfo.PropertyType.GetMethod("Add");
                add.Invoke(dataset, new object[] { recordClass });
            }
        }
    }
}
