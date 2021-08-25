using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    /// <summary>
    /// Represent an <c><see cref="YDatabaseImage"/></c> general exception.
    /// </summary>
    public class YDatabaseException : Exception 
    {
        /// <summary>
        /// Create a new <c><see cref="YDatabaseException"/></c> with default values.
        /// </summary>
        public YDatabaseException() : base() { }

        /// <summary>
        /// Create a new <c><see cref="YDatabaseException"/></c> with a message.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        public YDatabaseException(string message) : base(message) { }
    }

    /// <summary>
    /// Represent an <c><see cref="YDatabaseImage"/></c> file exception.
    /// </summary>
    public class YDatabaseFileException : YDatabaseException
    {
        /// <summary>
        /// The file the exception refers to.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Create a new <c><see cref="YDatabaseFileException"/></c>.
        /// </summary>
        /// <param name="filename">The file the exception refers to.</param>
        /// <param name="message">The message of the exception.</param>
        public YDatabaseFileException(string filename, string message) : base(message)
        {
            this.Filename = filename;
        }
    }

    /// <summary>
    /// Represent an <c><see cref="YDatabaseImage"/></c> wrong key exception.
    /// </summary>
    public class YWrongDatabaseKeyException : YDatabaseFileException
    {
        /// <summary>
        /// The wrong key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Create a new <c><see cref="YWrongDatabaseKeyException"/></c>.
        /// </summary>
        /// <param name="filename">The file the exception refers to.</param>
        /// <param name="key">The wrong key.</param>
        public YWrongDatabaseKeyException(string filename, string key) :
            base(filename, $"Wrong Database key.\nFilename : {filename}\nKey : {key}") 
        {
            this.Key = key;
        }
    }

    /// <summary>
    /// Represent an <c><see cref="YDatabaseImage"/></c> corrupted XML exception.
    /// </summary>
    public class YDatabaseXmlCorruptionException : YDatabaseFileException
    {
        /// <summary>
        /// Create a new <c><see cref="YDatabaseXmlCorruptionException"/></c>.
        /// </summary>
        /// <param name="filename">The file the exception refers to.</param>
        /// <param name="message">The message of the exception.</param>
        public YDatabaseXmlCorruptionException(string filename, string message) :
            base(filename, $"Database Xml is corrupted.\nFilename : {filename}\n{message}") { }
    }

    /// <summary>
    /// Represent an <c><see cref="YDatabaseImage"/></c> classes definition exception.
    /// </summary>
    public class YDatabaseClassesDefinitionException : YDatabaseException
    {
        /// <summary>
        /// The table the exception refers to.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Create a new <c><see cref="YDatabaseClassesDefinitionException"/></c>.
        /// </summary>
        /// <param name="tableName">The table the exception refers to.</param>
        /// <param name="message">The message of the exception.</param>
        public YDatabaseClassesDefinitionException(string tableName, string message) : 
            base($"Database Classes has bad definition.\nTable name : '{tableName}'\n{message}")
        {
            this.TableName = tableName;
        }
    }
}
