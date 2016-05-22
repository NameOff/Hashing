using System;
using System.Collections.Generic;

namespace HashTable
{
    public class HashTable<TKey, TValue> where TKey : IComparable
    {
        private struct Entry
        {
            public int hashCode;
            public int next;
            public TKey key;
            public TValue value;
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
                throw new ArgumentException();
            Initialize(capacity);
        }

        public TValue this[TKey key]
        {
            get
            {
                var i = FindEntry(key);
                if (i >= 0)
                    return entries[i].value;
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

            for (var i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && entries[i].key.CompareTo(key) == 0)
                {
                    entries[i].value = value;
                    return;
                }
            }

            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next;
                freeCount--;
            }
            else
            {
                if (Count == entries.Length)
                {
                    Resize(GetPrime(Count));
                    targetBucket = hashCode % buckets.Length;
                }
                index = Count;
                Count++;
            }
            entries[index] = new Entry { hashCode = hashCode, next = buckets[targetBucket], key = key, value = value };
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
                if (newEntries[i].hashCode >= 0)
                {
                    var bucket = newEntries[i].hashCode % newSize;
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }
            buckets = newBuckets;
            entries = newEntries;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();

            if (buckets != null)
            {
                var hashCode = key.GetHashCode();
                var bucket = hashCode % buckets.Length;
                var last = -1;
                for (var i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && entries[i].key.CompareTo(key) == 0)
                    {
                        if (last < 0)
                            buckets[bucket] = entries[i].next;
                        else
                            entries[last].next = entries[i].next;
                        entries[i] = new Entry { hashCode = -1, next = freeList, key = default(TKey), value = default(TValue) };
                        freeList = i;
                        freeCount++;
                        Count--;
                        return true;
                    }
                }
            }
            throw new KeyNotFoundException();
        }

        private int FindEntry(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();

            if (buckets != null)
            {
                var hashCode = key.GetHashCode();
                for (var i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
                    if (entries[i].hashCode == hashCode && entries[i].key.CompareTo(key) == 0)
                        return i;
            }
            return -1;
        }


        private void Initialize(int capacity)
        {
            var size = GetPrime(capacity);
            buckets = new int[size];
            for (int i = 0; i < buckets.Length; i++)
                buckets[i] = -1;
            entries = new Entry[size];
            freeList = -1;
        }

        private bool IsPrime(int candidate)
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
