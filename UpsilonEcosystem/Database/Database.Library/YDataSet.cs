using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    public class YDataSet<T> : List<T> where T : YTable
    {
        public YTable[] GetYTables()
        {
            return this.Select(x => (YTable)x).ToArray();
        }
    }
}
