using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Upsilon.Database.Library.UnitTests
{
    public class Database : YDatabaseImage
    {
        [YDataset]
        public YDataSet<AUTHOR> AUTHORs { get; private set; } = null;

        [YDataset]
        public YDataSet<BOOK> BOOKs { get; private set; } = null;

        public Database(string filename, string key) : base(filename, key) { }
    }

    public class AUTHOR : YTable
    {
        [YField]
        public string Name { get; set; }

        [YField]
        public DateTime BirthDay { get; set; }

        public List<BOOK> Books { get { return ((Database)this._DatabaseImage).BOOKs.Where(x => x.Author == this.Name).ToList(); } }

        public AUTHOR(Database database) : base(database) { }
    }

    public class BOOK : YTable
    {
        [YField]
        public string Title { get; set; }

        //[YField]
        public string Editor { get; set; }

        [YField]
        public string Author { get; set; }

        [YField]
        public string Synopsis { get; set; }

        public AUTHOR BookAuthor { get { return ((Database)this._DatabaseImage).AUTHORs.Find(x => x.Name == this.Author); } }

        public BOOK(Database database) : base(database) { }
    }
}
