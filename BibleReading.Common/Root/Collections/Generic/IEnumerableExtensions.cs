using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibleReading.Common45.Root.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static List<KeyValuePair<string, string>> ToKeyValuePair(this IEnumerable<string[]> enumerable)
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
            foreach (string[] s in enumerable)
            {
                if(s.Length >= 2)
                    ret.Add(new KeyValuePair<string, string>(s[0], s[1]));
                else
                    ret.Add(new KeyValuePair<string, string>(s[0], s[0]));
            }

            return ret;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();

            foreach(TSource element in source)
            {
                if(seenKeys.Add(keySelector(element)))
                    yield return element;
            }
        }
    }
}
