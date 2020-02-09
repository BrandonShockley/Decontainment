using System.Collections.Generic;

namespace Extensions
{
    public static class ListExtensions
    {
        /// Returns the index inserted at
        public static int InsertAlphabetically<T>(this List<T> list, T item)
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
    }
}