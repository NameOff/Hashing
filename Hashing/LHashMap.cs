using System;
using System.Collections.Generic;

namespace Hashing
{
    public class LHashMap<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private class Record
        {
            public readonly TKey Key;
            public TValue Value;
            public Record Next;

            public Record(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public override string ToString()
            {
                return $"{Key} {Value}";
            }
        }

        public int Count { get; private set; }
        private int splitPointer;
        private int hashFunction;
        public const double LoadFactor = 0.75;
        private readonly int capacity;
        private List<Record> buckets;

        public LHashMap(int capacity = 2)
        {
            if (capacity < 0)
                throw new ArgumentException("Capacity value should not be negative number");
            this.capacity = capacity;
            Initialize();
        }

        public TValue this[TKey key]
        {
            get { return FindRecord(key).Value; }
            set { Insert(key, value); }
        }

        private void Initialize()
        {
            splitPointer = 0;
            buckets = new List<Record> { null, null };
            hashFunction = 2;
        }

        private Record FindRecord(TKey key)
        {
            var bucketIndex = CalculateBucketIndex(key);
            var record = buckets[bucketIndex];
            while (record != null)
            {
                if (Equals(record.Key, key))
                    return record;
                record = record.Next;
            }
            throw new KeyNotFoundException();
        }

        private double CalculateLoadFactor()
        {
            return (double)Count / (capacity * buckets.Count);
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value);
        }

        private int CalculateBucketIndex(TKey key)
        {
            var hash = key.GetHashCode();
            var potentialIndex = Math.Abs(hash % hashFunction);
            return potentialIndex >= splitPointer ? potentialIndex : Math.Abs(hash % (hashFunction * 2));
        }

        public void Remove(TKey key)
        {
            var index = CalculateBucketIndex(key);
            var record = buckets[index];
            Record previousRecord = null;
            while (record != null)
            {
                if (Equals(record.Key, key))
                {
                    if (previousRecord == null)
                        buckets[index] = buckets[index].Next;
                    else
                        previousRecord.Next = record.Next;
                    Count--;
                    return;
                }
                previousRecord = record;
                record = record.Next;
            }
            throw new KeyNotFoundException();
        }

        private void Insert(TKey key, TValue value)
        {
            var index = CalculateBucketIndex(key);
            var record = new Record(key, value);
            bool countMustChange;
            InsertToBucket(record, index, out countMustChange);
            if (countMustChange)
                Count++;
            var loadFactor = CalculateLoadFactor();
            if (loadFactor > LoadFactor)
                Split();
        }

        private void InsertToBucket(Record record, int bucketIndex, out bool countMustChange)
        {
            if (buckets[bucketIndex] == null)
                buckets[bucketIndex] = record;
            else
            {
                var rec = buckets[bucketIndex];
                while (true)
                {
                    if (Equals(rec.Key, record.Key))
                    {
                        rec.Value = record.Value;
                        countMustChange = false;
                        return;
                    }
                    if (rec.Next == null)
                        break;
                    rec = rec.Next;
                }
                rec.Next = record;
            }
            countMustChange = true;
        }

        private void Split()
        {
            buckets.Add(null);
            UpdateSplitPointer();
            var split = GetPreviousSplitPointer();
            if (buckets[split] == null)
                return;
            Record prevRecord = null;
            var record = buckets[split];
            while (record != null)
            {
                var next = record.Next;
                var bucketIndex = CalculateBucketIndex(record.Key);
                if (bucketIndex != split)
                {
                    if (prevRecord != null)
                        prevRecord.Next = record.Next;
                    else
                        buckets[split] = record.Next;
                    bool _;
                    record.Next = null;
                    InsertToBucket(record, bucketIndex, out _);
                }
                if (record.Next != null)
                    prevRecord = record;
                record = next;
            }
        }

        private int GetPreviousSplitPointer()
        {
            if (splitPointer == 0)
                return hashFunction / 2 - 1;
            return splitPointer - 1;
        }

        private void UpdateSplitPointer()
        {
            if (splitPointer == hashFunction - 1)
            {
                splitPointer = 0;
                hashFunction *= 2;
            }
            else
                splitPointer++;
        }
    }
}
