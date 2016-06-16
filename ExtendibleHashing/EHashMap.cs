using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HashTable;

namespace ExtendibleHashing
{
    public class EHashMap<TKey, TValue> : HashTable.IDictionary<TKey, TValue>
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
        }

        private int globalDepth;
        private readonly List<Bucket> buckets;
        private Stopwatch watch = new Stopwatch();

        public EHashMap(int capacity=1000)
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
            set
            {
                Add(key, value);
            }
        }

        public int Count { get; private set; }
        
        public TValue Get(TKey key)
        {
            Console.WriteLine(watch.ElapsedMilliseconds);
            var bucket = buckets[GetBucketIndex(key)];
            return bucket.Get(key);
        }

        public void Add(TKey key, TValue value)
        {
            var bucketIndex = GetBucketIndex(key);
            if (bucketIndex - buckets.Count / 2 >=0 && buckets[bucketIndex] == buckets[bucketIndex - buckets.Count/2])
                bucketIndex -= buckets.Count/2;
            var bucket = buckets[bucketIndex];

            if (bucket.IsFull && bucket.LocalDepth == globalDepth)
            {
                var bucketsCopy = new List<Bucket>(buckets);
                buckets.AddRange(bucketsCopy);
                globalDepth++;
            }
            
            if (bucket.IsFull && bucket.LocalDepth < globalDepth)
            {
                if (!bucket.Entries.ContainsKey(key))
                    Count++;
                bucket.Put(key, value);

                //buckets[bucketIndex + buckets.Count / 2] = new Bucket();
                //var newBucket = buckets[bucketIndex + buckets.Count / 2];
                //var recordsToDelete = new List<TKey>();
                //foreach (var key2 in bucket.Entries.Keys)
                //{
                //    var hash = KeyFunc(key2.GetHashCode());
                //    if ((hash | (1 << bucket.LocalDepth)) == hash)
                //        recordsToDelete.Add(key2);
                //}

                //foreach (var key2 in recordsToDelete)
                //{
                //    newBucket.Put(key2, bucket.Get(key2));
                //    bucket.Entries.Remove(key2);
                //}

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

                //watch.Start();
                var indexes = new List<int>();

                indexes.Add(bucketIndex);
                indexes.Add(bucketIndex + buckets.Count / 2);

                //watch.Stop();

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
                if (!bucket.Entries.ContainsKey(key))
                    Count++;
                bucket.Put(key, value);
            }
        }
    }
}
