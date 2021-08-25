using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    internal sealed class YField
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public YField(string filename, string key, string tablename, XmlNode node)
        {
            try
            {
                this.Name = node.Attributes["name"].Value.Uncipher_Aes(key);
                this.Type = Type.GetType(node.Attributes["type"].Value.Uncipher_Aes(key));
            }
            catch
            {
                throw new YDatabaseXmlCorruptionException(filename, $"A field definition is not valid in '{tablename}' table node.");
            }
        }

        public static XmlNode GetFieldNode(PropertyInfo field, string key)
        {
            XmlDocument document = new ();
            
            XmlNode node = document.CreateNode(XmlNodeType.Element, "field", "");

            XmlAttribute attribute = document.CreateAttribute("name");
            attribute.Value = field.Name.Cipher_Aes(key);
            node.Attributes.Append(attribute);

            attribute = document.CreateAttribute("type");
            attribute.Value = field.PropertyType.FullName.Cipher_Aes(key);
            node.Attributes.Append(attribute);

            return node;
        }
    }
}
