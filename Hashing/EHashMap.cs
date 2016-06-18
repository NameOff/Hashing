using System;
using System.Collections.Generic;

namespace Hashing
{
    public class EHashMap<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private class Bucket
        {
            public int LocalDepth;
            public static int Capacity;
            public readonly Dictionary<TKey, TValue> Entries;

            public Bucket()
            {
                LocalDepth = 1;
                Entries = new Dictionary<TKey, TValue>(Capacity);
            }

            public bool IsFull => Entries.Count >= Capacity;

            public void Put(TKey key, TValue value)
            {
                Entries[key] = value;
            }

            public TValue Get(TKey key)
            {
                return Entries[key];
            }

            public bool ContainsKey(TKey key)
            {
                return Entries.ContainsKey(key);
            }
        }

        private int globalDepth;
        private readonly List<Bucket> buckets;

        public EHashMap(int capacity = 300)
        {
            if (capacity < 0)
                throw new ArgumentException();
            Bucket.Capacity = capacity;
            buckets = new List<Bucket> { new Bucket(), new Bucket() };
            globalDepth = 1;
        }

        private Func<int, int> KeyFunc => hash => hash & ((1 << globalDepth) - 1);

        private int GetBucketIndex(TKey key)
        {
            var hash = key.GetHashCode();
            return KeyFunc(hash);
        }

        public void Remove(TKey key)
        {
            var bucketIndex = GetBucketIndex(key);
            var bucket = buckets[bucketIndex];
            if (!bucket.Entries.ContainsKey(key))
                throw new KeyNotFoundException();
            bucket.Entries.Remove(key);
            Count--;
        }

        public TValue this[TKey key]
        {
            get { return Get(key); }
            set { Add(key, value); }
        }

        public int Count { get; private set; }

        public TValue Get(TKey key)
        {
            var bucket = buckets[GetBucketIndex(key)];
            return bucket.Get(key);
        }

        private void ExpandBuckets()
        {
            var bucketsCopy = new List<Bucket>(buckets);
            buckets.AddRange(bucketsCopy);
            globalDepth++;
        }

        private void SplitBucket(int bucketIndex)
        {
            var bucket = buckets[bucketIndex];
            var newBuckets = new List<Bucket>() { new Bucket(), new Bucket() };

            foreach (var key in bucket.Entries.Keys)
            {
                var value = bucket.Get(key);
                var hash = KeyFunc(key.GetHashCode());

                if ((hash | (1 << bucket.LocalDepth)) == hash)
                    newBuckets[1].Put(key, value);
                else
                    newBuckets[0].Put(key, value);
            }

            var indexes = new List<int> { bucketIndex, bucketIndex + buckets.Count / 2 };

            foreach (var index in indexes)
            {
                if ((index | (1 << bucket.LocalDepth)) == index)
                    buckets[index] = newBuckets[1];
                else
                    buckets[index] = newBuckets[0];
                buckets[index].LocalDepth = bucket.LocalDepth + 1;
            }
        }

        public void Add(TKey key, TValue value)
        {
            var bucketIndex = GetBucketIndex(key);
            if (bucketIndex - buckets.Count / 2 >= 0 && buckets[bucketIndex] == buckets[bucketIndex - buckets.Count / 2])
                bucketIndex -= buckets.Count / 2;
            var bucket = buckets[bucketIndex];

            if (!bucket.ContainsKey(key))
                Count++;
            bucket.Put(key, value);

            if (bucket.IsFull)
            {
                if (bucket.LocalDepth == globalDepth)
                    ExpandBuckets();
                if (bucket.LocalDepth < globalDepth)
                    SplitBucket(bucketIndex);
            }
        }
    }
}
