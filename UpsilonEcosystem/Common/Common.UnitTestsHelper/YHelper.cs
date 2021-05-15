using System;
using System.IO;
using System.Linq;
using System.Text;
using Upsilon.Database.Library;

namespace Upsilon.Common.UnitTestsHelper
{
    public static class YHelper
    {
        public static string GetDatabaseFilePath(YHelperDatabaseConfiguration configuration, bool UseTempDatabase = true)
        {
            string filePath = _tryGetSolutionDirectoryInfo(Environment.CurrentDirectory) + configuration.DatabaseDirectory + configuration.Reference + ".ydb";

            if (UseTempDatabase)
            {
                filePath = filePath.Replace(configuration.Reference + ".ydb", configuration.Reference + "_tmp.ydb");
            }

            return filePath;
        }

        public static T OpenDatabaseImage<T>(YHelperDatabaseConfiguration configuration) where T : YDatabaseImage, new ()
        {
            string databaseFilename = Upsilon.Common.UnitTestsHelper.YHelper.GetDatabaseFilePath(configuration);

            YHelperDatabaseConfiguration config = new(configuration);
            string sourceFilename = Upsilon.Common.UnitTestsHelper.YHelper.GetDatabaseFilePath(config, false);
            if (File.Exists(sourceFilename)
                && (!File.Exists(databaseFilename)
                    || configuration.ResetTempDatabase))
            {
                File.Copy(sourceFilename, databaseFilename, true);
            }

            if (configuration.CheckExistingFile
                && !File.Exists(databaseFilename))
            {
                throw new FileNotFoundException("Database file not found", databaseFilename);
            }

            T database = new();
            database.Open(databaseFilename, configuration.Key);

            return database;
        }

        public static void ClearDatabaseImage(YHelperDatabaseConfiguration configuration)
        {
            string sourceFilePath = Upsilon.Common.UnitTestsHelper.YHelper.GetDatabaseFilePath(configuration);
            if (File.Exists(sourceFilePath))
            {
                File.Delete(sourceFilePath);
            }
        }

        private static string _tryGetSolutionDirectoryInfo(string currentPath = null)
        {
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
