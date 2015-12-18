using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam
{
    public static class DictionaryExtensions
    {

        public static T TryGet<T>(this Dictionary<Guid, T> dictionary, Guid key)
        {
            T result;
            dictionary.TryGetValue(key, out result);
            return result;
        }

        public static T TryGet<T>(this Dictionary<string, T> dictionary, string key)
        {
            T result;
            dictionary.TryGetValue(key, out result);
            return result;
        }
    }
}