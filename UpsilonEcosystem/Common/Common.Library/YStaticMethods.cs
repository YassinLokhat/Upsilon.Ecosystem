using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Upsilon.Common.Library
{
    public static class YStaticMethods
    {
        #region XmlAttributeCollection Extention Methods
        public static bool Contains(this XmlAttributeCollection Attributes, string attribute)
        {
            if (Attributes != null
                && Attributes[attribute] != null)
            {
                return true;
            }

            return false;
        }

        public static bool IsNullOrWhiteSpace(this XmlAttributeCollection Attributes, string attribute)
        {
            return !Attributes.Contains(attribute) || String.IsNullOrWhiteSpace(Attributes[attribute].Value);
        }
        #endregion

        #region Object Extention Methods
        public static string GetMD5HashCode(this object obj)
        {
            string str = obj.ToString();

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            string[] hash = mD5.ComputeHash(Encoding.ASCII.GetBytes(str)).Select(x => x.ToString()).ToArray();

            return string.Join(string.Empty, hash);
        }
        #endregion

        #region Enum Extention Methods
        public static bool IsEnumFlagPresent<T>(this T value, T lookingForFlag)
            where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            int intValue = (int)(object)value;
            int intLookingForFlag = (int)(object)lookingForFlag;
            return ((intValue & intLookingForFlag) == intLookingForFlag);
        }
        #endregion

        #region Serialization/Deserialization Extention Methods
        public static string SerializeObject(this object toSerialize)
        {
            return JsonSerializer.Serialize(toSerialize, toSerialize.GetType());
        }

        public static object DeserializeObject(this string toDeserialize, Type type)
        {
            return JsonSerializer.Deserialize(toDeserialize, type);
        }
        #endregion
    }
}
