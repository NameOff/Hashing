using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public class Hash<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private class Element
        {
            public readonly TKey Key;
            public TValue Value;
            public Element Next;

            public Element(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private readonly int[] primes =
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
        };

        private Element[] buckets;
        public int Count { get; private set; }

        public Hash(int capacity = 0)
        {
            if (capacity < 0)
                throw new ArgumentException("Capacity value should not be a negative number");
            var size = GetPrime(capacity);
            buckets = new Element[size];
        }

        public TValue this[TKey key]
        {
            get
            {
                Element elem;
                if (TryFindElement(key, out elem))
                    return elem.Value;
                throw new KeyNotFoundException();
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            var hash = key.GetHashCode();
            var bucketIndex = hash % buckets.Length;
            var elem = buckets[bucketIndex];
            if (elem == null)
            {
                buckets[bucketIndex] = new Element(key, value);
                Count++;
                if (Count == buckets.Length)
                    Resize();
                return;
            }
            while (elem != null)
            {
                if (Equals(elem.Key, key))
                {
                    elem.Value = value;
                    return;
                }
                if (elem.Next == null)
                {
                    elem.Next = new Element(key, value);
                    Count++;
                    if (Count == buckets.Length)
                        Resize();
                    return;
                }
                elem = elem.Next;
            }
        }

        public void Remove(TKey key)
        {
            var bucketIndex = key.GetHashCode() % buckets.Length;
            var curElem = buckets[bucketIndex];
            Element prevElem = null;
            while (curElem != null)
            {
                if (Equals(curElem.Key, key))
                {
                    if (prevElem == null)
                        buckets[bucketIndex] = curElem.Next;
                    else
                        prevElem.Next = curElem.Next;
                    Count--;
                    return;
                }
                prevElem = curElem;
                curElem = curElem.Next;
            }
            throw new KeyNotFoundException();
        }

        private void Resize()
        {
            var capacity = GetPrime(buckets.Length);
            var newBuckets = new Element[capacity];
            foreach (var elem in buckets)
            {
                var curElem = elem;
                while (curElem != null)
                {
                    var newBucketIndex = curElem.Key.GetHashCode() % capacity;
                    if (newBuckets[newBucketIndex] == null)
                        newBuckets[newBucketIndex] = new Element(curElem.Key, curElem.Value);
                    else
                    {
                        var tmp = newBuckets[newBucketIndex];
                        while (tmp.Next != null)
                            tmp = tmp.Next;
                        tmp.Next = new Element(curElem.Key, curElem.Value);
                    }
                    curElem = curElem.Next;
                }
            }
            buckets = newBuckets;
        }

        private bool TryFindElement(TKey key, out Element elem)
        {
            var bucketIndex = key.GetHashCode() % buckets.Length;
            var curElem = buckets[bucketIndex];
            while (curElem != null)
            {
                if (Equals(curElem.Key, key))
                {
                    elem = curElem;
                    return true;
                }
                curElem = curElem.Next;
            }
            elem = null;
            return false;
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
    }
}
