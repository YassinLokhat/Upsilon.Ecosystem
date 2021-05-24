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

        #region String Extention Methods
        public static bool IsIdentifiant(this string identifiant)
        {
            string specialChars = new(identifiant.Where(x => !char.IsLetterOrDigit(x)).ToArray());
            specialChars = new(specialChars.Where(x => x != '_').ToArray());

            if (String.IsNullOrWhiteSpace(identifiant)
                || specialChars.Length != 0
                || char.IsDigit(identifiant[0]))

            {
                return false;
            }

            return true;
        }

        public static string GetMD5HashCode(this string str)
        {
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

        #region Type Extention Methods
        public static byte GetFieldType(this Type type)
        {
            if (type.Equals(typeof(short[])))
            {
                return 0;
            }
            else if (type.Equals(typeof(sbyte))
                || type.Equals(typeof(short))
                || type.Equals(typeof(int))
                || type.Equals(typeof(long))
                || type.Equals(typeof(byte))
                || type.Equals(typeof(ushort))
                || type.Equals(typeof(uint))
                || type.Equals(typeof(ulong)))
            {
                return 1;
            }
            else if (type.Equals(typeof(float))
                || type.Equals(typeof(double))
                || type.Equals(typeof(decimal)))
            {
                return 2;
            }
            else if (type.Equals(typeof(string))
                || type.Equals(typeof(char)))
            {
                return 3;
            }
            else if (type.Equals(typeof(DateTime)))
            {
                return 4;
            }

            return byte.MaxValue;
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
