using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTaskManagerShared.Utils
{
    public static class ListUtil
    {
        public static void MatchTo<T>(this IList<T> list, IReadOnlyList<T> other)
        {
            list.RemoveRange(list.Except(other).ToList());
            for (int i = 0; i < other.Count; i++)
            {
                if (i > list.Count - 1)
                {
                    list.Add(other[i]);
                }
                else if (!Equals(other[i], list[i]))
                {
                    list.Insert(i, other[i]);
                }
            }
        }

        public static void RemoveRange<TValue>(this IList<TValue> collection, IEnumerable<TValue> source)
        {
            foreach (TValue item in source)
            {
                collection.Remove(item);
            }
        }

        public static void AddRange<TValue>(this IList<TValue> collection, IEnumerable<TValue> source)
        {
            foreach (TValue item in source)
            {
                collection.Add(item);
            }
        }
    }
}
