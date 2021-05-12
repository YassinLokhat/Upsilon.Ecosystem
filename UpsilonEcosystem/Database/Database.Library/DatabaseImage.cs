using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    public class DatabaseImage
    {
        #region Public Properties
        public string Key { get; set; }
        public Dictionary<string, Table> Tables { get; private set; }
        #endregion

        #region Private Properties
        private readonly string _FilePath;
        private FileStream _File = null;
        #endregion

        public DatabaseImage(string filePath, string key)
        {
            this._FilePath = filePath;
            this.Key = key;

            this.Tables = new Dictionary<string, Table>();

            this.Pull();
            this.Close();
        }

        #region Public Static Methods
        public static void CreateEmptUDatabaseFile(string filePath, string key)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tables key=\"" + key.GetMD5HashCode() + "\">\r\n</tables>";
            File.WriteAllText(filePath, xml);
        }
        #endregion

        #region Public Methods
        public void Pull()
        {
            string content = "";

            if (!File.Exists(this._FilePath))
            {
                _ThrowException(new FileNotFoundException());
            }

            while (this._File == null)
            {
                try
                {
                    content = File.ReadAllText(this._FilePath);
                    this._File = new FileStream(this._FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                Thread.Sleep(100);
            }

            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(content);
            }
            catch (Exception ex)
            {
                _ThrowException("Database Xml error : \n" + ex.Message);
            }

            try
            {
                _DecryptXml(document);
            }
            catch (Exception ex)
            {
                _ThrowException(ex.Message);
            }

            this.Tables.Clear();
            var tables = document.SelectNodes("//tables/table");
            foreach (XmlNode xmlTable in tables)
            {
                Table table = new Table(this, xmlTable);
                this.Tables[table.Name] = table;
            }
        }

        public void Push()
        {
            XmlDocument document = this._GetXmlDocument();
            try
            {
                _EncryptXml(document);
            }
            catch (Exception ex)
            {
                _ThrowException(ex.Message);
            }

            this.Close();

            TextWriter textWriter = new StreamWriter(this._FilePath);
            XmlTextWriter writer = new XmlTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1
            };
            document.WriteTo(writer);
            writer.Flush();
            textWriter.Close();
        }

        public void Sync()
        {
            Pull();
            Push();
        }

        public void Close()
        {
            if (this._File != null)
            {
                this._File.Close();
                this._File = null;
            }
        }

        public Table AddTable(Table table)
        {
            if (table == null
                || !table.IsValid())
            {
                throw new Exception("Adding a table whitch is not valid.");
            }

            if (this.Tables.ContainsKey(table.Name))
            {
                throw new Exception("Table '" + table.Name + "' already exists.");
            }

            this.Tables[table.Name] = table;

            return table;
        }

        public Table AddTable(string name)
        {
            Table table = new Table(this, name, new List<Field>());

            return this.AddTable(table);
        }

        public void RemoveTable(string tableName)
        {
            if (!this.Tables.ContainsKey(tableName))
            {
                throw new Exception("Table '" + tableName + "' not exists.");
            }

            this.Tables.Remove(tableName);
        }
        #endregion

        #region Private Methods
        private void _EncryptXml(XmlDocument document)
        {
            var root = document.SelectSingleNode("//tables");

            if (root.Attributes.IsNullOrWhiteSpace("key"))
            {
                _ThrowException("Database Xml error : Missing key attribute");
            }

            string hash = this.Key.GetMD5HashCode();

            if (hash != root.Attributes["key"].Value)
            {
                _ThrowException("Database Xml error : Wrong key");
            }

            if (String.IsNullOrWhiteSpace(this.Key))
            {
                return;
            }

            var tables = root.SelectNodes("./table");
            foreach (XmlNode table in tables)
            {
                for (int i = 0; i < table.Attributes.Count; i++)
                {
                    table.Attributes[i].Value = Cryptography.Encrypt_Aes(table.Attributes[i].Value, this.Key);
                }

                var fields = table.SelectNodes("./fields/field");
                foreach (XmlNode field in fields)
                {
                    for (int i = 0; i < field.Attributes.Count; i++)
                    {
                        field.Attributes[i].Value = Cryptography.Encrypt_Aes(field.Attributes[i].Value, this.Key);
                    }
                }

                var records = table.SelectNodes("./records/record");
                foreach (XmlNode record in records)
                {
                    for (int i = 0; i < record.Attributes.Count; i++)
                    {
                        record.Attributes[i].Value = Cryptography.Encrypt_Aes(record.Attributes[i].Value, this.Key);
                    }
                }
            }
        }

        private void _DecryptXml(XmlDocument document)
        {
            var root = document.SelectSingleNode("//tables");

            if (root.Attributes.IsNullOrWhiteSpace("key"))
            {
                throw new Exception("Database Xml error : Missing key attribute");
            }

            string hash = this.Key.GetMD5HashCode();

            if (hash != root.Attributes["key"].Value)
            {
                throw new Exception("Database Xml error : Wrong key");
            }

            if (String.IsNullOrWhiteSpace(this.Key))
            {
                return;
            }

            var tables = root.SelectNodes("./table");
            foreach (XmlNode table in tables)
            {
                for (int i = 0; i < table.Attributes.Count; i++)
                {
                    table.Attributes[i].Value = Cryptography.Decrypt_Aes(table.Attributes[i].Value, this.Key);
                }

                var fields = table.SelectNodes("./fields/field");
                foreach (XmlNode field in fields)
                {
                    for (int i = 0; i < field.Attributes.Count; i++)
                    {
                        field.Attributes[i].Value = Cryptography.Decrypt_Aes(field.Attributes[i].Value, this.Key);
                    }
                }

                var records = table.SelectNodes("./records/record");
                foreach (XmlNode record in records)
                {
                    for (int i = 0; i < record.Attributes.Count; i++)
                    {
                        record.Attributes[i].Value = Cryptography.Decrypt_Aes(record.Attributes[i].Value, this.Key);
                    }
                }
            }
        }

        private XmlDocument _GetXmlDocument()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tables key=\"" + this.Key.GetMD5HashCode() + "\">\r\n</tables>");
            XmlNode root = document.SelectSingleNode("/tables");
            XmlNode[] tables = this._GetXmlNodes(document);

            foreach (XmlNode table in tables)
            {
                root.AppendChild(table);
            }

            return document;
        }

        private XmlNode[] _GetXmlNodes(XmlDocument document)
        {
            List<XmlNode> tables = new List<XmlNode>();

            foreach (var table in this.Tables)
            {
                if (!table.Value.IsValid()
                    || table.Value.Fields.Count == 0)
                {
                    _ThrowException("Table definition not valid.");
                }
                tables.Add(table.Value.GetXmlNode(document));
            }

            return tables.ToArray();
        }

        private void _ThrowException(string message)
        {
            _ThrowException(new Exception(message));
        }

        private void _ThrowException(Exception exception)
        {
            this.Close();

            throw exception;
        }

        #endregion
    }
}
