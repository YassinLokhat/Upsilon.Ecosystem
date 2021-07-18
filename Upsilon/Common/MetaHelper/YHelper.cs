using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Upsilon.Database.Library;

namespace Upsilon.Common.MetaHelper
{
    public static class YHelper
    {
        public static string GetTestFilePath(YHelperConfiguration configuration, string extension, bool useTemp = true, bool checkFile = false)
        {
            string filePath = GetSolutionDirectory(Environment.CurrentDirectory) + configuration.FullPathDirectory + configuration.Reference + "." + extension;

            if (checkFile
                && !File.Exists(filePath))
            {
                throw new FileNotFoundException($"File '{filePath}' does not exist.", filePath);
            }

            if (useTemp)
            {
                filePath = filePath.Replace(configuration.Reference + "." + extension, configuration.Reference + "_tmp." + extension);
            }

            return filePath;
        }

        public static string GetDatabaseFilePath(YHelperDatabaseConfiguration configuration, bool UseTempDatabase = true)
        {
            return GetTestFilePath(configuration, "ydb", UseTempDatabase);
        }

        public static T OpenDatabaseImage<T>(YHelperDatabaseConfiguration configuration) where T : YDatabaseImage
        {
            string databaseFilename = Upsilon.Common.MetaHelper.YHelper.GetDatabaseFilePath(configuration);

            YHelperDatabaseConfiguration config = new(configuration);
            string sourceFilename = Upsilon.Common.MetaHelper.YHelper.GetDatabaseFilePath(config, false);
            if (File.Exists(sourceFilename)
                && (!File.Exists(databaseFilename)
                    || configuration.ResetTempFile))
            {
                File.Copy(sourceFilename, databaseFilename, true);
            }

            if (configuration.CheckExistingFile
                && !File.Exists(databaseFilename))
            {
                throw new FileNotFoundException("Database file not found", databaseFilename);
            }

            try
            {
                T database = (T)Activator.CreateInstance(typeof(T), new object[] { databaseFilename, configuration.Key });
                
                return database;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public static void ClearDatabaseImage(YHelperDatabaseConfiguration configuration)
        {
            string sourceFilePath = Upsilon.Common.MetaHelper.YHelper.GetDatabaseFilePath(configuration);
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
    }
}
