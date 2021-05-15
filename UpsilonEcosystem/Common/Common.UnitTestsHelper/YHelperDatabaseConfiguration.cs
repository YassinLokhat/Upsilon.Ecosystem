using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.UnitTestsHelper
{
    public class YHelperDatabaseConfiguration
    {
        public string Reference { get; set; } = string.Empty;
        public string DatabaseDirectory { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public bool ResetTempDatabase { get; set; } = true;
        public bool CheckExistingFile { get; set; } = true;

        public YHelperDatabaseConfiguration() { }
        public YHelperDatabaseConfiguration(YHelperDatabaseConfiguration copy)
        {
            this.Reference = copy.Reference;
            this.DatabaseDirectory = copy.DatabaseDirectory;
            this.Key = copy.Key;
            this.ResetTempDatabase = copy.ResetTempDatabase;
            this.CheckExistingFile = copy.CheckExistingFile;
        }
    }
}
