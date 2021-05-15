using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    public class YDatabaseException : Exception
    {
        public string Filename { get; private set; }
        public string Key { get; private set; }

        public YDatabaseException(string filename, string key, string message) : base(message)
        {
            this.Filename = filename;
            this.Key = key;
        }
    }

    public class YWrongDatabaseKeyException : YDatabaseException
    {
        public YWrongDatabaseKeyException(string filename, string key, string message = "") :
            base(filename, key, $"Wrong Database key.\nFilename : {filename}\nKey : {key}\n{message}") { }
    }

    public class YDatabaseXmlCorruptionException : YDatabaseException
    {
        public YDatabaseXmlCorruptionException(string filename, string key, string message = "") :
            base(filename, key, $"Database Xml get corrupted.\nFilename : {filename}\nKey : {key}\n{message}") { }
    }

    public class YWrongRecordFieldCountException : YDatabaseException
    {
        public int FieldCount { get; private set; }

        public YWrongRecordFieldCountException(int fieldCount, string message = "") : 
            base(string.Empty, string.Empty, $"Wrong record field count.\n{fieldCount} fields expected.\n{message}")
        {
            this.FieldCount = fieldCount;
        }
    }

    public class YInconsistentRecordFieldTypeException : YDatabaseException
    {
        public Type FieldType1 { get; private set; }
        public Type FieldType2 { get; private set; }

        public YInconsistentRecordFieldTypeException(Type fieldType1, Type fieldType2, string message = "") : 
            base(string.Empty, string.Empty, $"Inconsistent record field type.\n{fieldType1} not match with {fieldType2}.\n{message}")
        {
            this.FieldType1 = fieldType1;
            this.FieldType2 = fieldType2;
        }
    }
}
