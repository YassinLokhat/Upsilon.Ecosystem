using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Database.Library
{
    public sealed class YDataSet<T> : List<T> where T : YTable
    {
        // To be removed
        private readonly YDatabaseImage _DatabaseImage = null;
        
        private readonly List<long> _removedIndexes = new();

        public YDataSet(YDatabaseImage databaseImage) : base()
        {
            this._DatabaseImage = databaseImage;
        }

        public long[] GetRemovedIndexes()
        {
            return this._removedIndexes.ToArray();
        }

        public YTable[] GetYTables()
        {
            return this.Select(x => (YTable)x).ToArray();
        }

        public new void Remove(T item)
        {
            this._removedIndexes.Add(item.InternalIndex);
            base.Remove(item);
        }

        public new void RemoveAt(int index)
        {
            this.Remove(this[index]);
        }

#pragma warning disable CA1829 // Use Length/Count property instead of Count() when available
        public new void Insert(int index, T item)
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
                    item.InternalIndex = this.Select(x => x.InternalIndex).Max();

                    if (this.LongCount() == long.MaxValue
                        || item.InternalIndex == long.MaxValue)
                    {
                        throw new YDatabaseException($"Cannot add item anymore since {long.MaxValue} item count limit has been reach.");
                    }

                    item.InternalIndex++;
                }
            }

            base.Insert(index, item);
        }
#pragma warning restore CA1829 // Use Length/Count property instead of Count() when available

        public new void Add(T item)
        {
            this.Insert(this.Count, item);
        }

        public T GetRecordByInternalIndex(long internalIndex)
        {
            return this.Find(x => x.InternalIndex == internalIndex);
        }

        public new bool Contains(T item)
        {
            return base.Contains(item)
                || this.Any(x => x.InternalIndex == item.InternalIndex);
        }
    }
}
