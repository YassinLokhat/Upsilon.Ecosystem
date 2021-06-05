using System;
using Upsilon.Common.Library;

namespace Upsilon.Tools.YDBDesigner.Core
{
    public class YDBDesignerCore
    {
        private string _filename = string.Empty;
        private string _key = string.Empty;

        public YTranslator Translator = null;

        public YDBDesignerCore()
        {
            this.Translator = new("./lang/english.ulf", string.Empty);
        }

        public string[] Open(string filename, string key)
        {
            this._filename = filename;
            this._key = key;

            return new[] { "Table1", "Table2" };
        }

        public void Save()
        {

        }

        public void SaveAs(string filename, string key)
        {

        }

        public void Close()
        {

        }

        public void AddTable(string tableName)
        {

        }

        public void RenameTable(string tableName, string newTableName)
        {

        }

        public void DeleteTable(string tableName)
        {

        }

        public void RebuildInternalIndex(string tableName)
        {

        }

        public string[][] GetTableDefinition(string tableName)
        {
            return new[]
            {
                new[] {$"{tableName}.Field1", "Type1"},
                new[] {$"{tableName}.Field2", "Type2"},
                new[] {$"{tableName}.Field3", "Type3"},
            };
        }

        public string[][] GetTableData(string tableName)
        {
            return new[]
            {
                new[] {"1", "Field1_data1", "Field2_data1", "Field3_data1", },
                new[] {"2", "Field1_data2", "Field2_data2", "Field3_data2", },
            };
        }
    }
}
