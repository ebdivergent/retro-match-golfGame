using System.Collections.Generic;

namespace AppCore 
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default) 
        {
            TValue outValue;

            if (dict.TryGetValue(key, out outValue)) 
                return outValue;

            return defaultValue;
        }

        public static TValue GetValueOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : class, new()
        {
            TValue outValue;

            if (dict.TryGetValue(key, out outValue))
                return outValue;

            TValue createdValue = new TValue();
            dict.Add(key, createdValue);
            return createdValue;
        }
    }
}
