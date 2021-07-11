using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Upsilon.Common.Library;
using Upsilon.Database.Library;

namespace Upsilon.Tools.YDBDesigner.Core
{
    public sealed class YDBDesignerCore
    {
        private string _filename = string.Empty;
        private string _key = string.Empty;
        private FileStream _file = null;

        public YTranslator Translator { get; private set; } = null;
        public List<YTableImage> Tables { get; private set; } = null;

        public YDBDesignerCore()
        {
            this.Translator = new("./lang/english.ulf", string.Empty);
        }

        public void Open(string filename, string key)
        {
            this.Close();

            this._filename = filename;
            this._key = key;
            string content = string.Empty;

            content = File.ReadAllText(this._filename);
            this._file = new FileStream(this._filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            XmlDocument document = new();
            try
            {
                document.LoadXml(content);
            }
            catch (Exception ex)
            {
                throw new YDatabaseXmlCorruptionException(this._filename, ex.Message);
            }

            XmlNode root = document.SelectSingleNode("//tables");

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

            this.Tables = new List<YTableImage>();

            foreach (XmlNode node in root.SelectNodes("./table"))
            {
                try
                {
                    this.Tables.Add(new(node, this._key));
                }
                catch (Exception ex)
                {
                    throw new YDatabaseXmlCorruptionException(this._filename, ex.Message);
                }
            }
        }

        public void Save()
        {
            this.SaveAs(this._filename, this._key);
        }

        public void SaveAs(string filename, string key)
        {
            this.Close();
        }

        public void Close()
        {
            if (this._file != null)
            {
                this._file.Close();
            }
        }

        public void AddTable(string tableName)
        {

        }

        public void RenameTable(string tableName, string newTableName)
        {
            YTableImage tableImage = this.Tables.Find(x => x.Name == tableName);
            tableImage.Name = newTableName;
        }

        public int DeleteTable(string tableName)
        {
            return this.Tables.RemoveAll(x => x.Name == tableName);
        }
    }
}
