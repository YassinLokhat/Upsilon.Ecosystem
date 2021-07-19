﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.MetaHelper
{
    public enum YUnitTestFilesDirectory
    {
        Database,
        Files,
        Language,
    }

    public class YHelperConfiguration
    {
        public static readonly string TestFilesDirectory = @"\Upsilon\UnitTests\TestsFiles\";

        public string FullPathDirectory { get { return YHelperConfiguration.TestFilesDirectory + Directory.ToString() + @"\"; } }
        public YUnitTestFilesDirectory Directory { get; set; }
        public string Reference { get; set; } = string.Empty;
        public bool ResetTempFile { get; set; } = true;
        public bool CheckExistingFile { get; set; } = true;

        public YHelperConfiguration() { }
        public YHelperConfiguration(YHelperConfiguration copy)
        {
            this.Reference = copy.Reference;
            this.Directory = copy.Directory;
            this.ResetTempFile = copy.ResetTempFile;
            this.CheckExistingFile = copy.CheckExistingFile;
        }
    }

    public class YHelperDatabaseConfiguration : YHelperConfiguration
    {
        public string Key { get; set; } = string.Empty;

        public YHelperDatabaseConfiguration() { }
        public YHelperDatabaseConfiguration(YHelperDatabaseConfiguration copy) : base(copy)
        {
            this.Key = copy.Key;
        }
    }
}