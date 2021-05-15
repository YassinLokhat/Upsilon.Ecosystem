using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    public abstract class YDatabaseImage
    {
        protected string _filename = string.Empty;
        protected string _key = string.Empty;

        protected Dictionary<string, List<YTable>> _Tables = null;

        private XmlDocument _document = null;
        private FileStream _file = null;

        public YDatabaseImage()
        {
            this._Tables = new Dictionary<string, List<YTable>>();
        }

        protected void _FillTablesFromXml<T>(List<T> table, string tableName) where T : YTable, new ()
        {
            foreach (XmlNode item in this._document.SelectNodes($"/tables//table[@name='{tableName}']/records/record"))
            {
                T t = new ();
                t.SetDatabase(this);
                t.SetRecord(item);
                table.Add(t);
            }
        }

        protected void _FillTablesFromLists<T>(List<T> table, string tableName) where T : YTable, new ()
        {
            this._Tables[tableName] = table.Select(x => (YTable)x).ToList();
        }
       
        public void SetKey(string key)
        {
            this.Pull();
            this._key = key;
            this.Push();
        }

        public void SaveAs(string filename, string key)
        {
            this.Pull();
            this._key = key;
            this._filename = filename;
            this.Push();
        }

        public void SaveAs(string filename)
        {
            this.SaveAs(filename, this._key);
        }

        public void Open(string filename, string key)
        {
            this._filename = filename;
            this._key = key;

            this.Sync();
        }

        public void Close()
        {
            if (this._file != null)
            {
                this._file.Close();
                this._file = null;
            }
        }

        public void Sync()
        {
            this.Pull();
            this.Push();
        }

        public virtual void Pull()
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
                        this._file = new FileStream(this._filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
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
                    throw new YDatabaseXmlCorruptionException(this._filename, this._key, ex.Message);
                }

                this._document.DecryptXml(this._filename, this._key);
            }
            catch
            {
                this.Close();
                throw;
            }
        }

        public virtual void Push()
        {
            XmlDocument document = new ();
            document.LoadXml(YDatabaseImage.GetEmptyXmlDocument(this._key));

            XmlNode tables = document.SelectSingleNode("/tables");

            try
            {
                foreach (var table in this._Tables)
                {
                    XmlNode tableNode = document.CreateNode(XmlNodeType.Element, "table", string.Empty);
                    XmlAttribute attribute = document.CreateAttribute("name");
                    attribute.Value = table.Key;
                    tableNode.Attributes.Append(attribute);

                    XmlNode fields = document.CreateNode(XmlNodeType.Element, "fields", string.Empty);
                    XmlNode records = document.CreateNode(XmlNodeType.Element, "records", string.Empty);

                    foreach (YTable item in table.Value)
                    {
                        if (fields.ChildNodes.Count == 0)
                        {
                            fields = item.XmlFields;
                        }

                        records.AppendChild(document.ImportNode(item.GetRecord(), true));
                    }

                    tableNode.AppendChild(document.ImportNode(fields, true));
                    tableNode.AppendChild(document.ImportNode(records, true));
                    tables.AppendChild(tableNode);
                }

                document.EncryptXml(this._filename, this._key);
            }
            finally
            {
                this.Close();
            }

            document.Save(this._filename);
        }

        public static string GetEmptyXmlDocument(string key)
        {
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tables key=\"{key.GetMD5HashCode()}\">\r\n</tables>";
        }
    }
}
