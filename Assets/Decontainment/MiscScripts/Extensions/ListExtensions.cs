using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class ListExtensions
    {
        /// Returns the index inserted at
        public static int InsertAlphabetically<T>(this IList<T> list, T item)
        {
            int index;
            for (index = 0; index < list.Count; ++index) {
                if (string.Compare(item.ToString(), list[index].ToString()) <= 0) {
                    break;
                }
            }
            list.Insert(index, item);
            return index;
        }

        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; ++i) {
                if (predicate(list[i])) {
                    return i;
                }
            }
            return -1;
        }
    }
}