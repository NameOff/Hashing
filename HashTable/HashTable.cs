using System;
using System.Collections.Generic;

namespace HashTable
{
    public class HashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private struct Entry
        {
            public int HashCode;
            public int Next;
            public TKey Key;
            public TValue Value;
        }

        private readonly int[] primes =
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
        };

        private int[] buckets;
        private Entry[] entries;
        private int freeList;
        private int freeCount;
        public int Count { get; private set; }

        public HashTable(int capacity = 0)
        {
            if (capacity < 0)
                throw new ArgumentException("Capacity value should not be negative number");
            Initialize(capacity);
        }

        public TValue this[TKey key]
        {
            get
            {
                var i = FindEntry(key);
                if (i >= 0)
                    return entries[i].Value;
                throw new KeyNotFoundException();
            }
            set { Insert(key, value); }
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value);
        }

        private void Insert(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException();

            var hashCode = key.GetHashCode();
            var targetBucket = hashCode % buckets.Length;

            for (var i = buckets[targetBucket]; i >= 0; i = entries[i].Next)
            {
                if (entries[i].HashCode == hashCode && entries[i].Key.Equals(key))
                {
                    entries[i].Value = value;
                    return;
                }
            }

            AddNewEntry(key, value, hashCode, targetBucket);
        }

        private void AddNewEntry(TKey key, TValue value, int hash, int targetBucket)
        {
            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].Next;
                freeCount--;
            }
            else
            {
                if (Count == entries.Length)
                {
                    Resize(GetPrime(Count));
                    targetBucket = hash % buckets.Length;
                }
                index = Count;
                Count++;
            }
            entries[index] = new Entry { HashCode = hash, Next = buckets[targetBucket], Key = key, Value = value };
            buckets[targetBucket] = index;
        }

        private void Resize(int newSize)
        {
            var newBuckets = new int[newSize];
            for (var i = 0; i < newBuckets.Length; i++)
                newBuckets[i] = -1;
            var newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, Count);
            for (var i = 0; i < Count; i++)
            {
                if (newEntries[i].HashCode >= 0)
                {
                    var bucket = newEntries[i].HashCode % newSize;
                    newEntries[i].Next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }
            buckets = newBuckets;
            entries = newEntries;
        }

        public void Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();

            var hashCode = key.GetHashCode();
            var bucket = hashCode % buckets.Length;
            var last = -1;
            for (var i = buckets[bucket]; i >= 0; last = i, i = entries[i].Next)
            {
                if (entries[i].HashCode == hashCode && entries[i].Key.Equals(key))
                {
                    if (last < 0)
                        buckets[bucket] = entries[i].Next;
                    else
                        entries[last].Next = entries[i].Next;
                    entries[i] = new Entry
                    {
                        HashCode = -1,
                        Next = freeList,
                        Key = default(TKey),
                        Value = default(TValue)
                    };
                    freeList = i;
                    freeCount++;
                    Count--;
                    return;
                }
            }

            throw new KeyNotFoundException();
        }

        private int FindEntry(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();

            var hashCode = key.GetHashCode();
            for (var i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].Next)
                if (entries[i].HashCode == hashCode && entries[i].Key.Equals(key))
                    return i;
            
            return -1;
        }


        private void Initialize(int capacity)
        {
            var size = GetPrime(capacity);
            buckets = new int[size];
            for (var i = 0; i < buckets.Length; i++)
                buckets[i] = -1;
            entries = new Entry[size];
            freeList = -1;
        }

        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
                return candidate == 2;

            var limit = (int)Math.Sqrt(candidate);
            for (var divisor = 3; divisor <= limit; divisor += 2)
                if (candidate % divisor == 0)
                    return false;
            return true;
        }

        private int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException();

            foreach (var prime in primes)
                if (prime > min)
                    return prime;


            return GeneratePrimeNumber(min);
        }

        private int GeneratePrimeNumber(int min)
        {
            for (var i = min + 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i))
                    return i;
            }
            return min;
        }
    }
}
