using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upsilon.Database.Library;

namespace Upsilon.Database.Library.UnitTests.DatabaseClasses
{
    public class SigmaDB : YDatabaseImage
    {
        [YDataset]
        public YDataSet<CONFIG> CONFIGs { get; private set; } = null;

        [YDataset]
        public YDataSet<COMMAND> COMMANDs { get; private set; } = null;

        [YDataset]
        public YDataSet<MODULE> MODULEs { get; private set; } = null;

        [YDataset]
        public YDataSet<ALIAS> ALIASs { get; private set; } = null;

        public SigmaDB(string filename, string key) : base(filename, key) { }
    }

    public class CONFIG : YTable
    {
        [YField]
        public long CONFIG_ID { get; private set; }

        [YField]
        public string ConfigName { get; set; }

        [YField]
        public string ConfigValue { get; set; }

        public CONFIG(SigmaDB database) : base(database)
        {
            if (database.CONFIGs.Count != 0)
            {
                this.CONFIG_ID = database.CONFIGs.Select(x => x.CONFIG_ID).Max() + 1;
            }
        }
    }

    public class COMMAND : YTable
    {
        [YField]
        public long COMMAND_ID { get; private set; }
       
        [YField]
        public string Name { get; set; }

        [YField]
        public string Program { get; set; }

        [YField]
        public string Arguments { get; set; }

        [YField]
        public bool IsBySSI { get; set; }

        [YField]
        public bool IsStartup { get; set; }

        public COMMAND(SigmaDB database) : base(database)
        {
            if (database.COMMANDs.Count != 0)
            {
                this.COMMAND_ID = database.COMMANDs.Select(x => x.COMMAND_ID).Max() + 1;
            }
        }
    }

    public class MODULE : YTable
    {
        [YField]
        public long MODULE_ID { get; private set; }

        [YField]
        public string Reference { get; set; }

        [YField]
        public string Key { get; set; }

        [YField]
        public string Version { get; set; }

        [YField]
        public string Name { get; set; }

        [YField]
        public bool IsFree { get; set; }

        [YField]
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

    public class ALIAS : YTable
    {
        [YField]
        public long ALIAS_ID { get; private set; }
       
        [YField]
        public long MODULE_ID { get; set; }

        [YField]
        public string Command { get; set; }

        [YField]
        public string Alias { get; set; }

        [YField]
        public string Program { get; set; }

        [YField]
        public string Arguments { get; set; }

        [YField]
        public string Description { get; set; }

        [YField]
        public bool IsStartup { get; set; }

        [YField]
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
