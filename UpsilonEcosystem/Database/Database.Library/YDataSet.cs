using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    public sealed class YDataSet<T> : List<T> where T : YTable
    {
        private YDatabaseImage _DatabaseImage = null;

        public YDataSet(YDatabaseImage databaseImage) : base()
        {
            this._DatabaseImage = databaseImage;
        }

        public YTable[] GetYTables()
        {
            return this.Select(x => (YTable)x).ToArray();
        }

        public new void Add(T item)
        {
            if (this.Contains(item))
            {
                throw new YDatabaseException($"Cannot add '{item.InternalIndex}' since it is already in this Dataset");
            }
            
            if (item.InternalIndex == 0)
            {
                item.InternalIndex = 1;

                if (this.Any())
                {
                    if (this.LongCount() == long.MaxValue)
                    {
                        throw new YDatabaseException($"Cannot add item anymore since {long.MaxValue} item count limit has been reach.");
                    }

                    item.InternalIndex = this.Select(x => x.InternalIndex).Max();
                    if (item.InternalIndex == long.MaxValue)
                    {
                        this._DatabaseImage.RebuildInternalIndex();
                    }
                }
            }

            base.Add(item);
        }

        public new bool Contains(T item)
        {
            return base.Contains(item)
                || this.Any(x => x.InternalIndex == item.InternalIndex);
        }
    }
}
