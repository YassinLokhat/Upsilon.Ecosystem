using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    public interface ITable
    {
        void SetRecord(Record record);
        Record GetRecord();
    }
}
