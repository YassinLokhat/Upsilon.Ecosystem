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
    public abstract class YDatabaseImage
    {
        //private readonly double _REBUILD_INDEX_RATE = 0.90;

        private string _filename = string.Empty;
        private string _key = string.Empty;
        
        public readonly Dictionary<string, List<YField>> TablesDefinition = new();

        private XmlDocument _document = null;
        private FileStream _file = null;

        public YDatabaseImage(string filename, string key)
        {
            this._filename = filename;
            this._key = key;

            this.Pull(false);
        }

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
                this.TablesDefinition.Clear();

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

            string hash = this._key.GetMD5HashCode();

            if (hash != root.Attributes["key"].Value)
            {
                throw new YWrongDatabaseKeyException(this._filename, this._key);
            }
        }

        private XmlNode _pullTable(PropertyInfo datasetInfo, XmlNode root)
        {
            Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

            XmlNode tableXml = root.SelectNodes("./table").Cast<XmlNode>()
               .Where(n => n.Attributes.Contains("name") && n.Attributes["name"].Value.Uncipher_Aes(this._key) == dataType.Name).FirstOrDefault();

            if (tableXml == null)
            {
                tableXml = this._document.ImportNode(YTable.GetEmptyTableNode(dataType.Name, this._key), true);
                root.AppendChild(tableXml);
            }

            return tableXml;
        }

        private void _pullFields(PropertyInfo datasetInfo, XmlNode root)
        {
            Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

            this.TablesDefinition[dataType.Name] = new();
            PropertyInfo[] fieldsInfo = dataType.GetProperties()
            .Where(x => x.CustomAttributes
                .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any()).ToArray();

            foreach (PropertyInfo fieldInfo in fieldsInfo)
            {
                XmlNode fieldXml = root.SelectNodes("./field").Cast<XmlNode>()
                   .Where(n => n.Attributes.Contains("name") && n.Attributes["name"].Value.Uncipher_Aes(this._key) == fieldInfo.Name).FirstOrDefault();

                if (fieldXml == null)
                {
                    fieldXml = this._document.ImportNode(YField.GetFieldNode(fieldInfo, this._key), true);
                    root.AppendChild(fieldXml);
                }

                YField yField = new(this._filename, this._key, dataType.Name, fieldXml);
                if (yField.Type != fieldInfo.PropertyType)
                {
                    throw new YDatabaseClassesDefinitionException(dataType.Name, $"Type '{fieldInfo.PropertyType}' does not match with '{yField.Type}' type for the '{fieldInfo.Name}' field.");
                }

                this.TablesDefinition[dataType.Name].Add(yField);
            }
        }

        private void _pullDataset(PropertyInfo datasetInfo, XmlNode root)
        {
            Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

            object dataset = Activator.CreateInstance(datasetInfo.PropertyType, new object[] { this });
            datasetInfo.SetValue(this, dataset);

            foreach (XmlNode recordXml in root.SelectNodes("./record"))
            {
                if (recordXml.Attributes.IsNullOrWhiteSpace($"field_0"))
                {
                    throw new YDatabaseXmlCorruptionException(this._filename, $"A record does not InternalIndex in the table '{dataType.Name}'");
                }

                YTable recordClass = (YTable)Activator.CreateInstance(dataType, new object[] { this });
                recordClass.SetRecord(recordXml, this._key);

                var add = datasetInfo.PropertyType.GetMethod("Add");
                add.Invoke(dataset, new object[] { recordClass });
            }
        }

        public void Push()
        {
            PropertyInfo[] datasetsInfo = this.GetType().GetProperties()
                .Where(x => x.CustomAttributes
                    .Where(y => y.AttributeType == typeof(YDatasetAttribute)).Any()
                    && x.PropertyType.Name == typeof(YDataSet<YTable>).Name).ToArray();

            foreach (PropertyInfo datasetInfo in datasetsInfo)
            {
                Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                object dataset = datasetInfo.GetValue(this);
                var getYTables = datasetInfo.PropertyType.GetMethod("GetYTables");
                YTable[] yTables = (YTable[])getYTables.Invoke(dataset, Array.Empty<object>());
                XmlNode table = this._document.SelectNodes("/tables/table")
                    .Cast<XmlNode>()
                    .Where(x => x.Attributes["name"].Value.Uncipher_Aes(this._key) == dataType.Name)
                    .FirstOrDefault();
                XmlNode xmlRecords = table.SelectSingleNode("./records");

                foreach (YTable yTable in yTables)
                {
                    XmlNode xmlRecord = xmlRecords.ChildNodes
                        .Cast<XmlNode>()
                        .Where(x => x.Attributes["field_0"].Value.Uncipher_Aes(this._key) == yTable.InternalIndex.ToString())
                        .FirstOrDefault();

                    if (xmlRecord == null)
                    {
                        xmlRecord = this._document.ImportNode(yTable.GetXmlRecord(this._key), true);
                        xmlRecords.AppendChild(xmlRecord);
                    }
                    else
                    {
                        Dictionary<string, string> fields = yTable.GetFieldsDico(this._key);

                        foreach (var field in fields)
                        {
                            xmlRecord.Attributes[field.Key].Value = field.Value;
                        }
                    }
                }
            }

            if (this._file != null)
            {
                this._document.Save(this._file);
            }
            else
            {
                this._document.Save(this._filename);
            }

            this.Close();
        }

        public void RebuildInternalIndex()
        {

        }

        public void ChangeKey(string key)
        {
            this.Pull();
            this._key = key;
            this.Push();
        }

        public void SaveAs(string filename, string key)
        {
            this.Pull(false);
            this._key = key;
            this._filename = filename;
            this.Push();
        }

        public void SaveAs(string filename)
        {
            this.SaveAs(filename, this._key);
        }

        public void Close()
        {
            if (this._file != null)
            {
                this._file.Close();
                this._file = null;
            }
        }

        public static string GetEmptyXmlDocument(string key)
        {
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tables key=\"{key.GetMD5HashCode()}\">\r\n</tables>";
        }
    }
}
