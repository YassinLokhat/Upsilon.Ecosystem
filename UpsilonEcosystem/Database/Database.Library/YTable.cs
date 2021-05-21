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
    public abstract class YTable
    {
        protected YDatabaseImage _DatabaseImage = null;

        public XmlNode GetRecord(string key)
        {
            PropertyInfo[] properties = this.GetType().GetProperties()
                .Where(x => x.CustomAttributes
                    .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any())
                .ToArray();

            XmlDocument document = new ();

            XmlNode node = document.CreateNode(XmlNodeType.Element, "record", string.Empty);

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo fieldPropertyInfo = properties[i];

                XmlAttribute xmlField = document.CreateAttribute($"field_{i}");
                xmlField.Value = YCryptography.Cither_Aes(YField.GetStringFromObject(fieldPropertyInfo.GetValue(this)), key);
                node.Attributes.Append(xmlField);
            }

            return node;
        }
        
        public void SetRecord(YField[] yFields, XmlNode node)
        {
            for (int i = 0; i < yFields.Length; i++)
            {
                YField yField = yFields[i];

                PropertyInfo fieldPropertyInfo = this.GetType().GetProperties()
                    .Where(x => x.CustomAttributes
                        .Where(y => y.AttributeType == typeof(YFieldAttribute)
                            && y.ConstructorArguments[0].Value.ToString() == yField.Name).Any())
                    .FirstOrDefault();
                object value = yField.Default;
                if (node.Attributes.Contains($"field_{i}"))
                {
                    value = YField.GetObjectFromString(yField.Type, node.Attributes[$"field_{i}"].Value);
                }

                fieldPropertyInfo.SetValue(this, value);
            }
        }

        public YTable(YDatabaseImage databaseImage)
        {
            this._DatabaseImage = databaseImage;

            string tablename = this.GetType().CustomAttributes.First().ConstructorArguments.First().Value.ToString();
            
            PropertyInfo[] fieldProperties = this.GetType().GetProperties()
                    .Where(x => x.CustomAttributes
                        .Where(y => y.AttributeType == typeof(YFieldAttribute)).Any())
                    .ToArray();
            foreach (PropertyInfo fieldPropertyInfo in fieldProperties)
            {
                YFieldType type = YField.GetYFieldType(fieldPropertyInfo.PropertyType);

                fieldPropertyInfo.SetValue(this, YField.GetObjectFromString(type, fieldPropertyInfo.CustomAttributes.First().ConstructorArguments[1].Value.ToString()));
            }
        }
    }
}
