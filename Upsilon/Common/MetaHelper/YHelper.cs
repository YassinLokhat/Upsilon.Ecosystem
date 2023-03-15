using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Upsilon.Common.Library;

namespace Upsilon.Common.MetaHelper
{
    public static class YHelper
    {
        public static string GetTestFilePath(YHelperConfiguration configuration, bool useTemp = true, bool checkFile = false)
        {
            string filePath = GetSolutionDirectory(Environment.CurrentDirectory) + configuration.FullPathDirectory + configuration.Reference + "." + configuration.Extention;

            if (checkFile
                && !File.Exists(filePath))
            {
                throw new FileNotFoundException($"File '{filePath}' does not exist.", filePath);
            }

            if (useTemp)
            {
                filePath = filePath.Replace(configuration.Reference + "." + configuration.Extention, configuration.Reference + "_tmp." + configuration.Extention);
            }

            return filePath;
        }

        public static string GenerateTempFile(YHelperConfiguration configuration, bool @override = true)
        {
            string sourceFile = GetTestFilePath(configuration, false, true);
            string tempFile = GetTestFilePath(configuration);

            File.Copy(sourceFile, tempFile, @override);

            return tempFile;
        }

        public static string GetDatabaseFilePath(YHelperConfiguration configuration, bool UseTempDatabase = true)
        {
            return GetTestFilePath(configuration, UseTempDatabase);
        }

        public static void ClearTestFile(YHelperConfiguration configuration)
        {
            string sourceFilePath = YHelper.GetTestFilePath(configuration);
            if (File.Exists(sourceFilePath))
            {
                File.Delete(sourceFilePath);
            }
        }

        public static string GetSolutionDirectory(string currentPath = null)
        {
            if (File.Exists(currentPath))
            {
                currentPath = Path.GetDirectoryName(currentPath);
            }

            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory.FullName;
        }

        public static string GetRandomString()
        {
            Random random = new ((int)DateTime.Now.Ticks);

            return GetRandomString((short)(random.Next(short.MaxValue) + 1));
        }

        public static string[] GetRandomSetOfString()
        {
            Random random = new((int)DateTime.Now.Ticks);
            int keyCount = random.Next(1, 10);
            List<string> keys = new();

            for (int j = 0; j < keyCount; j++)
            {
                keys.Add(GetRandomString());
            }

            return keys.ToArray();
        }

        public static string GetRandomString(short textLength)
        {
            Random random = new ((int)DateTime.Now.Ticks);
            byte[] bytes = new byte[textLength];

            random.NextBytes(bytes);

            return Encoding.ASCII.GetString(bytes);
        }

        public static void CorruptString(ref string value)
        {
            Random random = new ((int)DateTime.Now.Ticks);
            char[] chars = value.ToCharArray();
            int randomIndex = random.Next(chars.Length);
            char randomChar;

            do
            {
                randomChar = (char)random.Next();
            }
            while (chars[randomIndex] == randomChar);

            chars[randomIndex] = randomChar;

            value = new string(chars);
        }

        public static int GetRandomInt(int minValue, int maxValue)
        {
            Random random = new((int)DateTime.Now.Ticks);

            return random.Next(minValue, maxValue);
        }
    }
}
