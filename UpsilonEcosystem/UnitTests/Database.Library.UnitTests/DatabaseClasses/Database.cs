using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Upsilon.Database.Library.UnitTests
{
    public class Database : YDatabaseImage
    {
        [YTable("PLATFORM")]
        public YDataSet<PLATFORM> PLATFORMs { get; private set; } = new YDataSet<PLATFORM>();

        [YTable("LOGIN")]
        public YDataSet<LOGIN> LOGINs { get; private set; } = new YDataSet<LOGIN>();

        public Database(string filename, string key) : base(filename, key) { }
    }

    public class PLATFORM : YTable
    {
        [YField("Label")]
        public string Label0 { get; set; }

        [YField("Url")]
        public string Url0 { get; set; }
    }

    public class LOGIN : YTable
    {
        [YField("Label")]
        public string Label0 { get; set; }

        [YField("UserName")]
        public string UserName0 { get; set; }

        [YField("Password")]
        public string Password0 { get; set; }

        [YField("PLATFORM_Label")]
        public string PLATFORM_Label0 { get; set; }
    }
}
