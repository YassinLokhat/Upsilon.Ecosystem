using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        public static T[] GetEnumValues<T>()
            where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
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

        #region .Net 5.0 Fixing hacks
        public static void ProcessStartUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion
    }
}
