// -----------------------------------------------------------------------
// <copyright file="DictionaryHelpers.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DictionaryHelpers
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultVal)
        {
            TValue ret;
            if (dictionary.TryGetValue(key, out ret))
            {
                return ret;
            }

            return defaultVal;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue ret;
            if (dictionary.TryGetValue(key, out ret))
            {
                return ret;
            }

            return default(TValue);
        }
    }
}