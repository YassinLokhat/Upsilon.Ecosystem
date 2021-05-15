using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Upsilon.Common.Library;
using Upsilon.Database.Library;

namespace Upsilon.Database.Library.UnitTests
{
    public class Database : YDatabaseImage
    {
        public List<PLATFORM> PLATFORMs { get; private set; }
        public List<LOGIN> LOGINs { get; private set; }

        public Database() : base ()
        {
            this.PLATFORMs = new List<PLATFORM>();
            this.LOGINs = new List<LOGIN>();
        }

        public override void Pull()
        {
            base.Pull();

            this.PLATFORMs.Clear();
            this.LOGINs.Clear();

            this._FillTablesFromXml(this.PLATFORMs, "PLATFORM");
            this._FillTablesFromXml(this.LOGINs, "LOGIN");
        }

        public override void Push()
        {
            this._Tables.Clear();

            this._FillTablesFromLists(this.PLATFORMs, "PLATFORM");
            this._FillTablesFromLists(this.LOGINs, "LOGIN");

            base.Push();
        }
    }

    public class PLATFORM : YTable
    {
        public string Label { get; set; }
        public string Url { get; set; }

        public List<LOGIN> LOGINs
        {
            get
            {
                if (this._Database == null)
                {
                    return null;
                }

                return ((Database)this._Database).LOGINs.Where(x => x.PLATFORM_Label.Equals(this.Label)).ToList();
            }
        }

        public PLATFORM() : base()
        {
            InitializeToDefaultValues();
        }

        public PLATFORM(Database database) : base(database)
        {
            InitializeToDefaultValues();
        }

        public override void InitializeToDefaultValues()
        {
            int i = 0;

            this._Fields.Add(new YField { Name = "Label", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this.Label = (string)this._Fields[i++].Default;
            this._Fields.Add(new YField { Name = "Url", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this.Url = (string)this._Fields[i++].Default;
        }

        public override XmlNode GetRecord()
        {
            return this._ComputeRecord(Label, Url);
        }

        public override void SetRecord(XmlNode node)
        {
            object[] fields = this._FillFields(node);

            int i = 0;
            this.Label = (string)fields[i++];
            this.Url = (string)fields[i++];
        }
    }

    public class LOGIN : YTable
    {
        public string Label { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PLATFORM_Label { get; set; }

        public LOGIN() : base()
        {
            InitializeToDefaultValues();
        }

        public LOGIN(Database database) : base(database)
        {
            InitializeToDefaultValues();
        }

        public override void InitializeToDefaultValues()
        {
            int i = 0;

            this._Fields.Add(new YField { Name = "Label", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this.Label = (string)this._Fields[i++].Default;
            this._Fields.Add(new YField { Name = "UserName", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this.UserName = (string)this._Fields[i++].Default;
            this._Fields.Add(new YField { Name = "Password", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this.Password = (string)this._Fields[i++].Default;
            this._Fields.Add(new YField { Name = "PLATFORM_Label", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this.PLATFORM_Label = (string)this._Fields[i++].Default;
        }

        public override XmlNode GetRecord()
        {
            return this._ComputeRecord(Label, UserName, Password, PLATFORM_Label);
        }

        public override void SetRecord(XmlNode node)
        {
            object[] fields = this._FillFields(node);

            int i = 0;
            this.Label = (string)fields[i++];
            this.UserName = (string)fields[i++];
            this.Password = (string)fields[i++];
            this.PLATFORM_Label = (string)fields[i++];
        }
    }
}
