using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    public interface IDictionary<in TKey, TValue>
    {
        void Add(TKey key, TValue value);
        void Remove(TKey key);
        TValue this[TKey key] { get; set; }
        int Count { get; }
    }
}
