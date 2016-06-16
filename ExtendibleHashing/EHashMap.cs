using System;
using System.Collections.Generic;
using HashTable;

namespace ExtendibleHashing
{
    public class EHashMap<TKey, TValue> : HashTable.IDictionary<TKey, TValue>
    {
        public class Bucket
        {
            public int LocalDepth;
            private const int Capacity = 3;
            public readonly Dictionary<TKey, TValue> Entries;

            public Bucket()
            {
                LocalDepth = 0;
                Entries = new Dictionary<TKey, TValue>();
            }

            public bool IsFull => Entries.Count > Capacity;

            public void Put(TKey key, TValue value)
            {
                Entries.Add(key, value);
            }

            public TValue Get(TKey key)
            {
                return Entries[key];
            }
        }

        private int globalDepth;
        private readonly List<Bucket> buckets;
        
        public EHashMap()
        {
            buckets = new List<Bucket> {new Bucket()};
        }

        private Func<int, int> KeyFunc => hash => hash & ((1 << globalDepth) - 1);

        private Bucket GetBucket(TKey key)
        {
            var hash = key.GetHashCode();
            var directory = buckets[KeyFunc(hash)];
            return directory;
        }

        public void Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get { return Get(key); }
            set
            {
               Add(key, value);
            }
        }

        public int Count { get; private set; }

        public TValue Get(TKey key)
        {
            return GetBucket(key).Get(key);
        }

        public void Add(TKey key, TValue value)
        {
            var bucket = GetBucket(key);
            if (bucket.IsFull && bucket.LocalDepth == globalDepth)
            {
                var bucketsCopy = new List<Bucket>(buckets);
                buckets.AddRange(bucketsCopy);
                globalDepth++;
            }

            if (bucket.IsFull && bucket.LocalDepth < globalDepth)
            {
                bucket.Put(key, value);
                var bucket1 = new Bucket();
                var bucket2 = new Bucket();

                foreach (var key2 in bucket.Entries.Keys)
                {
                    var value2 = bucket.Get(key2);
                    var hash = key2.GetHashCode() & ((1 << globalDepth) - 1);

                    if ((hash | (1 << bucket.LocalDepth)) == hash)
                        bucket2.Put(key2, value2);
                    else
                        bucket1.Put(key2, value2);
                }

                var indexes = new List<int>();

                for (var i = 0; i < buckets.Count; i++)
                {
                    if (buckets[i] == bucket)
                        indexes.Add(i);
                }

                foreach (var index in indexes)
                {
                    if ((index | (1 << bucket.LocalDepth)) == index)
                        buckets[index] = bucket2;
                    else
                        buckets[index] = bucket1;
                }

                bucket1.LocalDepth = bucket.LocalDepth + 1;
                bucket2.LocalDepth = bucket.LocalDepth + 1;

            }
            else
            {
                bucket.Put(key, value);
            }
        }
    }
}
