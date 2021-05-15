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
    public class Database3 : YDatabaseImage
    {
        public List<PLATFORM3> PLATFORMs { get; private set; }
        public List<LOGIN3> LOGINs { get; private set; }

        public Database3() : base ()
        {
            this.PLATFORMs = new List<PLATFORM3>();
            this.LOGINs = new List<LOGIN3>();
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

    public class PLATFORM3 : YTable
    {
        public long PLATFORM_ID { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }

        public List<LOGIN3> LOGINs
        {
            get
            {
                if (this._Database == null)
                {
                    return null;
                }

                return ((Database3)this._Database).LOGINs.Where(x => x.PLATFORM_Label.Equals(this.Label)).ToList();
            }
        }

        public PLATFORM3() : base()
        {
            InitializeToDefaultValues();
        }

        public PLATFORM3(Database3 database) : base(database)
        {
            InitializeToDefaultValues();

            this.PLATFORM_ID = database.PLATFORMs.Any() ?
                database.PLATFORMs.Select(x => x.PLATFORM_ID).Max() + 1 :
                (long)this._Fields.Find(x => x.Name == "PLATFORM_ID").Default;
        }

        public override void InitializeToDefaultValues()
        {
            this._Fields.Add(new YField { Name = "Label", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this._Fields.Add(new YField { Name = "Url", Type = YFieldType.String, Default = string.Empty, AutoIncrement = false, });
            this._Fields.Add(new YField { Name = "PLATFORM_ID", Type = YFieldType.Integer, Default = (long)1, AutoIncrement = true, });
            this._Fields.Add(new YField { Name = "Description", Type = YFieldType.String, Default = "My default platform description", AutoIncrement = false, });

            int i = 0;
            this.Label = (string)this._Fields[i++].Default;
            this.Url = (string)this._Fields[i++].Default;
            this.PLATFORM_ID = (long)this._Fields[i++].Default;
            this.Description = (string)this._Fields[i++].Default;
        }

        public override XmlNode GetRecord()
        {
            return this._ComputeRecord(Label, Url, PLATFORM_ID, Description);
        }

        public override void SetRecord(XmlNode node)
        {
            object[] fields = this._FillFields(node);

            int i = 0;
            this.Label = (string)fields[i++];
            this.Url = (string)fields[i++];
            this.PLATFORM_ID = (long)fields[i++];
            this.Description = (string)fields[i++];
        }
    }

    public class LOGIN3 : YTable
    {
        public string Label { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PLATFORM_Label { get; set; }

        public LOGIN3() : base()
        {
            InitializeToDefaultValues();
        }

        public LOGIN3(Database database) : base(database)
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
