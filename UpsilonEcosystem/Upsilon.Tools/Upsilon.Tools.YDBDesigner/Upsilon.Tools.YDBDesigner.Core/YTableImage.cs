using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;
using Upsilon.Database.Library;

namespace Upsilon.Tools.YDBDesigner.Core
{
    public class YTableImage
    {
        public string Name { get; set; }
        public List<YFieldImage> Fields { get; private set; }
        public List<YRecord> Records { get; private set; }

        public YTableImage(XmlNode node, string key)
        {
            this.Name = node.Attributes["name"].Value.Uncipher_Aes(key);

            this.Fields = new();

            foreach (XmlNode field in node.SelectNodes("./fields/field"))
            {
                this.Fields.Add(new(field, key));
            }

            this.Records = new();

            foreach (XmlNode field in node.SelectNodes("./records/record"))
            {
                this.Records.Add(new(field, key));
            }
        }

        public XmlNode GetXmlNode(string key)
        {
            return null;
        }

        public string[][] GetFieldList()
        {
            return this.Fields.Select(x => new[] { x.Name, x.Type }).ToArray();
        }

        public string[][] GetRecordList()
        {
            return this.Records.Select(x => x.Values).ToArray();
        }

        public void RebuildInternalIndex()
        {

        }
    }

    public class YFieldImage
    {
        public string Name { get; private set; }
        public string Type { get; private set; }

        public YFieldImage(XmlNode node, string key)
        {
            this.Name = node.Attributes["name"].Value.Uncipher_Aes(key);
            this.Type = node.Attributes["type"].Value.Uncipher_Aes(key);
        }
    }

    public class YRecord
    {
        public string[] Values { get; private set; }

        public YRecord(XmlNode node, string key)
        {
            List<string> val = new();

            foreach (XmlAttribute attribute in node.Attributes)
            {
                val.Add(attribute.Value.Uncipher_Aes(key));
            }

            this.Values = val.ToArray();
        }
    }
}
