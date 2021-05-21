using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    public abstract class YDatabaseImage
    {
        private string _filename = string.Empty;
        private string _key = string.Empty;

        private XmlDocument _document = null;
        private FileStream _file = null;

        //private readonly Dictionary<string, List<YField>> _tablesDefinition = new();

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

                var root = this._document.SelectSingleNode("//tables");

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

                Dictionary<string, List<YField>> tablesDefinition = new ();

                foreach (XmlNode tableXml in root.SelectNodes("./table"))
                {
                    for (int i = 0; i < tableXml.Attributes.Count; i++)
                    {
                        tableXml.Attributes[i].Value = YCryptography.Uncipher_Aes(tableXml.Attributes[i].Value, this._key);
                    }

                    if (tableXml.Attributes.IsNullOrWhiteSpace("name"))
                    {
                        throw new YDatabaseXmlCorruptionException(this._filename, $"Attribute 'name' is missing in a table definition.");
                    }

                    string tableName = tableXml.Attributes["name"].Value;

                    if (!tableName.IsIdentifiant())
                    {
                        throw new YDatabaseXmlCorruptionException(this._filename, $"The '{tableName}' table name is not valid.");
                    }

                    tablesDefinition[tableName] = new List<YField>();

                    var fields = tableXml.SelectNodes("./fields/field");
                    if (fields.Count == 0)
                    {
                        throw new YDatabaseXmlCorruptionException(this._filename, $"'{tableName}' table  does not have field definition.");
                    }

                    foreach (XmlNode field in fields)
                    {
                        tablesDefinition[tableName].Add(new(this._filename, tableName, field, this._key));
                    }

                    PropertyInfo datasetInfo = this.GetType().GetProperties()
                        .Where(x =>x.CustomAttributes
                            .Where(y => y.AttributeType == typeof(YDatasetAttribute)
                                && y.ConstructorArguments.First().Value.ToString() == tableName).Any()
                            && x.PropertyType.Name == typeof(YDataSet<YTable>).Name).FirstOrDefault();

                    if (datasetInfo == null)
                    {
                        throw new YDatabaseClassesDefinitionException(tableName, $"Missing [YTable(\"{tableName}\")] attribute on YDataset<YTable> property in the class '{this.GetType().Name}'.");
                    }

                    Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                    foreach (YField yField in tablesDefinition[tableName])
                    {
                        PropertyInfo fieldPropertyInfo = dataType.GetProperties()
                            .Where(x => x.CustomAttributes
                                .Where(y => y.AttributeType == typeof(YFieldAttribute)
                                    && y.ConstructorArguments[0].Value.ToString() == yField.Name).Any())
                            .FirstOrDefault();

                        if (fieldPropertyInfo == null)
                        {
                            throw new YDatabaseClassesDefinitionException(tableName, $"Missing [YField(\"{tableName}\")] attribute on a property in the class '{dataType.Name}'.");
                        }

                        if (YField.GetYFieldType(fieldPropertyInfo.PropertyType) != yField.Type)
                        {
                            throw new YDatabaseClassesDefinitionException(tableName, $"Type of '{fieldPropertyInfo.Name}' does not match with '{yField.Type}' type.");
                        }
                    }

                    object dataset = datasetInfo.GetValue(this);
                    var clear = datasetInfo.PropertyType.GetMethod("Clear");
                    clear.Invoke(dataset, Array.Empty<object>());

                    foreach (XmlNode recordXml in tableXml.SelectNodes("./records/record"))
                    {
                        for (int i = 0; i < tablesDefinition[tableName].Count; i++)
                        {
                            recordXml.Attributes[i].Value = YCryptography.Uncipher_Aes(recordXml.Attributes[i].Value, this._key);

                            if (recordXml.Attributes[i].Name == $"field_{i}")
                            {
                                try
                                {
                                    object obj = YField.GetObjectFromString(tablesDefinition[tableName][i].Type, recordXml.Attributes[$"field_{i}"].Value);
                                }
                                catch
                                {
                                    throw new YDatabaseXmlCorruptionException(this._filename, $"'field_{i}' of a record does not match with '{tableName}' table field definition.");
                                }
                            }
                        }

                        YTable recordClass = (YTable)Activator.CreateInstance(dataType, new object[] { this });
                        recordClass.SetRecord(tablesDefinition[tableName].ToArray(), recordXml);

                        var add = datasetInfo.PropertyType.GetMethod("Add");
                        add.Invoke(dataset, new object[] { recordClass });
                    }
                }
            }
            catch
            {
                this.Close();
                throw;
            }
        }

        public void Push()
        {
            this._document = new();
            this._document.LoadXml(YDatabaseImage.GetEmptyXmlDocument(this._key));

            PropertyInfo[] datasetsInfo = this.GetType().GetProperties()
                .Where(x => x.CustomAttributes
                    .Where(y => y.AttributeType == typeof(YDatasetAttribute)).Any()
                    && x.PropertyType.Name == typeof(YDataSet<YTable>).Name).ToArray();

            XmlNode tables = this._document.SelectSingleNode("/tables");
            try
            {
                foreach (PropertyInfo datasetInfo in datasetsInfo)
                {
                    string tableName = datasetInfo.CustomAttributes.First().ConstructorArguments.First().Value.ToString();
                    Type dataType = datasetInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                    XmlNode tableNode = this._document.CreateNode(XmlNodeType.Element, "table", string.Empty);
                    XmlAttribute attribute = this._document.CreateAttribute("name");
                    attribute.Value = YCryptography.Cither_Aes(tableName, this._key);
                    tableNode.Attributes.Append(attribute);

                    XmlNode fields = this._document.CreateNode(XmlNodeType.Element, "fields", string.Empty);

                    PropertyInfo[] fieldsProperties = dataType.GetProperties()
                                    .Where(x => x.CustomAttributes
                                        .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any())
                                    .ToArray();

                    foreach (PropertyInfo fieldProp in fieldsProperties)
                    {
                        fields.AppendChild(this._document.ImportNode(YField.GetXmlNode(fieldProp, this._key), true));
                    }

                    tableNode.AppendChild(fields);

                    object dataset = datasetInfo.GetValue(this);
                    var getYTables = datasetInfo.PropertyType.GetMethod("GetYTables");
                    YTable[] classRecords = (YTable[])getYTables.Invoke(dataset, Array.Empty<object>());

                    XmlNode records = this._document.CreateNode(XmlNodeType.Element, "records", string.Empty);
                    foreach (YTable classRecord in classRecords)
                    {
                        records.AppendChild(this._document.ImportNode(classRecord.GetRecord(this._key), true));
                    }
                    tableNode.AppendChild(records);

                    tables.AppendChild(tableNode);
                }
            }
            finally
            {
                this.Close();
            }

            this._document.Save(this._filename);
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
