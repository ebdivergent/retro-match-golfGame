using System.Collections.Generic;
using UnityEngine;

namespace AppCore 
{
    public static class ListExtensions 
    {
        public static int[] ToArrayOfIndexes(this int count) 
        {
            int[] returned = new int[count];

            while (count > 0)
                returned[count] = --count;

            return returned;
        }

        public static T GetAndRemove<T>(this IList<T> list, int index)
        {
            var itemReturned = list[index];

            list.RemoveAt(index);

            return itemReturned;
        }

        public static T GetRandomAndRemove<T>(this IList<T> list) 
        {
            int randomIndex = Random.Range(0, list.Count);

            return GetAndRemove(list, randomIndex);
        }

        public static T GetLastAndRemove<T>(this IList<T> list)
        {
            int lastIndex = list.Count - 1;

            return GetAndRemove(list, lastIndex);
        }

        public static void Shuffle<T>(this IList<T> list) 
        {
            List<T> tmpContainer = new List<T>();

            tmpContainer.AddRange(list);

            list.Clear();

            while (tmpContainer.Count != 0)
                list.Add(tmpContainer.GetRandomAndRemove());
        }

        public static int GetCyclicIndexWithOffset<T>(this ICollection<T> collection, 
            int origin, 
            int offset) 
        {
            int indexCounted = origin + (offset % collection.Count);

            return (indexCounted < 0 ?
                indexCounted + collection.Count : indexCounted) 
                % collection.Count;
        }
    }
}
