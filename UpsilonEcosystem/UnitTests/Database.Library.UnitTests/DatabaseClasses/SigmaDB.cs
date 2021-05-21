using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upsilon.Database.Library;

namespace Database.Library.UnitTests.DatabaseClasses
{
    public class SigmaDB : YDatabaseImage
    {
        [YDataset("CONFIG")]
        public YDataSet<CONFIG> CONFIGs { get; private set; } = new YDataSet<CONFIG>();

        [YDataset("COMMAND")]
        public YDataSet<COMMAND> COMMANDs { get; private set; } = new YDataSet<COMMAND>();

        [YDataset("MODULE")]
        public YDataSet<MODULE> MODULEs { get; private set; } = new YDataSet<MODULE>();

        [YDataset("ALIAS")]
        public YDataSet<ALIAS> ALIASs { get; private set; } = new YDataSet<ALIAS>();

        public SigmaDB(string filename, string key) : base(filename, key) { }
    }

    [YTable("CONFIG")]
    public class CONFIG : YTable
    {
        [YField(fieldName: "CONFIG_ID", defaulValue: "1")]
        public long CONFIG_ID { get; private set; }

        [YField(fieldName: "ConfigName", defaulValue: "")]
        public string ConfigName { get; set; }

        [YField(fieldName: "ConfigValue", defaulValue: "")]
        public string ConfigValue { get; set; }

        public CONFIG(SigmaDB database) : base(database)
        {
            if (database.CONFIGs.Count != 0)
            {
                this.CONFIG_ID = database.CONFIGs.Select(x => x.CONFIG_ID).Max() + 1;
            }
        }
    }

    [YTable("COMMAND")]
    public class COMMAND : YTable
    {
        [YField(fieldName: "COMMAND_ID", defaulValue: "1")]
        public long COMMAND_ID { get; private set; }
       
        [YField(fieldName: "Name", defaulValue: "")]
        public string Name { get; set; }

        [YField(fieldName: "Program", defaulValue: "")]
        public string Program { get; set; }

        [YField(fieldName: "Arguments", defaulValue: "")]
        public string Arguments { get; set; }

        [YField(fieldName: "IsBySSI", defaulValue: "")]
        public bool IsBySSI { get; set; }

        [YField(fieldName: "IsStartup", defaulValue: "")]
        public bool IsStartup { get; set; }

        public COMMAND(SigmaDB database) : base(database)
        {
            if (database.COMMANDs.Count != 0)
            {
                this.COMMAND_ID = database.COMMANDs.Select(x => x.COMMAND_ID).Max() + 1;
            }
        }
    }

    [YTable("MODULE")]
    public class MODULE : YTable
    {
        [YField(fieldName: "MODULE_ID", defaulValue: "1")]
        public long MODULE_ID { get; private set; }

        [YField(fieldName: "Reference", defaulValue: "")]
        public string Reference { get; set; }

        [YField(fieldName: "Key", defaulValue: "")]
        public string Key { get; set; }

        [YField(fieldName: "Version", defaulValue: "")]
        public string Version { get; set; }

        [YField(fieldName: "Name", defaulValue: "")]
        public string Name { get; set; }

        [YField(fieldName: "IsFree", defaulValue: "")]
        public bool IsFree { get; set; }

        [YField(fieldName: "About", defaulValue: "")]
        public string About { get; set; }

        public List<ALIAS> ALIASs { get { return ((SigmaDB)this._DatabaseImage).ALIASs.Where(x => x.MODULE_ID == this.MODULE_ID).ToList(); } }

        public MODULE(SigmaDB database) : base(database)
        {
            if (database.MODULEs.Count != 0)
            {
                this.MODULE_ID = database.MODULEs.Select(x => x.MODULE_ID).Max() + 1;
            }
        }
    }

    [YTable("ALIAS")]
    public class ALIAS : YTable
    {
        [YField(fieldName: "ALIAS_ID", defaulValue: "1")]
        public long ALIAS_ID { get; private set; }
       
        [YField(fieldName: "MODULE_ID", defaulValue: "1")]
        public long MODULE_ID { get; set; }

        [YField(fieldName: "Command", defaulValue: "")]
        public string Command { get; set; }

        [YField(fieldName: "Alias", defaulValue: "")]
        public string Alias { get; set; }

        [YField(fieldName: "Program", defaulValue: "")]
        public string Program { get; set; }

        [YField(fieldName: "Arguments", defaulValue: "")]
        public string Arguments { get; set; }

        [YField(fieldName: "Description", defaulValue: "")]
        public string Description { get; set; }

        [YField(fieldName: "IsStartup", defaulValue: "")]
        public bool IsStartup { get; set; }

        [YField(fieldName: "IsEnabled", defaulValue: "")]
        public bool IsEnabled { get; set; }

        public MODULE MODULE { get { return ((SigmaDB)this._DatabaseImage).MODULEs.Find(x => x.MODULE_ID == this.MODULE_ID); } }

        public ALIAS(SigmaDB database) : base(database)
        {
            if (database.ALIASs.Count != 0)
            {
                this.ALIAS_ID = database.ALIASs.Select(x => x.ALIAS_ID).Max() + 1;
            }
        }
    }
}
