﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library
{
    /// <summary>
    /// Represent a set of record.
    /// </summary>
    /// <typeparam name="T">The class that represent the record. This class shound inherit to <c><see cref="YTable"/></c>.</typeparam>
    public sealed class YDataSet<T> : List<T> where T : YTable
    {
        private readonly List<long> _removedIndexes = new();

        /// <summary>
        /// Pop all removed Internal Indexes.
        /// </summary>
        /// <returns>All removed Internal Indexes.</returns>
        public long[] PopRemovedIndexes()
        {
            YDebugTrace.TraceOn();

            long[] output = this._removedIndexes.ToArray();
            this._removedIndexes.Clear();

            return YDebugTrace.TraceOff(output);
        }

        internal YTable[] _GetYTables()
        {
            return YDebugTrace.Trace(this.Select(x => (YTable)x).ToArray());
        }

        /// <summary>
        /// Remove a record from the dataset.
        /// </summary>
        /// <param name="item">The record to remove.</param>
        public new void Remove(T item)
        {
            YDebugTrace.TraceOn(new object[] { item });

            this._removedIndexes.Add(item.InternalIndex);
            base.Remove(item);

            YDebugTrace.TraceOff();
        }

        /// <summary>
        /// Remove the record at the given index from the dataset.
        /// </summary>
        /// <param name="index">The index of the record to remove.</param>
        public new void RemoveAt(int index)
        {
            YDebugTrace.TraceOn(new object[] { index });

            this.Remove(this[index]);

            YDebugTrace.TraceOff();
        }


        /// <summary>
        /// Insert a record at a certain index in the dataset.
        /// </summary>
        /// <param name="index">The index where the record will be insert.</param>
        /// <param name="item">The record to insert.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1829:Use Length/Count property instead of Count() when available", Justification = "<Pending>")]
        public new void Insert(int index, T item)
        {
            YDebugTrace.TraceOn(new object[] { index, item });

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

            YDebugTrace.TraceOff();
        }

        /// <summary>
        /// Add a range of records at the end of the dataset.
        /// </summary>
        /// <param name="items">The set of record to insert.</param>
        public new void AddRange(IEnumerable<T> items)
        {
            YDebugTrace.TraceOn(new object[] { items });

            foreach (T item in items)
            {
                this.Add(item);
            }

            YDebugTrace.TraceOff();
        }

        /// <summary>
        /// Add a record at the end of the dataset.
        /// </summary>
        /// <param name="item">The record to insert.</param>
        public new void Add(T item)
        {
            YDebugTrace.TraceOn(new object[] { item });

            this.Insert(this.Count, item);

            YDebugTrace.TraceOff();
        }

        /// <summary>
        /// Get a record by its internal index.
        /// </summary>
        /// <param name="internalIndex">The internal index.</param>
        /// <returns>The record found or <c>null</c>.</returns>
        public T GetRecordByInternalIndex(long internalIndex)
        {
            return YDebugTrace.Trace(this.Find(x => x.InternalIndex == internalIndex), new object[] { internalIndex });
        }

        /// <summary>
        /// Check if a record is present in the dataset.
        /// </summary>
        /// <param name="item">The record to check.</param>
        /// <returns><c>true</c> or <c>false</c>.</returns>
        public new bool Contains(T item)
        {
            return YDebugTrace.Trace(base.Contains(item)
                || this.Any(x => x.InternalIndex == item.InternalIndex), new object[] { item });
        }
    }
}
