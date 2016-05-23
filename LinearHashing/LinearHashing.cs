using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearHashing
{
    public class LinearHashing<TKey, TValue>
    {
        private int pointer;
        private int size;

        public LinearHashing(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentException("Capacity value should not be negative number");
            Initialize(capacity);
        }

        public TValue this[TKey key]
        {
            get { throw new NotImplementedException(); }
            set { Insert(key, value); }
        }

        private void Initialize(int capacity)
        {
            pointer = 0;
            size = capacity;
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value);
        }

        public void Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        private void Insert(TKey key, TValue value)
        {
            var hash = key.GetHashCode();
            var targetBucket = hash%size;
            throw new NotImplementedException();
        }
    }
}