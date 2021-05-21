using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Upsilon.Database.Library.UnitTests
{
    public class Database : YDatabaseImage
    {
        [YDataset(datasetType: "PLATFORM")]
        public YDataSet<PLATFORM> PLATFORMs { get; private set; } = new YDataSet<PLATFORM>();

        [YDataset(datasetType: "LOGIN")]
        public YDataSet<LOGIN> LOGINs { get; private set; } = new YDataSet<LOGIN>();

        public Database(string filename, string key) : base(filename, key) { }
    }

    [YTable(tableName: "PLATFORM")]
    public class PLATFORM : YTable
    {
        [YField(fieldName: "Label", defaulValue: "")]
        public string Label0 { get; set; }

        [YField(fieldName: "Url", defaulValue: "")]
        public string Url0 { get; set; }

        [YField(fieldName: "BirthDay", defaulValue: "0")]
        public DateTime BirthDay0 { get; set; }

        public PLATFORM(Database database) : base(database) { }
    }

    [YTable(tableName: "LOGIN")]
    public class LOGIN : YTable
    {
        [YField(fieldName: "Label", defaulValue: "")]
        public string Label0 { get; set; }

        [YField(fieldName: "UserName", defaulValue: "")]
        public string UserName0 { get; set; }

        [YField(fieldName: "Password", defaulValue: "")]
        public string Password0 { get; set; }

        [YField(fieldName: "PLATFORM_Label", defaulValue: "")]
        public string PLATFORM_Label0 { get; set; }

        public LOGIN(Database database) : base(database) { }
    }
}
