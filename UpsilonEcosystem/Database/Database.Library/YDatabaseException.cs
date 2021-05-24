using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    public class YDatabaseException : Exception 
    { 
        public YDatabaseException() : base() { }
        public YDatabaseException(string message) : base(message) { }
    }
    
    public class YDatabaseFileException : YDatabaseException
    {
        public string Filename { get; private set; }

        public YDatabaseFileException(string filename, string message) : base(message)
        {
            this.Filename = filename;
        }
    }

    public class YWrongDatabaseKeyException : YDatabaseFileException
    {
        public string Key { get; set; }
        public YWrongDatabaseKeyException(string filename, string key) :
            base(filename, $"Wrong Database key.\nFilename : {filename}\nKey : {key}") 
        {
            this.Key = key;
        }
    }

    public class YDatabaseXmlCorruptionException : YDatabaseFileException
    {
        public YDatabaseXmlCorruptionException(string filename, string message) :
            base(filename, $"Database Xml is corrupted.\nFilename : {filename}\n{message}") { }
    }

    public class YDatabaseClassesDefinitionException : YDatabaseException
    {
        public string TableName { get; set; }

        public YDatabaseClassesDefinitionException(string tableName, string message) : 
            base($"Database Classes has bad definition.\nTable name : '{tableName}'\n{message}")
        {
            this.TableName = tableName;
        }
    }
}
