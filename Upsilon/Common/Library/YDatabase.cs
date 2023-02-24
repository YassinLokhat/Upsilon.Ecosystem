using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    public interface IYDataTable
    {
        void EncryptTables(string[] passwords);
        void DecryptTables(string[] passwords);
    }

    public abstract class YDatabase : IYDataTable
    {
        public abstract void DecryptTables(string[] passwords);
        public abstract void EncryptTables(string[] passwords);

        public static T Load<T>(string filename, string[] passwords) where T : YDatabase
        {
            var user = File.ReadAllText(filename).DeserializeObject<T>();
            user.DecryptTables(passwords);

            return user;
        }

        public static void Save(YDatabase database, string filename, string[] passwords)
        {
            database.EncryptTables(passwords);
            File.WriteAllText(filename, database.SerializeObject(true));
        }
    }

}
