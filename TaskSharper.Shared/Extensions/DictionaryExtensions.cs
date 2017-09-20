using System.Collections.Generic;

namespace TaskSharper.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
            return dictionary;
        }
    }
}
