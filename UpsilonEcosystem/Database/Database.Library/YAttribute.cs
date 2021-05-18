using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    [AttributeUsage(AttributeTargets.Property)]
    public class YTableAttribute : Attribute
    {
        public string TableName { get; set; }

        public YTableAttribute(string tableName) : base()
        {
            this.TableName = tableName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class YFieldAttribute : Attribute 
    {
        public string FieldName { get; set; }

        public YFieldAttribute(string fieldName) : base()
        {
            this.FieldName = fieldName;
        }
    }
}
