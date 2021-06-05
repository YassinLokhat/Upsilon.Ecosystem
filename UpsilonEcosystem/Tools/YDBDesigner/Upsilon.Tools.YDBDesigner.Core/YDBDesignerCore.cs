﻿using System;
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
            this.Translator = new("./lang/english.json", string.Empty);
        }

        public string[] Open(string filename, string key)
        {
            this._filename = filename;
            this._key = key;

            return null;
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
    }
}
