namespace Hashing
{
    public interface IDictionary<in TKey, TValue>
    {
        void Add(TKey key, TValue value);
        void Remove(TKey key);
        TValue this[TKey key] { get; set; }
        int Count { get; }
    }
}
