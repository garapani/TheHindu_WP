using System.Collections.Generic;

namespace TheHindu.Helper
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                list.Add(item);
            }
        }
    }
}