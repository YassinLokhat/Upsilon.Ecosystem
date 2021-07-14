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
    /// <summary>
    /// This static class contains some static methods and extention methods.
    /// </summary>
    public static class YStaticMethods
    {
        #region XmlAttributeCollection Extention Methods
        /// <summary>
        /// Check if a <c><see cref="XmlAttributeCollection"/> <paramref name="Attributes"/></c> contains an attribute named as the <c><paramref name="attribute"/></c>.
        /// </summary>
        /// <param name="Attributes">The <see cref="XmlAttributeCollection"/>.</param>
        /// <param name="attribute">The name of the attribute to check.</param>
        /// <returns>Return <c>true</c> or <c>false</c>.</returns>
        public static bool Contains(this XmlAttributeCollection Attributes, string attribute)
        {
            if (Attributes != null
                && Attributes[attribute] != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a <c><see cref="XmlAttributeCollection"/> <paramref name="Attributes"/></c> contains an attribute named as the <c><paramref name="attribute"/></c> and its value is not null or empty.
        /// </summary>
        /// <param name="Attributes">The <see cref="XmlAttributeCollection"/>.</param>
        /// <param name="attribute">The name of the attribute to check.</param>
        /// <returns>Return <c>true</c> or <c>false</c>.</returns>
        public static bool IsNullOrWhiteSpace(this XmlAttributeCollection Attributes, string attribute)
        {
            return !Attributes.Contains(attribute) || String.IsNullOrWhiteSpace(Attributes[attribute].Value);
        }
        #endregion

        #region Object Extention Methods
        /// <summary>
        /// Get the the MD5 Hash code of an <c><see cref="Object"/> <paramref name="obj"/></c> as a string.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/>.</param>
        /// <returns>Return the MD5 Hash code.</returns>
        public static string GetMD5HashCode(this object obj)
        {
            string str = obj.ToString();

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            string[] hash = mD5.ComputeHash(Encoding.ASCII.GetBytes(str)).Select(x => x.ToString()).ToArray();

            return string.Join(string.Empty, hash);
        }

        /// <summary>
        /// Clone a object.
        /// </summary>
        /// <param name="obj">The object to clone.</param>
        /// <returns>The cloned object.</returns>
        public static object Clone(this object obj)
        {
            return obj.SerializeObject().DeserializeObject(obj.GetType());
        }
        #endregion

        #region Enum Extention Methods
        /// <summary>
        /// Check if the <c><typeparamref name="T"/>.<paramref name="lookingForFlag"/></c> is set in the <c><typeparamref name="T"/>.<paramref name="value"/></c> enumeration flag.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="lookingForFlag">The flag value to look at.</param>
        /// <returns>Return <c>true</c> or <c>false</c>.</returns>
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

        /// <summary>
        /// Get all values of the <c><typeparamref name="T"/></c> enum type.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <returns>Returns all values of the givent enum.</returns>
        public static T[] GetEnumValues<T>()
            where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }
        #endregion

        #region Serialization/Deserialization Extention Methods
        /// <summary>
        /// Serialize an <c><see cref="Object"/> <paramref name="toSerialize"/></c>.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <returns>The serialized string.</returns>
        public static string SerializeObject(this object toSerialize)
        {
            return JsonSerializer.Serialize(toSerialize, toSerialize.GetType());
        }

        /// <summary>
        /// Deserialize an <c><paramref name="toDeserialize"/></c> string to a <c><paramref name="type"/></c> type.
        /// </summary>
        /// <param name="toDeserialize">The string to deserialize.</param>
        /// <param name="type">The type of the object.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(this string toDeserialize, Type type)
        {
            return JsonSerializer.Deserialize(toDeserialize, type);
        }
        #endregion

        #region .Net 5.0 Fixing hacks
        /// <summary>
        /// Replacing the method <c><see cref="Process"/>.<see cref="Process.Start()"/></c> since it will not run for urls on .Net 5.0 framework.
        /// </summary>
        /// <param name="url">The Url to open.</param>
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
