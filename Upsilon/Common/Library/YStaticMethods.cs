using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
            YDebugTrace.TraceOn();

            if (Attributes != null
                && Attributes[attribute] != null)
            {
                return YDebugTrace.TraceOff(true);
            }

            return YDebugTrace.Trace(false);
        }

        /// <summary>
        /// Check if a <c><see cref="XmlAttributeCollection"/> <paramref name="Attributes"/></c> contains an attribute named as the <c><paramref name="attribute"/></c> and its value is not null or empty.
        /// </summary>
        /// <param name="Attributes">The <see cref="XmlAttributeCollection"/>.</param>
        /// <param name="attribute">The name of the attribute to check.</param>
        /// <returns>Return <c>true</c> or <c>false</c>.</returns>
        public static bool IsNullOrWhiteSpace(this XmlAttributeCollection Attributes, string attribute)
        {
            return YDebugTrace.Trace(!Attributes.Contains(attribute) || String.IsNullOrWhiteSpace(Attributes[attribute].Value));
        }
        #endregion

        #region Object Extention Methods
        /// <summary>
        /// Get the MD5 Hash code of an <c><see cref="Object"/> <paramref name="obj"/></c> as a string.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/>.</param>
        /// <returns>Return the MD5 Hash code.</returns>
        public static string GetMD5HashCode(this object obj)
        {
            YDebugTrace.TraceOn();

            string str = obj.ToString();

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            string[] hash = mD5.ComputeHash(Encoding.ASCII.GetBytes(str)).Select(x => x.ToString()).ToArray();

            return YDebugTrace.TraceOff(string.Join(string.Empty, hash));
        }

        /// <summary>
        /// Get the Upsilon Hash code of an <c><see cref="Object"/> <paramref name="obj"/></c> as a string.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/>.</param>
        /// <returns>Return the Upsilon Hash code.</returns>
        public static string GetUpsilonHashCode(this object obj)
        {
            YDebugTrace.TraceOn();

            string str = obj.SerializeObject() + obj.ToString();

            System.Security.Cryptography.MD5 mD5 = System.Security.Cryptography.MD5.Create();
            string[] hash = mD5.ComputeHash(Encoding.UTF8.GetBytes(str)).Select(x => x.ToString()).ToArray();

            System.Security.Cryptography.SHA512 sHA512 = System.Security.Cryptography.SHA512.Create();
            hash = hash.Union(sHA512.ComputeHash(Encoding.UTF8.GetBytes(str)).Select(x => x.ToString())).ToArray();

            return YDebugTrace.TraceOff(string.Join(string.Empty, hash));
        }

        /// <summary>
        /// Copy the content of a given object to the current object without creating new object.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="objectDestination">The object destination.</param>
        /// <param name="objectSource">The object source.</param>
        public static void CopyFrom<T>(this T objectDestination, T objectSource)
        {
            YDebugTrace.TraceOn();

            Type type = typeof(T);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                object value = property.GetValue(objectSource);
                property.SetValue(objectDestination, value);
            }

            YDebugTrace.TraceOff();
        }

        /// <summary>
        /// Clone a object.
        /// </summary>
        /// <typeparam name="T">The type of the object to clone.</typeparam>
        /// <param name="obj">The object to clone.</param>
        /// <returns>The cloned object.</returns>
        public static T Clone<T>(this T obj)
        {
            YDebugTrace.TraceOn();

            JsonSerializerOptions jsonSerializerOptions = new() {  };

            return YDebugTrace.TraceOff(obj.SerializeObject().DeserializeObject<T>());
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
            YDebugTrace.TraceOn();

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            int intValue = (int)(object)value;
            int intLookingForFlag = (int)(object)lookingForFlag;

            return YDebugTrace.TraceOff((intValue & intLookingForFlag) == intLookingForFlag);
        }

        /// <summary>
        /// Get all values of the <c><typeparamref name="T"/></c> enum type.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <returns>Returns all values of the givent enum.</returns>
        public static T[] GetEnumValues<T>()
            where T : struct
        {
            return YDebugTrace.Trace(Enum.GetValues(typeof(T)).Cast<T>().ToArray());
        }
        #endregion

        #region Serialization/Deserialization Extention Methods
        /// <summary>
        /// Serialize an <c><see cref="Object"/> <paramref name="toSerialize"/></c>.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="indent">Indent the result or not. Default value is <c>false</c>.</param>
        /// <returns>The serialized string.</returns>
        public static string SerializeObject(this object toSerialize, bool indent = false)
        {
            YDebugTrace.TraceOn();

            JsonSerializerOptions options = new()
            {
                WriteIndented = indent,
            };

            return YDebugTrace.TraceOff(JsonSerializer.Serialize(toSerialize, toSerialize.GetType(), options));
        }

        /// <summary>
        /// Deserialize an <c><paramref name="toDeserialize"/></c> string to a <c><paramref name="type"/></c> type.
        /// </summary>
        /// <param name="toDeserialize">The string to deserialize.</param>
        /// <param name="type">The type of the object.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(this string toDeserialize, Type type)
        {
            return YDebugTrace.Trace(JsonSerializer.Deserialize(toDeserialize, type));
        }

        /// <summary>
        /// Deserialize an <c><paramref name="toDeserialize"/></c> string to a <c><typeparamref name="T"/></c> type.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="toDeserialize">The string to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeObject<T>(this string toDeserialize)
        {
            return YDebugTrace.Trace(JsonSerializer.Deserialize<T>(toDeserialize));
        }
        #endregion

        #region .Net 5.0 Fixing hacks
        /// <summary>
        /// Replacing the method <c><see cref="Process"/>.<see cref="Process.Start()"/></c> since it will not run for urls on .Net 5.0 framework.
        /// </summary>
        /// <param name="url">The Url to open.</param>
        public static void ProcessStartUrl(string url)
        {
            YDebugTrace.TraceOn();

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

            YDebugTrace.TraceOff();
        }
        #endregion

        #region Download Methods
        /// <summary>
        /// Download a string from the given URL.
        /// </summary>
        /// <param name="url">The URL of the string to download.</param>
        /// <returns>The downloaded string.</returns>
        public static string DownloadString(string url)
        {
            YDebugTrace.TraceOn();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            WebClient webClient = new();

            return YDebugTrace.TraceOff(webClient.DownloadString(url));
        }

        /// <summary>
        /// Download a file from the given URL.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="filePath"></param>
        /// <returns>The downloaded string.</returns>
        public static void DownloadFile(string url, string filePath)
        {
            YDebugTrace.TraceOn();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            WebClient webClient = new();

            webClient.DownloadFile(url, filePath);

            YDebugTrace.TraceOff();
        }
        #endregion

        #region File Methods
        /// <summary>
        /// Copy a file or a directory.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationDirectory">The directory where the copy will be save.</param>
        /// <param name="override">Override the destination if already exists.</param>
        /// <param name="throwException">Throw an exception when error occurs.</param>
        public static void Copy(string sourcePath, string destinationDirectory, bool @override = false, bool throwException = true)
        {
            YDebugTrace.TraceOn();

            if (File.Exists(sourcePath))
            {
                Directory.CreateDirectory(destinationDirectory);

                try
                {
                    File.Copy(sourcePath, Path.Combine(destinationDirectory, Path.GetFileName(sourcePath)), @override);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    if (throwException)
                        throw;
                }
            }
            else if (Directory.Exists(sourcePath))
            {
                DirectoryInfo dir = new(sourcePath);

                string[] dirs = dir.GetDirectories().Select(x => x.FullName).ToArray();
                string[] files = dir.GetFiles().Select(x => x.FullName).ToArray();

                destinationDirectory = Path.Combine(destinationDirectory, Path.GetFileName(sourcePath));
                Directory.CreateDirectory(destinationDirectory);

                foreach (string source in dirs.Union(files))
                {
                    YStaticMethods.Copy(source, destinationDirectory, @override);
                }
            }
            else if (throwException)
            {
                throw new Exception($"File or Directory not found :\n'{sourcePath}'");
            }

            YDebugTrace.TraceOff();
        }
        #endregion

        #region Linq extention methods
        /// <summary>
        /// Find an elemen from a listable object.
        /// </summary>
        /// <typeparam name="T">The type of object in the list.</typeparam>
        /// <param name="list">The list of objects.</param>
        /// <param name="predicate">The search criteria.</param>
        /// <returns>The object found or <c>null</c>.</returns>
        public static T Find<T>(this IList<T> list, Predicate<T> predicate)
        {
            return YDebugTrace.Trace(list.ToList().Find(predicate));
        }

        /// <summary>
        /// Find an elemen from an enumerable object.
        /// </summary>
        /// <typeparam name="T">The type of object in the list.</typeparam>
        /// <param name="list">The list of objects.</param>
        /// <param name="predicate">The search criteria.</param>
        /// <returns>The object found or <c>null</c>.</returns>
        public static T Find<T>(this IEnumerable<T> list, Predicate<T> predicate)
        {
            return YDebugTrace.Trace(list.ToList().Find(predicate));
        }

        /// <summary>
        /// Take <paramref name="count"/> elements from the <paramref name="startIndex"/> of the <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The type of object in the list.</typeparam>
        /// <param name="list">The list of objects.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of element to take.</param>
        /// <returns></returns>
        public static IEnumerable<T> TakeElementFrom<T>(this IEnumerable<T> list, int startIndex, int count)
        {
            YDebugTrace.TraceOn(); 

            if (count < 0)
            {
                count = list.Count() - startIndex;
            }

            T[] subArray = new T[count];
            Array.Copy(list.ToArray(), startIndex, subArray, 0, count);

            return YDebugTrace.TraceOff(subArray.AsEnumerable());
        }

        /// <summary>
        /// Take all elements from the <paramref name="startIndex"/> of the <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The type of object in the list.</typeparam>
        /// <param name="list">The list of objects.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public static IEnumerable<T> TakeElementFrom<T>(this IEnumerable<T> list, int startIndex)
        {
            return YDebugTrace.Trace(list.TakeElementFrom(startIndex, -1));
        }
        #endregion

        #region List extention methods
        /// <summary>
        /// Get the common root of a list of directories.
        /// </summary>
        /// <param name="directories">The list of directories.</param>
        /// <param name="separator">The path separator.</param>
        /// <returns></returns>
        public static string GetCommonRootDirectory(IList<string> directories, char separator)
        {
            YDebugTrace.TraceOn();

            var urls = directories.Select(x => x.TrimEnd(separator).Split(separator).ToList()).ToArray();

            if (!urls.Any())
            {
                return string.Empty;
            }

            var rootUrl = new List<string>();

            bool @continue = true;
            while (@continue)
            {
                string firstElement = urls.First().FirstOrDefault();
                foreach (var url in urls)
                {
                    if (url.FirstOrDefault() == firstElement
                        && url.Any())
                    {
                        url.RemoveAt(0);
                    }
                    else
                    {
                        @continue = false;
                        break;
                    }
                }

                if (@continue)
                {
                    rootUrl.Add(firstElement);
                }
            }

            return YDebugTrace.TraceOff(string.Join(separator, rootUrl));
        }

        /// <summary>
        /// Get the common root of a list of directories.
        /// </summary>
        /// <param name="directories">The list of directories.</param>
        /// <param name="separator">The path separator.</param>
        /// <returns></returns>
        public static string GetCommonRootDirectory(IEnumerable<string> directories, char separator)
        {
            return YDebugTrace.Trace(GetCommonRootDirectory(directories.ToArray(), separator));
        }
        #endregion
    }
}
