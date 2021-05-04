using System;
using System.IO;
using System.Linq;
using System.Text;
using Upsilon.Database.Library;

namespace Upsilon.Common.UnitTestsHelper
{
    public static class Helper
    {
        public static string GetDatabaseFilePath(string reference)
        {
            string filePath = _TryGetSolutionDirectoryInfo(Environment.CurrentDirectory) + @"\UpsilonEcosystem\UnitTests\Tests\Database\" + reference + ".ydb";

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Database file not found", filePath);
            }

            return filePath;
        }

        public static string GetTempDatabaseFilePath(string reference, bool checkFile = true)
        {
            string filePath = GetDatabaseFilePath(reference);
            filePath = filePath.Replace(reference, reference + "_tmp");

            if (checkFile
                && !File.Exists(filePath))
            {
                throw new FileNotFoundException("Database file not found", filePath);
            }

            return filePath;
        }

        public static DatabaseImage OpenDatabaseImage(string reference, string key)
        {
            string sourceFilePath = Upsilon.Common.UnitTestsHelper.Helper.GetDatabaseFilePath(reference);

            return Upsilon.Common.UnitTestsHelper.Helper._OpenDatabaseImage(sourceFilePath, key);
        }

        public static DatabaseImage OpenTempDatabaseImage(string reference, string key, bool reset = true)
        {
            string sourceFilePath = Upsilon.Common.UnitTestsHelper.Helper.GetDatabaseFilePath(reference);
            string databaseFilePath = Upsilon.Common.UnitTestsHelper.Helper.GetTempDatabaseFilePath(reference, false);
            if (reset)
            {
                File.Copy(sourceFilePath, databaseFilePath, true);
            }

            return Upsilon.Common.UnitTestsHelper.Helper._OpenDatabaseImage(databaseFilePath, key);
        }

        private static DatabaseImage _OpenDatabaseImage(string sourceFilePath, string key)
        {
            DatabaseImage database = new DatabaseImage(sourceFilePath, key);

            return database;
        }

        public static void ClearDatabaseImage(string reference)
        {
            string sourceFilePath = Upsilon.Common.UnitTestsHelper.Helper.GetTempDatabaseFilePath(reference);

            File.Delete(sourceFilePath);
        }

        private static string _TryGetSolutionDirectoryInfo(string currentPath = null)
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
            Random random = new Random((int)DateTime.Now.Ticks);

            return GetRandomString((short)(random.Next(short.MaxValue) + 1));
        }

        public static string GetRandomString(short textLength)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            byte[] bytes = new byte[textLength];

            random.NextBytes(bytes);

            return Encoding.ASCII.GetString(bytes);
        }

        public static void CorruptString(ref string value)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
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
